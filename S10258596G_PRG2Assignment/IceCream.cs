using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2
{
    internal abstract class IceCream
    {
        public string Option { get; set; }
        public int Scoops { get; set; }
        public List<Flavour> Flavours { get; set; }
        public List<Topping> Toppings { get; set; }

        public IceCream() 
        {
            List<Flavour> Flavours = new List<Flavour>();
            List<Topping> Toppings = new List<Topping>();
        }

        public IceCream(string option, int scoops, List<Flavour> flavours, List<Topping> toppings)
        {
            Option = option;
            Scoops = scoops;
            Flavours = flavours;
            Toppings = toppings;
        }

        public abstract double CalculatePrice();

        public override string ToString()
        {
            string flavours = "";
            foreach (Flavour flavour in Flavours)
            {
                flavours += flavour + "  ";
            }

            string toppings = "";
            foreach (Topping topping in Toppings)
            {
                toppings += topping + "  ";
            }

            string output = $"Option: {Option} \tScopps: {Scoops} \tFlavours: {Flavours} \tToppings: {Toppings}";

            return output;
        }
    }
}
