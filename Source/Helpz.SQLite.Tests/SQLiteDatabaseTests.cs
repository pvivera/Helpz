using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpz.SQLite.Tests
{
    public class SQLiteDatabaseTests
    {
        [Test]
        public void DisposesOfDatabaseFileIfRequiredTo()
        {
            var databaseFilePath = string.Empty;

            // Arrange
            using (var sut = new SQLiteDatabase(SQLiteHelpz.CreateLabeledConnectionString("testdb")))
            {
                // Act
                databaseFilePath = sut.ConnectionString.DatabaseFilePath;
                File.Exists(databaseFilePath)
                    .Should()
                    .BeTrue("the database file should be created by SQLiteDatabase");
            };

            // Aassert
            File.Exists(databaseFilePath)
                .Should()
                .BeFalse("the database file should be deleted");
        }                      
    }
}
