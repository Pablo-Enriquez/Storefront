using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Services;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace EnriquezStorefront
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Check if the filename is provided as a command line arguement
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: Enriquezstorefront <inventory_filename>");
                return;
            }

            string filename = args[0];
            Dictionary<string, double> inventory = ReadInventory(filename);

            if (inventory == null)
            {
                Console.WriteLine("Failed to read inventory from the file chosen.");
                return;
            }

            //This will display the inventory to the user
            Console.WriteLine("These are the available items for purchase:");
            foreach (var item in inventory)
            {
                Console.WriteLine($"{item.Key}: ${item.Value}");
            }
            Console.WriteLine();

            //This will initialize variabls to store the users purchases
            Dictionary<string, int> cart = new Dictionary<string, int>();
            double totalPrice = 0;

            //This allows the user to make purchases
            while (true)
            {
                Console.WriteLine("Enter the name of the item you would like to purchase (if finished type 'done' to finish):");
                string itemName = Console.ReadLine().Trim().ToLower();

                if (itemName == "done")
                    break;
                if (!inventory.ContainsKey(itemName))
                {
                    Console.WriteLine("Invalid item name. Please enter the item again.");
                    continue;
                }

                Console.WriteLine($"Enter the quantity of '{itemName}' you would like to purchase:");
                int quantity;
                if (!int.TryParse(Console.ReadLine(), out quantity) || quantity <= 0)
                {
                    Console.WriteLine("Invalid quantity. Please enter a positive ointeger.");
                    continue;
                }

                cart[itemName] = quantity;
                totalPrice += inventory[itemName] * quantity;
            }

            //Display the items purchased and the total price
            Console.WriteLine("\nItems Purchases:");
            foreach (var item in cart)
            {
                Console.WriteLine($"{item.Value} {item.Key}(s)");
            }
            Console.WriteLine($"The total price: ${totalPrice}");
        }

        static Dictionary<string, double> ReadInventory(string filename)
        {
            Dictionary<string, double> inventory = new Dictionary<string, double>();

            try
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] parts = line.Split(':');
                        if ((parts.Length == 2))
                        {
                            string itemName = parts[0].Trim().ToLower();
                            double price;
                            if (double.TryParse(parts[1], out price))
                            {
                                inventory[itemName] = price;
                            }
                        }
                    }
                }
                return inventory;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error has occured while reading the inventory: ");
                return null;
            }
        }
    }
}
