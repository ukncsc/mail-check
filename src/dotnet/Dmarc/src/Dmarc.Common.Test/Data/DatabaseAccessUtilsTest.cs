using System;
using Dmarc.Common.Data;
using NUnit.Framework;

namespace Dmarc.Common.Test.Data
{
    public class DatabaseAccessUtilsTest
    {
        [Test]
        public void GetDatabaseUsername()
        {
            string connectionString = "Server = dev-cluster.cluster-cn7nidgb1mh9.eu-west-1.rds.amazonaws.com; Port = 3306; Database = dmarc; Uid = dev_dnsproc; Connection Timeout=5;"; 

            string username = DatabaseAccessUtils.GetDatabaseUsername(connectionString);
            Assert.That(username, Is.EqualTo("dev_dnsproc"));
        }

        [Test]
        public void GetDatabaseUsernameMalformedString()
        {
            string connectionString = "fd;lgfdkglfd;gfdkg;lfdg";
            Assert.Throws<Exception>(() => DatabaseAccessUtils.GetDatabaseUsername(connectionString));
        }
    }

}
