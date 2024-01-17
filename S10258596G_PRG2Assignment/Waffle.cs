using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2
{
    internal class Waffle : IceCream
    {
        public string WaffleFlavour { get; set; }

        public Waffle(string option, int scoops, List<Flavour> flavours, List<Topping> toppings, string waffleFlavour) : base(option, scoops, flavours, toppings)
        {
            WaffleFlavour = waffleFlavour;
            // Values are "Original", "Red velvet", "charcoal", or "pandan"
        }

        public override double CalculatePrice()
        {
            double singleScopp = 7.00;
            double doubleScopp = 8.50;
            double threeScoop = 9.50;
            double totalPrice = 0.00;
            double totalFlavourPrice = 0.00;

            // Adding Price depending on the number of scoops ordered
            switch (Scoops)
            {
                case 1:
                    totalPrice += singleScopp;
                    break;
                case 2:
                    totalPrice += doubleScopp;
                    break;
                case 3:
                    totalPrice -= threeScoop;
                    break;
            }

            // Adding any additional costs if there are any premium flavours
            foreach (Flavour flavour in Flavours)
            {
                if (flavour.Premium)
                {
                    totalPrice += 2.00;
                }
            }

            // Adding any additional topping costs if there are any premium flavours
            double toppingPrice = Toppings.Count() * 1.00;
            totalPrice += toppingPrice;

            // Adding any additional fees for flavoured waffles
            if (WaffleFlavour != "Original")
            {
                totalPrice += 3.00;
            }


            return totalPrice;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder(base.ToString());
            stringBuilder.AppendLine($"\nWaffle Flavour: {WaffleFlavour}");
            return stringBuilder.ToString();
        }

    }
}
