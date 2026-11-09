using System.Collections.Generic;
using System.Linq;
using System;

using NUnit.Framework;
using MMABooksEFClasses.Models;
using Microsoft.EntityFrameworkCore;

namespace MMABooksTests
{
    [TestFixture]
    public class ProductTests
    {
        MMABooksContext dbContext;
        Product p;
        List<Product> products;

        [SetUp]
        public void Setup()
        {
            dbContext = new MMABooksContext();
            dbContext.Database.ExecuteSqlRaw("call usp_testingResetProductData()");
        }

        [Test]
        public void GetAllTest()
        {
            products = dbContext.Products.OrderBy(p => p.ProductCode).ToList();
            Assert.AreEqual(16, products.Count);
            Assert.AreEqual("A4CS", products[0].ProductCode.Trim());
            PrintAll(products);
        }

        private void PrintAll(List<Product> products)
        {
            foreach (Product pr in products)
            {
                Console.WriteLine(pr.ProductCode.Trim() + " | " + pr.Description + " | " + pr.UnitPrice + " | " + pr.OnHandQuantity);
            }
        }

        [Test]
        public void GetByPrimaryKeyTest()
        {
            p = dbContext.Products.Find("CS10");
            Assert.IsNotNull(p);
            Assert.AreEqual("Murach's C# 2010", p.Description);
            Console.WriteLine(p);
        }

        [Test]
        public void GetUsingWhere()
        {
            // get a list of all of the products that have a unit price of 56.50 (or calculated values as written)
            var products = dbContext.Products.Select(
             p => new { p.ProductCode, p.UnitPrice, p.OnHandQuantity, Value = p.UnitPrice * p.OnHandQuantity }).
             OrderBy(p => p.ProductCode).ToList();
            Assert.AreEqual(16, products.Count);
            foreach (var p in products)
            {
                Console.WriteLine(p);
            }
        }

        [Test]
        public void GetWithCalculatedFieldTest()
        {
            // get a list of objects that include the productcode, unitprice, quantity and inventoryvalue
            var products = dbContext.Products.Select(
            p => new { p.ProductCode, p.UnitPrice, p.OnHandQuantity, Value = p.UnitPrice * p.OnHandQuantity }).
            OrderBy(p => p.ProductCode).ToList();
            Assert.AreEqual(16, products.Count);
            foreach (var p in products)
            {
                Console.WriteLine(p);
            }
        }

        [Test]
        public void DeleteTest()
        {
            //created product to delete
            p = new Product();
            p.ProductCode = "DEL1";
            p.Description = "Delete Me";
            p.UnitPrice = 1.00m;
            p.OnHandQuantity = 1;
            dbContext.Products.Add(p);
            dbContext.SaveChanges();
            Assert.IsNotNull(dbContext.Products.Find("DEL1"));

            dbContext.Products.Remove(p);
            dbContext.SaveChanges();
            Assert.IsNull(dbContext.Products.Find("DEL1"));
        }

        [Test]
        public void CreateTest()
        {
            p = new Product()
            {
                ProductCode = "TPD1",
                Description = "Test Product Description",
                UnitPrice = 49.99m,
                OnHandQuantity = 25
            };

            dbContext.Products.Add(p);
            dbContext.SaveChanges();

            // Verify creation
            var createdProduct = dbContext.Products.Find("TPD1");
            Assert.IsNotNull(createdProduct);
            Assert.AreEqual("Test Product Description", createdProduct.Description);
            
        }

        [Test]
        public void UpdateTest()
        {
            p = dbContext.Products.Find("CS10");
            Assert.IsNotNull(p);

            // Modify a property
            p.Description = "Updated Murach's C# 2010";
            dbContext.Products.Update(p);
            dbContext.SaveChanges();

            // Verify update
            var updatedProduct = dbContext.Products.Find("CS10");
            Assert.AreEqual("Updated Murach's C# 2010", updatedProduct.Description);
        }
    }
}