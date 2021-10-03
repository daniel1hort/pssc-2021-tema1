﻿using LanguageExt;
using System;
using Tema1.Fake;
using System.Linq;
using static Tema1.Domain.AppDomain;
using static Tema1.Domain.CartState;
using Tema1.Extensions;
using Tema1.Domain;

namespace Tema1
{
    class Program
    {
        static void Main(string[] args)
        {
            var userResult = Authenticate(RequestCredentials());
            var (response, user, exit) = userResult.Case switch
            {
                SomeCase<User> a => ($"Welcome {a.Value.Name}", a.Value, false),
                NoneCase<User> _ => ("Invalid credentials", null, true)
            };
            Console.WriteLine(response);
            if (exit) return;

            var cart = (CreateCart(user) as EmptyCart).Cart;
            while (true)
            {
                var (product, quantity) = RequestItem();
                var result = AddItemToCart(cart, product, quantity);
                var message = string.Empty; // Mixed declarations and expressions in destruction is currently in Preview
                (cart, message) = result switch
                {
                    ValidCart a => (a.Cart, ""),
                    InvalidCart a => (a.Cart, a.Message + (Environment.NewLine * (StringMultiplication)2))
                };
                Displayitems(cart);

                Console.Write(message);
                if (RequestPayment())
                {
                    PayCart(cart);
                    Console.WriteLine($"Good day {user.Name}");
                    break;
                }
            }
        }

        public static Unit Displayitems(Cart cart)
        {
            Console.WriteLine("Your items:");
            Console.Write(cart.Items
                .OrderBy(a => a.Product.Id)
                .Select(a => $"\t{a.Product.Id} / {a.Product.Name} / {a.Product.Price} / {a.Quantity}" + Environment.NewLine)
                .Aggregate((a, b) => a + b));
            Console.Write("Total price: ");
            Console.WriteLine(cart.Items.Select(a => a.Product.Price * a.Quantity).Aggregate((a, b) => a + b));
            Console.WriteLine();
            return Unit.Default;
        }

        public static string RequestCredentials()
        {
            Console.Write("Please enter password: ");
            return Console.ReadLine();
        }

        public static (Option<Product>, int) RequestItem()
        {
            var products = FakeDB.LoadProducts();
            products.Iter(a => Console.WriteLine($"Id: {a.Id}, Price: {a.Price}, Name: {a.Name}"));
            Console.WriteLine("Please select an item by it's Id and then input the quantity");
            Console.Write("Product Id: ");
            var productId = Console.ReadLine().Trim();
            Console.Write("Quantity: ");
            var quantity = int.Parse(Console.ReadLine().Trim()); // n-am chef sa fac validare
            var product = products.FirstOrDefault(a => a.Id.ToString() == productId);
            return (product, quantity);
        }

        public static bool RequestPayment()
        {
            Console.WriteLine("Please insert \"Y\" if you want to pay or anything else to continue shopping:");
            return Console.ReadLine().Trim().ToLower() switch
            {
                "y" => true,
                _ => false
            };
        }
    }
}
