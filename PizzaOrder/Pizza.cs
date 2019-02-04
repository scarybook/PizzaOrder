using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PizzaOrder
{
    public class Pizza
    {
        public string Name { get; set; }
        public List<string> AdditivesList { get; set; }
        public PizzaSize PizzaSize { get; set; }
        public double TotalPrice { get; set; }

        public Pizza(Pizza bazowa)
        {
            this.Name = bazowa.Name;
            this.AdditivesList = bazowa.AdditivesList;
            this.PizzaSize = bazowa.PizzaSize;
            this.TotalPrice = bazowa.TotalPrice;
        }

        public Pizza(string name, List<string> list)
        {
            this.Name = name;
            this.AdditivesList = list;
        }

        public void CalculatePrice()
        {
            TotalPrice = PizzaSize.Price + AdditivesList.Count * 2.5;
        }

    }
}
