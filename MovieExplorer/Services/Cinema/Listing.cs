﻿using System.Collections.Generic;

namespace MovieExplorer.Services.Cinema
{
    public class Listing
    {
        public Listing()
        {
            Times = new List<string>();
        }

        public string Title { get; set; }
        public List<string> Times { get; set; }
    }
}