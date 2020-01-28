﻿using System;
using System.Threading.Tasks;
using Plato.Issues.Models;
using Plato.Entities.Stores;
using PlatoCore.Messaging.Abstractions;
using PlatoCore.Reputations.Abstractions;

namespace Plato.Issues.Follow.Subscribers
{
    public class FollowSubscriber : IBrokerSubscriber
    {

        private readonly IUserReputationAwarder _reputationAwarder;
        private readonly IEntityStore<Issue> _entityStore;
        private readonly IBroker _broker;

        public FollowSubscriber(
            IUserReputationAwarder reputationAwarder,
            IEntityStore<Issue> entityStore,
            IBroker broker)
        {
            _reputationAwarder = reputationAwarder;
            _entityStore = entityStore;
            _broker = broker;
        }

        public void Subscribe()
        {
 
            _broker.Sub<Plato.Follows.Models.Follow>(new MessageOptions()
            {
                Key = "FollowCreated"
            }, async message => await FollowCreated(message.What));
            
            _broker.Sub<Plato.Follows.Models.Follow>(new MessageOptions()
            {
                Key = "FollowDeleted"
            }, async message => await FollowDeleted(message.What));

        }

        public void Unsubscribe()
        {

            _broker.Unsub<Plato.Follows.Models.Follow>(new MessageOptions()
            {
                Key = "FollowCreated"
            }, async message => await FollowCreated(message.What));

            _broker.Unsub<Plato.Follows.Models.Follow>(new MessageOptions()
            {
                Key = "FollowDeleted"
            }, async message => await FollowDeleted(message.What));

        }

        private async Task<Plato.Follows.Models.Follow> FollowCreated(Plato.Follows.Models.Follow follow)
        {

            if (follow == null)
            {
                return null;
            }

            // Is this a tag follow?
            if (!follow.Name.Equals(FollowTypes.Issue.Name, StringComparison.OrdinalIgnoreCase))
            {
                return follow;
            }

            // Ensure the entity we are following still exists
            var existingEntity = await _entityStore.GetByIdAsync(follow.ThingId);
            if (existingEntity == null)
            {
                return follow;
            }

            // Update total follows
            existingEntity.TotalFollows = existingEntity.TotalFollows + 1;

            // Persist changes
            var updatedEntity = await _entityStore.UpdateAsync(existingEntity);
            if (updatedEntity != null)
            {
                // Award reputation for following the entity
                await _reputationAwarder.AwardAsync(Reputations.NewFollow, follow.CreatedUserId, "Followed an issue");
            }
            
            return follow;

        }

        private async Task<Plato.Follows.Models.Follow> FollowDeleted(Plato.Follows.Models.Follow follow)
        {

            if (follow == null)
            {
                return null;
            }

            // Is this the correct follow type?
            if (!follow.Name.Equals(FollowTypes.Issue.Name, StringComparison.OrdinalIgnoreCase))
            {
                return follow;
            }

            // Ensure the entity we are following still exists
            var existingEntity = await _entityStore.GetByIdAsync(follow.ThingId);
            if (existingEntity == null)
            {
                return follow;
            }

            // Update total follows
            existingEntity.TotalFollows = existingEntity.TotalFollows - 1;
        
            // Ensure we don't go negative
            if (existingEntity.TotalFollows < 0)
            {
                existingEntity.TotalFollows = 0;
            }
            
            // Persist changes
            var updatedEntity = await _entityStore.UpdateAsync(existingEntity);
            if (updatedEntity != null)
            {
                // Revoke reputation for following
                await _reputationAwarder.RevokeAsync(Reputations.NewFollow, follow.CreatedUserId, "Unfollowed an issue");
            }
            
            return follow;

        }

    }

}
