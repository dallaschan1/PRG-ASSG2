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

        public List<Review> Reviews { get; set; }

        public IceCream() 
        {
            Flavours = new List<Flavour>();
            Toppings = new List<Topping>();
            Reviews = new List<Review>();
        }

        public IceCream(string option, int scoops, List<Flavour> flavours, List<Topping> toppings)
        {
            Option = option;
            Scoops = scoops;
            Flavours = flavours;
            Toppings = toppings;
            Reviews = new List<Review>();
        }


        public abstract double CalculatePrice();
        public override bool Equals(object obj)
        {
            if (obj is IceCream other)
            {
                return Option == other.Option
                    && Scoops == other.Scoops
                    && Flavours.SequenceEqual(other.Flavours)
                    && Toppings.SequenceEqual(other.Toppings);
            }
            return false;
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Option: {Option}");
            stringBuilder.AppendLine($"Scoops: {Scoops}");

            string flavourText = Flavours.Count > 0
                ? string.Join(", ", Flavours.Select(flavour => $"{flavour.Type} (Premium: {flavour.Premium}, Quantity: {flavour.Quantity})"))
                : "None";
            stringBuilder.AppendLine($"Flavours: {flavourText}");

            string toppingText = Toppings.Count > 0
                ? string.Join(", ", Toppings.Select(topping => topping.Type))
                : "None";
            stringBuilder.AppendLine($"Toppings: {toppingText}");

            return stringBuilder.ToString().TrimEnd();
        }

    }
}
