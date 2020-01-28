﻿using System.Collections.Generic;
using PlatoCore.Models.Reputations;
using PlatoCore.Reputations.Abstractions;

namespace Plato.Docs
{

    public class Reputations : IReputationsProvider<Reputation>
    {

        public static readonly Reputation NewTopic =
            new Reputation("New Topic", 1);

        public static readonly Reputation NewReply =
            new Reputation("New Reply", 1);

        public IEnumerable<Reputation> GetReputations()
        {
            return new[]
            {
                NewTopic,
                NewReply
            };
        }

    }
}
