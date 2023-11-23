using NUnit.Framework;

using MMABooksBusiness;
using MMABooksProps;
using MMABooksDB;

using DBCommand = MySql.Data.MySqlClient.MySqlCommand;
using System.Data;

using System.Collections.Generic;
using System;
using MySql.Data.MySqlClient;

namespace MMABooksTests
{
    [TestFixture]
    internal class ProductTests
    {
        [SetUp]
        public void TestResetDatabase()
        {
            ProductDB db = new ProductDB();
            DBCommand command = new DBCommand();
            command.CommandText = "usp_testingResetStateData";
            command.CommandType = CommandType.StoredProcedure;
            db.RunNonQueryProcedure(command);
        }

        [Test]
        public void TestNewStateConstructor()
        {
            // not in Data Store - no code
            Product c = new Product();

            Assert.AreEqual(string.Empty, c.ProductCode);
            Assert.AreEqual(string.Empty, c.Description);
            Assert.IsTrue(c.IsNew);
            Assert.IsFalse(c.IsValid);
        }

        [Test]
        public void TestRetrieveFromDataStoreContructor()
        {
            // retrieves from Data Store
            Product c = new Product(833);
            Assert.AreEqual("A4CS", c.ProductCode);
            Assert.AreEqual("'Murach''s ASP.NET 4 Web Programming with C# 2010'", c.Description);
            Assert.IsFalse(c.IsNew);
            Assert.IsTrue(c.IsValid);
        }

        [Test]
        public void TestSaveToDataStore()
        {
            Product c = new Product();
            c.ProductCode = "TestCode";
            c.Description = "Words";
            c.UnitPrice = 100.00m;
            c.OnHandQuantity = 5;
            Product c2 = new Product(c.ProductID);
            Assert.AreEqual(c2.Description, c.Description);
            Assert.AreEqual(c2.ProductCode, c.ProductCode);
        }

        [Test]
        public void TestUpdate()
        {
            Product c = new Product(833);
            c.ProductCode = "Edited";
            c.Save();

            Product c2 = new Product(833);
            Assert.AreEqual(c2.Description, c.Description);
            Assert.AreEqual(c2.ProductCode, c.ProductCode);
        }

        [Test]
        public void TestDelete()
        {
            Product c = new Product(833);
            c.Delete();
            c.Save();
            Assert.Throws<Exception>(() => new Product(833));
        }

        [Test]
        public void TestGetList()
        {
            Product c = new Product();
            List<Product> products = (List<Product>)c.GetList();
            Assert.AreEqual(16, products.Count);
            Assert.AreEqual("A4CS", products[0].ProductCode);
            Assert.AreEqual("Murach''s ASP.NET 4 Web Programming with C# 2010", products[0].Description);
        }

        [Test]
        public void TestNoRequiredPropertiesNotSet()
        {
            // not in Data Store - abbreviation and name must be provided
            Product c = new Product();
            Assert.Throws<Exception>(() => c.Save());
        }

        [Test]
        public void TestSomeRequiredPropertiesNotSet()
        {
            // not in Data Store - abbreviation and name must be provided
            Product c = new Product();
            Assert.Throws<Exception>(() => c.Save());
            c.Description = "??";
            Assert.Throws<Exception>(() => c.Save());
        }

        [Test]
        public void TestInvalidPropertySet()
        {
            Product c = new Product();
            Assert.Throws<ArgumentOutOfRangeException>(() => c.ProductCode = "12345678901");
        }

        [Test]
        public void TestConcurrencyIssue()
        {
            Product c1 = new Product(833);
            Product c2 = new Product(833);

            c1.ProductCode = "First";
            c1.Save();

            c2.ProductCode = "Second";
            Assert.Throws<Exception>(() => c2.Save());
        }
    }
}
