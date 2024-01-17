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

        public override double CalculatePrice()
        {
            double singleScopp = 4.00;
            double doubleScopp = 5.50;
            double threeScoop = 6.50;
            double totalPrice = 0.00;

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
            double totalFlavourPrice = 0.00;
            foreach (Flavour flavour in Flavours)
            {
                if (flavour.Premium)
                {
                    totalFlavourPrice += 2.00;
                }
            }
            totalPrice += totalFlavourPrice;

            // Adding any additional topping costs if there are any premium toppings
            double toppingPrice = Toppings.Count() * 1.00;
            totalPrice += toppingPrice;

            // Adding any additional fees for chocolate-dipped cone
            if (Dipped)
            {
                totalPrice += 2.00;
            }


            return totalPrice;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder(base.ToString());
            stringBuilder.AppendLine($"\nDipped: {Dipped}");
            return stringBuilder.ToString();
        }
    }
}
