﻿using System.Threading.Tasks;
using PlatoCore.Models.Reputations;

namespace PlatoCore.Reputations.Abstractions
{
    public interface IUserReputationAwarder
    {
        Task<UserReputation> AwardAsync(IReputation reputation, int userId, string description);

        Task<UserReputation> RevokeAsync(IReputation reputation, int userId, string description);

    }

}
