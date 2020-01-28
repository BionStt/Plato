﻿using System.Collections.Generic;
using Plato.Entities.Models;
using PlatoCore.Abstractions;

namespace Plato.Issues.Models
{

    public class ArticleDetails : Serializable
    {
        public IEnumerable<EntityUser> LatestUsers { get; set; } = new List<EntityUser>();
    }
    
}
