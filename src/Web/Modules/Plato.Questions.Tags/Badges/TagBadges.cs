﻿using System.Collections.Generic;
using PlatoCore.Badges.Abstractions;
using PlatoCore.Models.Badges;

namespace Plato.Questions.Tags.Badges
{
    public class TagBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge First =
            new Badge("QuestionsTagBadgesFirst",
                "First Question Tag",
                "Added a question tag",
                "fal fa-tag",
                BadgeLevel.Bronze,
                1);
        
        public static readonly Badge Bronze =
            new Badge("QuestionsTagBadgesBronze",
                "Question Tagger",
                "Added {threshold} question tags",
                "fal fa-tag",
                BadgeLevel.Bronze,
                10,
                2);

        public static readonly Badge Silver =
            new Badge("QuestionsTagBadgesSilver",
                "Question Tag Artist",
                "Added {threshold} question tags",
                "fal fa-tag",
                BadgeLevel.Silver,
                25,
                10);

        public static readonly Badge Gold =
            new Badge("QuestionsTagBadgesGold",
                "Question Taxonomist",
                "Added {threshold} question tags",
                "fal fa-tag",
                BadgeLevel.Gold,
                50,
                25);
        
        public IEnumerable<Badge> GetBadges()
        {
            return new[]
            {
                First,
                Bronze,
                Silver,
                Gold
            };

        }
        
    }

}