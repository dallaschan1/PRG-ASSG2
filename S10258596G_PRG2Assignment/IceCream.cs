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
                flavours += flavour;
            }

            
            List<string> tops = new List<string>() { "sprinkles", "mochi", "sago", "oreos" };
            string toppings = "";


            for (int i = 0; i < Toppings.Count; i++)
            {
                Topping topping = Toppings[i];
                string type = topping.Type.ToLower();
                if (tops.Contains(type))
                {
                    toppings += topping.Type; 
                    if (i != Toppings.Count - 1) 
                    {
                        toppings += ", ";
                    }
                }
            }

    
            if (toppings.EndsWith(", "))
            {
                toppings = toppings.Substring(0, toppings.Length - 2);
            }

            if (toppings == "")
            {
                toppings = "None";
            }

            string output = $"Option: {Option}\tScoops: {Scoops}\tFlavours: {flavours}\tToppings: {toppings}\t";

            return output;
        }
    }
}
