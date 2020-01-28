﻿using System;
using PlatoCore.Models.Users;

namespace Plato.Entities.Reactions.Models
{
    public class ReactionEntry : Reaction, IReactionEntry
    {

        public ReactionEntry(IReaction reaction)
        {
            Name = reaction.Name;
            Description = reaction.Description;
            Emoji = reaction.Emoji;
            Sentiment = reaction.Sentiment;
            Points = reaction.Points;
        }

        public ISimpleUser CreatedBy { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

    }

}
