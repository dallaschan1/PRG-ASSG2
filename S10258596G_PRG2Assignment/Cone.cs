using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2
{
    internal class Cone : IceCream
    {
        public bool Dipped { get; set; }

        public Cone() { }

        public Cone(string option, int scoops, List<Flavour> flavours, List<Topping> toppings, bool dipped) : base(option, scoops, flavours, toppings)
        {
            Dipped = dipped;
        }

        public double CalculatePrice()
        {
            double 
        }

        public override string ToString()
        {
            return base.ToString() + $"Dipped: {Dipped}";
        }
    }
}
