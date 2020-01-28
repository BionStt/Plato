﻿using PlatoCore.Models.Reputations;

namespace PlatoCore.Models.Badges
{

    public interface IBadge
    {

        string Category { get; set; }

        string Name { get; set; }

        string Title { get; set; }

        string Description { get; set; }

        string BackgroundIconCss { get; set; }

        string IconCss { get; set; }

        int Threshold { get; set; }

        int BonusPoints { get; set; }
        
        BadgeLevel Level { get; set; }

        IReputation GetReputation();
    }

    public enum BadgeLevel
    {
        Gold,
        Silver,
        Bronze
    }


}
