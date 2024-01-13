using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2
{
    internal class Customer
    {
        public string name { get; set; }
        public int memberId { get; set; }

        public DateTime dob { get; set; }

        public Order currentOrder { get; set; }

        public List<Order> orderHistory { get; set; }

        public PointCard rewards { get; set; }

        public Customer() 
        {
            orderHistory = new List<Order>();
        }

        public Customer(string name, int memberId, DateTime dob)
        {
            this.name = name;
            this.memberId = memberId;
            this.dob = dob;
            currentOrder = null;
            rewards = null;

        }

        public Order MakeOrder()
        {
            Order order = new Order();

            return order;
        }

        public bool IsBirthday()
        {
            DateTime today = DateTime.Today;
            if ((today.Day == dob.Day) && (today.Month == dob.Month))
            {
                return true;
            }
            else
            {
                return false;
            }


          
        }

        public override string ToString()
        {
            return $"{name}, {memberId}, {dob}";
        }

    }
    


    

    
}
