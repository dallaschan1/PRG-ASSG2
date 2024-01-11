using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2
{
    internal class Waffle
    {
        public string WaffleFlavour { get; set; }

        public Waffle(string option, int scoops, List<Flavour> flavours, List<Topping> toppings, string waffleFlavour) : base(option, scoops, flavours, toppings)
        {
            WaffleFlavour = waffleFlavour;
        }

        public double CalculatePrice()
        {

        }

        public override string ToString()
        {
            return base.ToString() + $"WaffleFlavour: {WaffleFlavour}";
        }
    }
}
