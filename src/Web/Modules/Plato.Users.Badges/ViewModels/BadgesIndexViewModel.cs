﻿using System;
using System.Collections.Generic;
using System.Text;
using PlatoCore.Models.Badges;

namespace Plato.Users.Badges.ViewModels
{
    public class BadgesIndexViewModel
    {
        
        public IEnumerable<IBadgeEntry> Badges { get; set; }

        public BadgesIndexOptions Options { get; set; }

    }

    public class BadgesIndexOptions
    {

        public int UserId { get; set; }

    }

}
