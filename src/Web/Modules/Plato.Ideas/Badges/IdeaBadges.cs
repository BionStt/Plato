﻿using System.Collections.Generic;
using PlatoCore.Badges.Abstractions;
using PlatoCore.Models.Badges;

namespace Plato.Ideas.Badges
{
    public class IdeaBadges : IBadgesProvider<Badge>
    {

        public static readonly Badge First =
            new Badge("IdeaBadgesFirst",
                "First Idea",
                "Posted an idea",
                "fal fa-lightbulb",
                BadgeLevel.Bronze,
                1,
                0);

        public static readonly Badge Bronze =
            new Badge("IdeaBadgesBronze",
                "Inventive",
                "Posted several ideas",
                "fal fa-lightbulb",
                BadgeLevel.Bronze,
                10,
                5);

        public static readonly Badge Silver =
            new Badge("IdeaBadgesSilver",
                "Full of Em",
                "Contributed many ideas",
                "fal fa-lightbulb", 
                BadgeLevel.Silver,
                25,
                10);
        
        public static readonly Badge Gold =
            new Badge("IdeaBadgesGold",
                "Ideator",
                "Contributed dozens of ideas",
                "fal fa-lightbulb",
                BadgeLevel.Gold,
                50,
                20);

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