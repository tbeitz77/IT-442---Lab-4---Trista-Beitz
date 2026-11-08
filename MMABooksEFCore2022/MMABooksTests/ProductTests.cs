using Microsoft.EntityFrameworkCore;
using MMABooksEFClasses.MarisModels;
using MMABooksEFClasses.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;


namespace MMABooksTests
{
    [TestFixture]
    public class ProductTests
    {

        MMABooksContext dbContext;
        Products p;
        List<Products> products;

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
            // get a list of all of the products that have a unit price of 56.50
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
            p = dbContext.Products.Find("JAVP");
            dbContext.Products.Remove(p);
            dbContext.SaveChanges();
            Assert.IsNull(dbContext.States.Find("JAVP"));
        }

        [Test]
        public void CreateTest()
        {

        }

        [Test]
        public void UpdateTest()
        {

        }
       
    }
}