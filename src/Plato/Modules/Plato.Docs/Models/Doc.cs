﻿using Plato.Entities.Models;

namespace Plato.Docs.Models
{
    public class Doc : Entity
    {
        public bool IsNew { get; set; }
        
        public Doc PreviousDoc { get; set; }

        public Doc NextDoc { get; set; }

    }
}
