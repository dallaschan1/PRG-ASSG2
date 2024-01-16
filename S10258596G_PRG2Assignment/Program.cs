using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Globalization;
using System.Xml.Linq;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Text.RegularExpressions;
using System.Linq.Expressions;

namespace Assignment2
{


    class Program
    {
        static void Main(string[] args)
        {
            Queue<Order> NormalQueue = new Queue<Order>();
            Queue<Order> GoldQueue = new Queue<Order>();
            Dictionary<int, Customer> customerDic = new Dictionary<int, Customer>();
            Dictionary<int, Order> orderDic = new Dictionary<int, Order>();

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
                        DateTime dob = DateTime.ParseExact(details[2], "d/M/yyyy", CultureInfo.InvariantCulture);
                        PointCard pointCard = new PointCard(Int32.Parse(details[4]),Int32.Parse(details[5]));
                        Customer customerNew = new Customer(details[0], Convert.ToInt32(details[1]), dob);
                        customerNew.rewards = pointCard;
                        customerDic.Add(customerNew.memberId, customerNew);

                    }
                }
            }

            CustomerCreation();


            bool CheckPremiumFlavour(string flavour)
            {
                List<string> specialFlavour = new List<string>() { "Durian", "Ube", "Sea Salt" };
                if (specialFlavour.Contains(flavour))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            void OrderCreation()
            {
                using (StreamReader sr = new StreamReader("orders.csv"))
                {
                    string? s = sr.ReadLine(); // read the heading
                    string[] heading = s.Split(',');

                    while ((s = sr.ReadLine()) != null)
                    {
                        string[] details = s.Split(',');
                        int id = Convert.ToInt32(details[0]);
                        DateTime timeReceived = DateTime.ParseExact(details[2], "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                        DateTime? timeFulfilled;
                        timeFulfilled = DateTime.ParseExact(details[3], "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

                        // Creating individual IceCreams
                        string option = details[4];
                        int scoops = Convert.ToInt32(details[5]);

                        // Adding all the flavours into flavourList
                        List<Flavour> flavourList = new List<Flavour>();
                        Dictionary<string, int> flavourCount = new Dictionary<string, int>();
                        int premiumFlavourCount = 0;
                        for (int i = 6; i < scoops+6; i++)
                        {
                            if (flavourCount.ContainsKey(details[i]))
                            {
                                flavourCount[details[i]]++;
                            }
                            else
                            {
                                flavourCount.Add(details[i], 1);
                            }
                        }
                        foreach (KeyValuePair<string, int> kvp in flavourCount)
                        {
                            Flavour addFlavour = new Flavour(kvp.Key, CheckPremiumFlavour(kvp.Key), kvp.Value);
                        }

                        // Adding all the toppings into toppingList
                        List<Topping> toppingList = new List<Topping>();
                        for (int i = 11; i < 4+11; i++)
                        {
                            if (details[i] != null)
                            {
                                toppingList.Add(new Topping(details[i]));
                            }
                            else { break; }
                        }

                        IceCream newIceCream = null;

                        if (option == "Waffle")
                        {
                            string waffleFlavour = details[7];
                            newIceCream = new Waffle(option, scoops, flavourList, toppingList, waffleFlavour);
                        }
                        else if (option == "Cone")
                        {
                            bool dipped = bool.Parse(details[6]);
                            newIceCream = new Cone(option, scoops, flavourList, toppingList, dipped);
                        }
                        else if (option == "Cup")
                        {
                            newIceCream = new Cup(option, scoops, flavourList, toppingList);
                        }
                        else
                        {
                            Console.WriteLine("Error Occurred! IceCream has not been created!");
                        }
                        // Finish creating IceCream and adding it to iceCreamList

                        List<IceCream> iceCreamList = new List<IceCream> { newIceCream };
                        Order newOrder = new Order(id, timeReceived, timeFulfilled, iceCreamList);

                        if (orderDic.ContainsKey(id))
                        {
                            orderDic[id].IceCreamList.Add(newIceCream);
                        }
                        else
                        {
                            orderDic.Add(id, newOrder);
                        }

                        // Finish creating order for customer
                        int memberID = Int32.Parse(details[1]);
                        Customer customer = customerDic[memberID];
                        List<Order> orderHistory = customer.orderHistory;
                        foreach (Order order in orderHistory)
                        {
                            if (order.Id == id)
                            {
                                order.IceCreamList.Add(newIceCream);
                            }
                        }
                        orderHistory.Add(newOrder);
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


            bool ProcessOption(int option)
            {
                switch (option)
                {
                    case 0:
                        Console.WriteLine("Thank you & Have a nice day! Bye bye");
                        return true;

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
                        Option4();
                        break;
                    case 5:
                        Option5();
                        break;
                    case 6:
                        Option6();
                        break;
                }
                return false;
            }

            while (true)
            {
                DisplayMenu();
                int option = OptionValidation();
                bool exit = ProcessOption(option);
                if (exit)
                {
                    break;
                }
            }

            void Option1()
            {
                // 1) List all customers - display the information of all the customers
                Console.WriteLine($"{"Name",-10} \t{"Member ID",-6} \t{"Date Of Birth",-10} \t{"Tier",-8} \t{"Points",-6} \t{"PunchCard",-9}");
                foreach (KeyValuePair<int, Customer> kvp in customerDic)
                {
                    Customer customer = kvp.Value;
                    Console.WriteLine($"{customer.name,-10} \t{customer.memberId,-6} \t\t{customer.dob.ToShortDateString(),-10} \t{customer.rewards.Tier, -8} \t{customer.rewards.Points,-6} \t{customer.rewards.PunchCard,-9}");
                }
                Console.WriteLine();
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
                Console.WriteLine();
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
                string name = "";
                int id = 0;
                DateTime dob;

                while (true)
                {
                    try
                    {
                        Console.Write("Enter Customer Name: ");
                        name = Console.ReadLine();

                        if (name == "")
                        {
                            Console.WriteLine("Name cannot be blank. Please enter a valid name.\n");
                        }
                        else if (!Regex.IsMatch(name, "^[a-zA-Z]+$"))
                        {
                            Console.WriteLine("Name cannot include numbers or special characters. Pleae enter a valid name.\n");
                        }
                        else { break; }
                    }
                    catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

                while (true)
                {
                    try
                    {
                        Console.Write("Enter Customer ID: ");
                        string input = Console.ReadLine();
                        if (input == "")
                        {
                            Console.WriteLine("CustomerID cannot be empty. Please try again.\n");
                        }
                        else
                        {
                            id = Int32.Parse(input);
                            if (!Regex.IsMatch(input, @"^\d{6}$"))
                            {
                                Console.WriteLine("MemberID should be 6 numbers long.\n");
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine("CustomerID cannot be include special characters or letters. Please try again.\n");
                    }
                    catch (Exception ex) { Console.WriteLine(ex.Message); }
                }

                while (true)
                {
                    try
                    {
                        Console.Write("Enter Customer Date Of Birth (dd/MM/yyyy): ");
                        dob = DateTime.ParseExact(Console.ReadLine(), "d/M/yyyy", null);
                        break;
                    }
                    catch (FormatException)
                    {
                        // Input is not in the correct format
                        Console.WriteLine("Invalid date format. Please enter the date in dd/MM/yyyy format.\n");
                    }
                    catch (Exception ex)
                    {
                        // Other unexpected errors
                        Console.WriteLine($"An error occurred: {ex.Message}\n");
                    }
                }
                Customer newCustomer = new Customer(name, id, dob);
                customerDic.Add(newCustomer.memberId, newCustomer);

                string data = $"{name},{id},{dob.ToShortDateString()},{newCustomer.rewards.Tier},{newCustomer.rewards.Points},{newCustomer.rewards.PunchCard}";

                using (StreamWriter sw = new StreamWriter("customers.csv", true))
                {
                    sw.WriteLine(data);
                }
                Console.WriteLine("Registration Status: Successful!\n"); // Might need to shift it in the error handling part instead
            }

            void Option4()
            {
                // list the customers from the customers.csv
                Console.WriteLine($"{"Name",-10} \t{"Member ID",-6} \t{"Date Of Birth",-10} \t{"Tier",-8} \t{"Points",-6} \t{"PunchCard",-9}");
                foreach (KeyValuePair<int, Customer> kvp in customerDic)
                {
                    Customer customer = kvp.Value;
                    Console.WriteLine($"{customer.name,-10} \t{customer.memberId,-6} \t\t{customer.dob.ToShortDateString(),-10} \t{customer.rewards.Tier,-8} \t{customer.rewards.Points,-6} \t{customer.rewards.PunchCard,-9}");
                }
                Console.WriteLine();


                // prompt user to select a customer and retrieve the selected customer
                int memberID = 0;
                while (true)
                {
                    try
                    {
                        Console.Write("Select Customer (Enter MemberID): ");
                        string input = Console.ReadLine();
                        if (input == "")
                        {
                            Console.WriteLine("CustomerID cannot be empty. Please try again.\n");
                        }
                        else
                        {
                            memberID = Int32.Parse(input);
                            if (!Regex.IsMatch(input, @"^\d{6}$"))
                            {
                                Console.WriteLine("MemberID should be 6 numbers long.\n");
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    catch (FormatException ex)
                    {
                        Console.WriteLine("CustomerID cannot be include special characters or letters. Please try again.\n");
                    }
                    catch (Exception ex) { Console.WriteLine(ex.Message); }
                }

                Customer selectedCustomer = customerDic[memberID];

                Order newOrder = new Order();

                int orderID = orderDic.Count + 1;
                DateTime timeRecieved = DateTime.Now;
                DateTime? timefulfilled = null;

                
                AddIceCream(selectedCustomer);

                while (true)
                {
                    Console.Write("Would you like to add another ice cream to the order? (yes / no): ");
                    string response = Console.ReadLine().ToLower();
                    if (response == "yes")
                    {
                        AddIceCream(selectedCustomer);
                    }
                    else if (response == "no")
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Invalid Input. Please enter (yes / no).");
                    }
                }

                if (selectedCustomer.rewards.Tier == "Gold")
                {
                    GoldQueue.Enqueue(selectedCustomer.currentOrder);
                }
                else
                {
                    NormalQueue.Enqueue(selectedCustomer.currentOrder);
                }

                Console.WriteLine("Your order has been made successfully!\n");
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
                        foreach (IceCream icecream in selectedCustomer.currentOrder.IceCreamList)
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
                                selectedIceCreamIndex <= selectedCustomer.currentOrder.IceCreamList.Count)
                            {
                                break;
                            }
                            Console.WriteLine($"Invalid input. Please enter a number between 1 and {selectedCustomer.currentOrder.IceCreamList.Count}:");
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

            void AddIceCream(Customer selectedCustomer)
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
                    IceCream newOne = new Waffle(type, scoops, flavours, toppings, waffleFlavor);
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

                    IceCream newOne = new Cone(type, scoops, flavours, toppings, dipped);
                    selectedCustomer.currentOrder.AddIceCream(newOne);
                }
                else if (type == "cup")
                {
                    IceCream newOne = new Cup(type, scoops, flavours, toppings);
                    selectedCustomer.currentOrder.AddIceCream(newOne);
                }

                selectedCustomer.currentOrder.Id = orderDic.Count + 1;
                selectedCustomer.currentOrder.TimeRecieved = DateTime.Now;
                selectedCustomer.currentOrder.TimeFulfilled = null;
            }
        }
    }
}