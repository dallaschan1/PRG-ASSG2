using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2
{
    internal class Topping
    {
        public string Type { get; set; }

        public Topping() { }

        public Topping(string type)
        {
            Type = type;
        }

        public override string ToString()
        {
            return $"Type: {Type}";
        }
    }
}
