﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Docs.Badges;
using PlatoCore.Abstractions.Extensions;
using PlatoCore.Cache.Abstractions;
using PlatoCore.Data.Abstractions;
using PlatoCore.Models.Notifications;
using PlatoCore.Models.Users;
using PlatoCore.Notifications.Abstractions;
using PlatoCore.Stores.Abstractions.Users;
using PlatoCore.Stores.Users;
using PlatoCore.Tasks.Abstractions;
using PlatoCore.Models.Badges;
using PlatoCore.Reputations.Abstractions;
using PlatoCore.Stores.Badges;
using PlatoCore.Badges.NotificationTypes;
using PlatoCore.Features.Abstractions;
using PlatoCore.Notifications.Extensions;

namespace Plato.Docs.Tasks
{
    public class DocBadgesAwarder : IBackgroundTaskProvider
    {

        private const string Sql = @"                       
                DECLARE @date datetimeoffset = SYSDATETIMEOFFSET(); 
                DECLARE @badgeName nvarchar(255) = '{name}';
                DECLARE @threshold int = {threshold};                  
                DECLARE @userId int;
                DECLARE @topics int;
                DECLARE @myTable TABLE
                (
                    Id int IDENTITY (1, 1) NOT NULL PRIMARY KEY,
                    UserId int NOT NULL
                );
                DECLARE MSGCURSOR CURSOR FOR SELECT e.CreatedUserId, COUNT(e.Id) AS Total 
                FROM {prefix}_Entities e
                WHERE e.FeatureId = {featureId} AND NOT EXISTS (
                   SELECT Id FROM {prefix}_UserBadges ub 
                   WHERE ub.UserId = e.CreatedUserId AND ub.BadgeName = @badgeName
                 )
                GROUP BY e.CreatedUserId
                ORDER BY Total DESC

                OPEN MSGCURSOR FETCH NEXT FROM MSGCURSOR INTO @userId, @topics;                    
                WHILE @@FETCH_STATUS = 0
                BEGIN
                    IF (@topics >= @threshold)
                    BEGIN
                        DECLARE @identity int;
                        EXEC {prefix}_InsertUpdateUserBadge 0, @badgeName, @userId, @date, @identity OUTPUT;
                        IF (@identity > 0)
                        BEGIN
                            INSERT INTO @myTable (UserId) VALUES (@userId);                     
                        END
                    END;
                    FETCH NEXT FROM MSGCURSOR INTO @userId, @topics;	                    
                END;
                CLOSE MSGCURSOR;
                DEALLOCATE MSGCURSOR;
                SELECT UserId FROM @myTable;";


        public int IntervalInSeconds => 240;

        public IEnumerable<Badge> Badges => new[]
        {
            DocBadges.First,
            DocBadges.Bronze,
            DocBadges.Silver,
            DocBadges.Gold
        };
        
        private readonly IUserNotificationTypeDefaults _userNotificationTypeDefaults;
        private readonly INotificationManager<Badge> _notificationManager;
        private readonly IUserReputationAwarder _userReputationAwarder;
        private readonly IPlatoUserStore<User> _userStore;
        private readonly IFeatureFacade _featureFacade;
        private readonly ICacheManager _cacheManager;
        private readonly IDbHelper _dbHelper;
        
        public DocBadgesAwarder(
            IUserNotificationTypeDefaults userNotificationTypeDefaults,
            INotificationManager<Badge> notificationManager,
            IUserReputationAwarder userReputationAwarder,
            IPlatoUserStore<User> userStore,
            IFeatureFacade featureFacade,
            ICacheManager cacheManager,
            IDbHelper dbHelper)
        {
            _userNotificationTypeDefaults = userNotificationTypeDefaults;
            _userReputationAwarder = userReputationAwarder;
            _notificationManager = notificationManager;
            _featureFacade = featureFacade;
            _cacheManager = cacheManager;
            _userStore = userStore;
            _dbHelper = dbHelper;
        }

        public async Task ExecuteAsync(object sender, SafeTimerEventArgs args)
        {

            var feature = await _featureFacade.GetFeatureByIdAsync("Plato.Docs");
            if (feature == null)
            {
                return;
            }

            var bot = await _userStore.GetPlatoBotAsync();
            foreach (var badge in this.Badges)
            {

                // Replacements for SQL script
                var replacements = new Dictionary<string, string>()
                {
                    ["{name}"] = badge.Name,
                    ["{threshold}"] = badge.Threshold.ToString(),
                    ["{featureId}"] = feature.Id.ToString()
                };

                var userIds = await _dbHelper.ExecuteReaderAsync<IList<int>>(Sql, replacements, async reader =>
                {
                    var users = new List<int>();
                    while (await reader.ReadAsync())
                    {
                        if (reader.ColumnIsNotNull("UserId"))
                        {
                            users.Add(Convert.ToInt32(reader["UserId"]));
                        }
                    }

                    return users;
                });

                if (userIds?.Count > 0)
                {

                    // Get all users awarded the badge
                    var users = await _userStore.QueryAsync()
                        .Take(userIds.Count, false)
                        .Select<UserQueryParams>(q => { q.Id.IsIn(userIds.ToArray()); })
                        .OrderBy("LastLoginDate", OrderBy.Desc)
                        .ToList();

                    // Send notifications
                    if (users != null)
                    {
                        foreach (var user in users.Data)
                        {

                            // ---------------
                            // Award reputation for new badges
                            // ---------------

                            var badgeReputation = badge.GetReputation();
                            if (badgeReputation.Points != 0)
                            {
                                await _userReputationAwarder.AwardAsync(badgeReputation, user.Id, $"{badge.Name} badge awarded");
                            }

                            // ---------------
                            // Trigger notifications
                            // ---------------

                            // Email notification
                            if (user.NotificationEnabled(_userNotificationTypeDefaults, EmailNotifications.NewBadge))
                            {
                                await _notificationManager.SendAsync(new Notification(EmailNotifications.NewBadge)
                                {
                                    To = user,
                                    From = bot
                                }, badge);
                            }

                            // Web notification
                            if (user.NotificationEnabled(_userNotificationTypeDefaults, WebNotifications.NewBadge))
                            {
                                await _notificationManager.SendAsync(new Notification(WebNotifications.NewBadge)
                                {
                                    To = user,
                                    From = bot
                                }, badge);
                            }

                        }
                    }

                    _cacheManager.CancelTokens(typeof(UserBadgeStore));

                }

            }

        }

    }

}
