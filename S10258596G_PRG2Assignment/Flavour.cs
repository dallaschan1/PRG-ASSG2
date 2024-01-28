using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Assignment2
{
    internal class Flavour
    {
        public string Type { get; set; }
        public bool Premium { get; set; }
        public int Quantity { get; set; }

        public Flavour() { }

        public Flavour(string type, bool premium, int quantity)
        {
            Type = type;
            Premium = premium;
            Quantity = quantity;
        }

        public override bool Equals(object obj)
        {
            return obj is Flavour other &&
                   Type == other.Type &&
                   Premium == other.Premium &&
                   Quantity == other.Quantity;
        }

        public override string ToString()
        {
            return $"Type: {Type}  Premium: {Premium}  Quantity: {Quantity},  ";
        }
    }
}
