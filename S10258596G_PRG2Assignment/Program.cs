using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Assignment2
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create customers and orders from the given data files at the onset.
            // Write your code here
            Dictionary<int, Customer> customerDic = new Dictionary<int, Customer>();

            void CustomerCreation()
            {
                using (StreamReader sr = new StreamReader("customers.csv"))
                {
                    string? s = sr.ReadLine(); // read the heading
                    string[] heading = s.Split(',');

                    while ((s = sr.ReadLine()) != null)
                    {
                        string[] details = s.Split(',');
                        Customer customerNew = new Customer(details[0], Convert.ToInt32(details[1]), DateTime.Parse(details[2]));
                        customerDic.Add(customerNew.memberId, customerNew);
                    }
                }
            }
            CustomerCreation();


            // NOT DONE YET!!!!
            void OrderCreation()
            {
                // Dataset not given yet.
                using (StreamReader sr = new StreamReader("orders.csv"))
                {
                    string? s = sr.ReadLine(); // read the heading
                    string[] heading = s.Split(',');

                    while ((s = sr.ReadLine()) != null)
                    {
                        string[] details = s.Split(',');
                        Order orderNew;
                    }
                }
            }
            OrderCreation();


            // Displays a menu for user to choose to perform each of the feature describe below repeatedly until user chooses to exit from the menu
            void DisplayMenu()
            {
                Console.WriteLine("---------------- M E N U -----------------");
                Console.WriteLine("[1] List all customers");
                Console.WriteLine("[2] List all current orders");
                Console.WriteLine("[3] Register a new customer");
                Console.WriteLine("[4] Create a customer’s order");
                Console.WriteLine("[5] Display order details of a customer");
                Console.WriteLine("[6] Modify order details");
                Console.WriteLine("[0] Exit Program");
                Console.WriteLine("------------------------------------------");
            }

            int OptionValidation()
            {
                // Data Validation and Exception Handling
                do
                {
                    try
                    {
                        Console.Write("Enter your option :");
                        int option = int.Parse(Console.ReadLine());

                        // Check if the entered option is within the valid range
                        if (option >= 1 && option <= 6)
                        {
                            return option;
                        }
                        else
                        {
                            Console.WriteLine("Invalid input. Please enter a number between 1 and 6.");
                        }
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Invalid input. Please enter a number between 1 and 6.");
                    }
                    catch (OverflowException)
                    {
                        Console.WriteLine("Invalid input. Please enter a number between 1 and 6.");
                    }
                } while (true);
            }

            void ProcessOption(int option)
            {
                switch (option)
                {
                    case 0:
                        Console.WriteLine("Thank you & Have a nice day! Bye bye");
                        break;

                    case 1:
                        // 1) List all customers - display the information of all the customers
                        foreach (KeyValuePair<int, Customer> kvp in customerDic)
                        {
                            Console.WriteLine(kvp.Value.ToString());
                        }
                        break;

                    case 2:
                        // 2) List all current orders - display the information of all current orders in both the gold members and regular queue
                        
                        /*
                        foreach (KeyValuePair<int, Order> kvp in orderDic)
                        {
                            ...
                        }
                        */

                        break;

                    case 3:
                        /*
                        3) Register a new customer
                         prompt user for the following information for the customer: name, id number, date of birth
                         create a customer object with the information given
                         create a Pointcard object
                         assign Pointcard object to the customer
                         append the customer information to the customers.csv file
                         display a message to indicate registration status
                        */
                        // Haven't finish data validation and exception handling and the rest
                        Console.Write("Enter Customer Name; ");
                        string name = Console.ReadLine();

                        Console.Write("Enter Customer ID: ");
                        int id = int.Parse(Console.ReadLine());

                        Console.Write("Enter Customer Date Of Birth (dd/MM/yyyy): ");
                        DateTime dob = Convert.ToDateTime(Console.ReadLine());

                        Customer newCustomer = new Customer(name, id, dob);
                        PointCard newPointCard = new PointCard();
                        newCustomer.rewards = newPointCard;

                        string data = $"{name}, {id}, {dob.Date}";

                        using (StreamWriter sw = new StreamWriter("customers.csv", true))
                        {
                            sw.WriteLine(data);
                        }

                        Console.WriteLine("Registration Status: Successful!"); // Might need to shift it in the error handling part instead

                        break;
                }
            }

            DisplayMenu();
            ProcessOption(OptionValidation());
        }
    }
}

/*
1) List all customers
 display the information of all the customers


2) List all current orders
 display the information of all current orders in both the gold members and regular queue


3) Register a new customer
 prompt user for the following information for the customer: name, id number, date of birth
 create a customer object with the information given
 create a Pointcard object
 assign Pointcard object to the customer
 append the customer information to the customers.csv file
 display a message to indicate registration status


4) Create a customer’s order
 list the customers from the customers.csv
 prompt user to select a customer and retrieve the selected customer
 create an order object
 prompt user to enter their ice cream order (option, scoops, flavours, toppings)
 create the proper ice cream object with the information given
 add the ice cream object to the order
 prompt the user asking if they would like to add another ice cream to the order, repeating 
the previous three steps if [Y] or continuing to the next step if [N]
 link the new order to the customer’s current order
 if the customer has a gold-tier Pointcard, append their order to the back of the gold 
members order queue. Otherwise append the order to the back of the regular order queue
 display a message to indicate order has been made successfully


5) Display order details of a customer
 list the customers
 prompt user to select a customer and retrieve the selected customer
 retrieve all the order objects of the customer, past and current
 for each order, display all the details of the order including datetime received, datetime 
fulfilled (if applicable) and all ice cream details associated with the order


6) Modify order details
 list the customers
 prompt user to select a customer and retrieve the selected customer’s current order
 list all the ice cream objects contained in the order
 prompt the user to either [1] choose an existing ice cream object to modify, [2] add an 
entirely new ice cream object to the order, or [3] choose an existing ice cream object to 
delete from the order
o if [1] is selected, have the user select which ice cream to modify then prompt the user 
for the new information for the modifications they wish to make to the ice cream
selected: option, scoops, flavours, toppings, dipped cone (if applicable), waffle flavour 
(if applicable) and update the ice cream object’s info accordingly
o if [2] is selected prompt the user for all the required info to create a new ice cream 
object and add it to the order
o if [3] is selected, have the user select which ice cream to delete then remove that ice 
cream object from the order. But if this is the only ice cream in the order, then simply 
display a message saying they cannot have zero ice creams in an order
 display the new updated order
*/