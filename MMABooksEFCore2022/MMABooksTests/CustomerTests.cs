using Microsoft.EntityFrameworkCore;

using MMABooksEFClasses.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MMABooksTests
{
    [TestFixture]
    public class CustomerTests
    {
        
        MMABooksContext dbContext;
        Customer? c;
        List<Customer>? customers;

        [SetUp]
        public void Setup()
        {
            dbContext = new MMABooksContext();
            dbContext.Database.ExecuteSqlRaw("call usp_testingResetData()");
        }

        [Test]
        public void GetAllTest()
        {
            customers = dbContext.Customers.OrderBy(c => c.Name).ToList();
           Assert.AreEqual(696, customers.Count);
           //Assert.AreEqual("Humble, Je", customers[275].Name);
            PrintAll(customers);
        }

        [Test]
        public void GetByPrimaryKeyTest()
        {
            c = dbContext.Customers.Find(20);
            Assert.IsNotNull(c);
            Assert.AreEqual("Chamberland, Sarah", c.Name);
            Console.WriteLine(c);
        }

        [Test]
        public void GetUsingWhere()
        {
            // get a list of all of the customers who live in OR
            customers = dbContext.Customers.Where(c => c.State == "OR").OrderBy(c => c.Name).ToList();
            Assert.AreEqual(5, customers.Count);
            PrintAll(customers);
        }

        [Test]
        public void GetWithInvoicesTest()
        {
            // get the customer whose id is 20 and all of the invoices for that customer
            c = dbContext.Customers.Include("Invoices").Where(c => c.CustomerId == 20).SingleOrDefault();
            Assert.IsNotNull(c);
            Assert.AreEqual("Chamberland, Sarah", c.Name);
            Assert.AreEqual(3, c.Invoices.Count);
            Console.WriteLine(c);
        }

        [Test]
        public void GetWithJoinTest()
        {
            // get a list of objects that include the customer id, name, statecode and statename
            var customers = dbContext.Customers.Join(
               dbContext.States,
               c => c.State,
               s => s.StateCode,
               (c, s) => new { c.CustomerId, c.Name, c.State, s.StateName }).OrderBy(r => r.StateName).ToList();
            Assert.AreEqual(696, customers.Count);
            // I wouldn't normally print here but this lets you see what each object looks like
            foreach (var c in customers)
            {
                Console.WriteLine(c);
            }
        }

        [Test]
        public void DeleteTest()
        {
            //Created a customer to delete. 
            c = new Customer();
            c.Name = "Beitz, Test";
            c.Address = "100 Main St";
            c.City = "Bend";
            c.State = "OR";
            c.ZipCode = "97701";
            dbContext.Customers.Add(c);
            dbContext.SaveChanges();
            int id = c.CustomerId;
            c = dbContext.Customers.Find(id);

            dbContext.Customers.Remove(c);
            dbContext.SaveChanges();
            Assert.IsNull(dbContext.Customers.Find(id));
        }

        [Test]
        public void CreateTest()
        {
            c = new Customer();
            c.Name = "Beitz, Test";
            c.Address = "100 Main St";
            c.City = "Bend";
            c.State = "OR";
            c.ZipCode = "97701";
            dbContext.Customers.Add(c);
            dbContext.SaveChanges();

            Customer created = dbContext.Customers.Where(x => x.Name == "Beitz, Test").SingleOrDefault();
            Assert.IsNotNull(created);
            Assert.AreEqual("Bend", created.City);
        }

        [Test]
        public void UpdateTest()
        {
            c = dbContext.Customers.Find(214);
            Assert.IsNotNull(c);
            c.City = "Baltimore";
            dbContext.Customers.Update(c);
            dbContext.SaveChanges();

            Customer updated = dbContext.Customers.Find(214);
            Assert.AreEqual("Baltimore", updated.City);
        }

        public void PrintAll(List<Customer> customers)
        {
            foreach (Customer c in customers)
            {
                Console.WriteLine(c.CustomerId + " | " + c.Name + " | " + c.City + " | " + c.State);
            }
        }
        
    }
}