﻿using System;
using System.Threading.Tasks;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Reputations.Abstractions;

namespace Plato.Articles.Categories.Follow.Subscribers
{
    public class FollowSubscriber : IBrokerSubscriber
    {
    
        private readonly IUserReputationAwarder _reputationAwarder;
        private readonly IBroker _broker;

        public FollowSubscriber(
            IUserReputationAwarder reputationAwarder,
            IBroker broker)
        {
            _reputationAwarder = reputationAwarder;
            _broker = broker;
        }

        public void Subscribe()
        {

            // Created
            _broker.Sub<Follows.Models.Follow>(new MessageOptions()
            {
                Key = "FollowCreated"
            }, async message => await FollowCreated(message.What));

            // Deleted
            _broker.Sub<Follows.Models.Follow>(new MessageOptions()
            {
                Key = "FollowDeleted"
            }, async message => await FollowDeleted(message.What));

        }

        public void Unsubscribe()
        {
            // Created
            _broker.Unsub<Follows.Models.Follow>(new MessageOptions()
            {
                Key = "FollowCreated"
            }, async message => await FollowCreated(message.What));

            // Deleted
            _broker.Unsub<Follows.Models.Follow>(new MessageOptions()
            {
                Key = "FollowDeleted"
            }, async message => await FollowDeleted(message.What));

        }

        private async Task<Follows.Models.Follow> FollowCreated(Follows.Models.Follow follow)
        {

            if (follow == null)
            {
                return null;
            }

            // Is this a channel follow?
            if (!follow.Name.Equals(FollowTypes.Category.Name, StringComparison.OrdinalIgnoreCase))
            {
                return follow;
            }

            // Award reputation for following category
            await _reputationAwarder.AwardAsync(Reputations.NewFollow, follow.CreatedUserId, "Followed an article category");

            return follow;

        }

        private async Task<Plato.Follows.Models.Follow> FollowDeleted(Plato.Follows.Models.Follow follow)
        {

            if (follow == null)
            {
                return null;
            }

            // Is this the correct follow?
            if (!follow.Name.Equals(FollowTypes.Category.Name, StringComparison.OrdinalIgnoreCase))
            {
                return follow;
            }

            // Revoke reputation for unfollowing category
            await _reputationAwarder.RevokeAsync(Reputations.NewFollow, follow.CreatedUserId, "Unfollowed an article category");

            return follow;

        }

    }

}
