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
        public override bool Equals(object obj)
        {
            return obj is Topping other && Type == other.Type;
        }
        public override string ToString()
        {
          

            return $"{Type}";
        }
    }
}
