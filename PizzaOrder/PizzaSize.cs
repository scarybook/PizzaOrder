﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaOrder
{
    public class PizzaSize
    {
        public string Name { get; set; }

        public string Tooltip { get; set; }

        public double Price { get; set; }

        public PizzaSize()
        {
                
        }

        public PizzaSize(string name, double price)
        {
            this.Name = name;
            this.Price = price;
        }

        public PizzaSize(string name, string tooltip, double price)
        {
            this.Name = name;
            this.Tooltip = tooltip;
            this.Price = price;
        }
    }
}
