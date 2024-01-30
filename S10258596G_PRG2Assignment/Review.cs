using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2
{
    internal class Review
    {
        public string FlavourType { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }

        public Review(string flavourType, int rating, string comment)
        {
            FlavourType = flavourType;
            Rating = rating;
            Comment = comment;
        }

       
        public override string ToString()
        {
            string output = $"Rating: {Rating}\nComment: {Comment}";
            return output;
        }
    }

}
