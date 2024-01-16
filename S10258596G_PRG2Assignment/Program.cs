using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Globalization;
using System.Xml.Linq;

namespace Assignment2
{


    class Program
    {
        static void Main(string[] args)
        {
            Queue<Order> NormalQueue = new Queue<Order>();
            Queue<Order> GoldQueue = new Queue<Order>();
            Dictionary<int, Customer> customerDic = new Dictionary<int, Customer>();

            // Create customers and orders from the given data files at the onset. 
            void CustomerCreation()
            {
                using (StreamReader sr = new StreamReader("customers.csv"))
                {
                    string? s = sr.ReadLine(); // added the heading so it doenst cause error
                    string[] heading = s.Split(',');

                    while ((s = sr.ReadLine()) != null)
                    {
                        string[] details = s.Split(',');
                        DateTime dob = DateTime.ParseExact(details[2], "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        Customer customerNew = new Customer(details[0], Convert.ToInt32(details[1]), dob);
                        customerDic.Add(customerNew.memberId, customerNew);
                    }
                }
            }

            CustomerCreation();


            // NOT DONE YET!!!!
            /* Commented out
            void OrderCreation()
            {
                // Dataset not given yet.
                using (StreamReader sr = new StreamReader("orders.csv"))
                {
                    string? s = sr.ReadLine(); // read the heading
                    string[] heading = s.Split(',');

                    while ((s = sr.ReadLine()) != null)
                    {
                        string[] details = s.Split(',');
                        Order orderNew;
                    }
                }
            }
            OrderCreation();
            */


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
                do
                {
                    try
                    {
                        Console.Write("Enter your option: ");
                        int option = int.Parse(Console.ReadLine());

                        if (option >= 0 && option <= 6) // Included 0 as a valid option
                        {
                            return option;
                        }
                        else
                        {
                            Console.WriteLine("Invalid input. Please enter a number between 0 and 6.");
                        }
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Invalid input. Please enter a number between 0 and 6.");
                    }
                    catch (OverflowException)
                    {
                        Console.WriteLine("Invalid input. Please enter a number between 0 and 6.");
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
                        Option1();
                        break;

                    case 2:
                        // 2) List all current orders - display the information of all current orders in both the gold members and regular queue
                        Option2();
                        break;

                    case 3:
                        // 3) Register a new customer
                        Option3();
                        break;
                    case 4:
                        break;
                    case 5:
                        Option5();
                        break;
                    case 6:
                        Option6();
                        break;

                }
            }

            while (true)
            {
                DisplayMenu();
                int option = OptionValidation();
                ProcessOption(option);
            }

            void Option1()
            {
                // 1) List all customers - display the information of all the customers
                Console.WriteLine($"{"Name",-10} \t{"Member ID",-6} \t{"Date Of Birth",-10}");
                foreach (KeyValuePair<int, Customer> kvp in customerDic)
                {
                    Customer customer = kvp.Value;
                    Console.WriteLine($"{customer.name,-10} \t{customer.memberId,-6} \t\t{customer.dob.ToShortDateString(),-10}");
                    Console.WriteLine();
                }
            }

            void Option2()
            {
                Console.WriteLine("Orders in the Normal Queue: ");

                foreach (Order order in NormalQueue)
                {
                    Console.WriteLine(order.ToString());
                }
                Console.WriteLine("Orders in the Gold Queue: ");
                foreach (Order order in GoldQueue)
                {
                    Console.WriteLine(order.ToString());
                }
            }

            void Option3()
            {
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
            }

            void Option5()
            {
                if (customerDic.Count > 0)
                {
                    foreach (var entry in customerDic)
                    {
                        Console.WriteLine($"Customer {entry.Key}: {entry.Value.ToString()}");
                    }
                    Console.Write("Which Customer do you wish to select (ID): ");

                    int customerId;
                    while (true)
                    {
                        if (int.TryParse(Console.ReadLine(), out customerId) &&
                            customerDic.TryGetValue(customerId, out Customer selectedCustomer))
                        {
                            if (selectedCustomer.orderHistory != null)
                            {
                                foreach (Order order in selectedCustomer.orderHistory)
                                {
                                    Console.WriteLine(order.ToString());
                                }
                            }
                            else
                            {
                                Console.WriteLine("This customer has no order history.");
                            }
                            break;
                        }
                        Console.Write("Invalid input. Please enter a valid customer ID:");
                    }
                }
                else
                {
                    Console.WriteLine("No customers available.");
                }
            }

            void Option6()
            {
                if (customerDic.Count == 0)
                {
                    Console.WriteLine("There are no customers.");
                    return;
                }

                Customer selectedCustomer = null;

                do
                {
                    foreach (var entry in customerDic)
                    {
                        Console.WriteLine($"Customer {entry.Key}: {entry.Value.ToString()}");
                    }
                    Console.Write("Which Customer do you wish to select (ID or 0 to exit): ");

                    int customerId;
                    while (true)
                    {
                        string inputs = Console.ReadLine();
                        if (int.TryParse(inputs, out customerId) &&
                            customerDic.TryGetValue(customerId, out selectedCustomer))
                        {
                            break;
                        }
                        else if (customerId == 0)
                        {
                            return;
                        }
                        Console.Write("Invalid input. Please enter a valid customer ID (0 to exit):");
                    }

                    if (selectedCustomer.currentOrder == null)
                    {
                        Console.WriteLine("The selected customer does not have a current order. Please select another customer.");
                    }

                } while (selectedCustomer.currentOrder == null);

                Console.WriteLine("\nChoose an action for the ice cream order:");
                Console.WriteLine("[1] Modify an existing ice cream");
                Console.WriteLine("[2] Add a new ice cream");
                Console.WriteLine("[3] Delete an existing ice cream");
                Console.Write("Enter your choice: ");

                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        int iceCreamNumber = 1;
                        foreach (IceCream icecream in selectedCustomer.currentOrder.iceCreamList)
                        {
                            Console.WriteLine($"Ice Cream {iceCreamNumber}: {icecream.ToString()}");
                            iceCreamNumber++;
                        }
                        Console.Write("Which Ice Cream do you wish to modify: ");

                        int selectedIceCreamIndex = 0;
                        while (true)
                        {
                            string inputs = Console.ReadLine();
                            if (int.TryParse(inputs, out selectedIceCreamIndex) &&
                                selectedIceCreamIndex >= 1 &&
                                selectedIceCreamIndex <= selectedCustomer.currentOrder.iceCreamList.Count)
                            {
                                break;
                            }
                            Console.WriteLine($"Invalid input. Please enter a number between 1 and {selectedCustomer.currentOrder.iceCreamList.Count}:");
                        }

                        selectedIceCreamIndex -= 1;
                        selectedCustomer.currentOrder.ModifyIceCream(selectedIceCreamIndex);
                        Console.WriteLine("Successfully Modified!");
                        break;

                    case "2":
                        AddIceCream(selectedCustomer);
                        Console.WriteLine("Successfully Added!");
                        break;

                    case "3":
                        // Existing code for deleting ice cream
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Please enter a number between 1 and 3.");
                        break;
                }
            }

            static void AddIceCream(Customer selectedCustomer)
            {
                List<string> normalFlavor = new List<string>() { "vanilla", "chocolate", "strawberry" };
                List<string> specialFlavor = new List<string>() { "durian", "ube", "sea salt" };
                List<Flavour> flavours = new List<Flavour>();
                List<Topping> toppings = new List<Topping>();



                string GetValidInput(string prompt, List<string> validResponses)
                {
                    string response;
                    do
                    {
                        Console.Write(prompt);
                        response = Console.ReadLine().ToLower();

                        if (string.IsNullOrWhiteSpace(response))
                        {
                            Console.WriteLine("Input cannot be blank. Please try again.");
                            continue;
                        }

                        if (!validResponses.Contains(response))
                        {
                            Console.WriteLine("Invalid input. Please try again.");
                        }
                    }
                    while (string.IsNullOrWhiteSpace(response) || !validResponses.Contains(response));
                    return response;
                }


                int GetNumberOfScoops()
                {
                    return GetValidInput("How many scoops do you want? (Single, Double or Triple): ", new List<string> { "single", "double", "triple" }) switch
                    {
                        "single" => 1,
                        "double" => 2,
                        "triple" => 3
                    };
                }

                void AddFlavors(int numberOfScoops)
                {
                    for (int i = 0; i < numberOfScoops; i++)
                    {
                        string flavorName;
                        do
                        {
                            Console.Write($"Scoop {i + 1}, what flavor would you like? Regular Flavors: (Vanilla, Chocolate, Strawberry), Premium Flavors: (Durian, Sea salt, Ube): ");
                            flavorName = Console.ReadLine().ToLower();

                            if (!normalFlavor.Contains(flavorName) && !specialFlavor.Contains(flavorName))
                            {
                                Console.WriteLine("Error: Invalid flavor. Please choose a valid flavor.");
                            }
                        }
                        while (!normalFlavor.Contains(flavorName) && !specialFlavor.Contains(flavorName));

                        bool isPremium = specialFlavor.Contains(flavorName);


                        Flavour existingFlavour = flavours.FirstOrDefault(f => f.Type == flavorName);
                        if (existingFlavour != null)
                        {
                            existingFlavour.Quantity++;
                        }
                        else
                        {
                            flavours.Add(new Flavour(flavorName, isPremium, 1));
                        }
                    }
                }

                void AddToppings()
                {
                    string response = GetValidInput("Do you wish to add toppings? (yes / no): ", new List<string> { "yes", "no" });
                    if (response == "yes")
                    {
                        string topping;
                        do
                        {
                            topping = GetValidInput("Which topping do you wish to add? (Sprinkles, Mochi, Sago, Oreos, exit to finish): ", new List<string> { "sprinkles", "mochi", "sago", "oreos", "exit" });
                            if (topping != "exit")
                            {
                                toppings.Add(new Topping(topping));


                                Console.WriteLine("Added!");
                            }
                        }
                        while (topping != "exit");
                    }
                }


                string type = GetValidInput("Type of Ice Cream [waffle, cone, cup]: ", new List<string> { "waffle", "cone", "cup" });
                int scoops = GetNumberOfScoops();
                AddFlavors(scoops);
                AddToppings();


                if (type == "waffle")
                {
                    string waffleFlavor = "original";


                    string waffleResponse = GetValidInput("Do you wish to change the flavor of your waffle? (yes / no) ", new List<string> { "yes", "no" });
                    if (waffleResponse == "yes")
                    {
                        waffleFlavor = GetValidInput("Which flavor do u want? (Red velvet, charcoal, or pandan waffle): ", new List<string> { "red velvet", "charcoal", "pandan waffle" });
                    }
                    Waffle newOne = new Waffle(type, scoops, flavours, toppings, waffleFlavor);
                    selectedCustomer.currentOrder.AddIceCream(newOne);
                }
                else if (type == "cone")
                {
                    bool dipped = false;
                    string coneResponse = GetValidInput("Do you wish to dip your cone in chocolate? (yes / no) ", new List<string> { "yes", "no" });
                    if (coneResponse == "yes")
                    {
                        dipped = true;

                    }

                    Cone newOne = new Cone(type, scoops, flavours, toppings, dipped);
                    selectedCustomer.currentOrder.AddIceCream(newOne);
                }
                else if (type == "cup")
                {
                    Cup newOne = new Cup(type, scoops, flavours, toppings);
                    selectedCustomer.currentOrder.AddIceCream(newOne);
                }
            }
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