using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Helpz.SQLite.Tests
{
    public class SQLiteHelpzTests
    {
        [Test]
        public void CreatesValidConnectionString()
        {
            // Arrange
            var sut = SQLiteHelpz.CreateLabeledConnectionString("testdb");

            // Act
            // Assert
            sut.DatabaseFilePath.Should().EndWith("-testdb.sqlite");
            Directory.Exists(Path.GetDirectoryName(sut.DatabaseFilePath))
                .Should()
                .BeTrue();
        }

        [Test]
        public void CreatesWorkingDatabases()
        {
            // Arrange
            var sut = SQLiteHelpz.CreateDatabase("test");

            // Act
            Action act = () => sut.Execute("CREATE TABLE [test] ([Id] [INTEGER] PRIMARY KEY ASC)");

            // Assert
            act.ShouldNotThrow<Exception>();
        }
    }
}
