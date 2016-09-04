// The MIT License (MIT)
//
// Copyright (c) 2016 Rasmus Mikkelsen
// https://github.com/rasmus/Helpz
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using FluentAssertions;
using NUnit.Framework;
using System.IO;

namespace Helpz.SQLite.Tests
{
    public class SQLiteConnectionStringTests
    {
        [Test]
        public void DatabaseFilePathIsExtracted()
        {
            // Arrange
            var tempFilePath = TempFileHelpz.MakeFilePath(SQLiteHelpz.SQLiteFileExt);
            var connectionString = string.Format(SQLiteHelpz.ConnectionStringMask, tempFilePath);
            var sut = new SQLiteConnectionString(connectionString);

            // Act
            var result = sut.DatabaseFilePath;

            // Assert
            result.Should().Be(tempFilePath);
        }

        [Test]
        public void RaisesExceptionForIncorrectDatabaseFileExt()
        {
            // Arrange
            var tempFilePath = TempFileHelpz.MakeFilePath(".bleep");
            var connectionStringWithInvalidFileExt = string.Format(SQLiteHelpz.ConnectionStringMask, tempFilePath);

            // Act
            Action act = () => new SQLiteConnectionString(connectionStringWithInvalidFileExt);

            // Assert
            act.ShouldThrow<ArgumentException>().WithMessage("*database name*") ;
        }

        [Test]
        public void RaisesExceptionForMalformedConnectionString()
        {
            // Arrange
            var tempFilePath = TempFileHelpz.MakeFilePath(SQLiteHelpz.SQLiteFileExt);
            var malformedConnectionString = $"Data Source{tempFilePath};Version=3;";

            // Act
            Action act = () => new SQLiteConnectionString(malformedConnectionString);

            // Assert
            act.ShouldThrow<ArgumentException>().WithMessage("*database file path*");
        }

        [Test]
        public void RaisesExceptionForIncorrectDatabaseFilePath()
        {
            // Arrange
            var invalidFilePath = "xyz/sample.sqlite";
            var connectionStringWithInvalidFilePath = string.Format(SQLiteHelpz.ConnectionStringMask, invalidFilePath);

            // Act
            Action act = () => new SQLiteConnectionString(connectionStringWithInvalidFilePath);

            // Assert
            act.ShouldThrow<ArgumentException>().WithMessage("*path * does not exist*");
        }

        [Test]
        public void DatabaseNameIsExtracted()
        {
            // Arrange
            var tempFilePath = TempFileHelpz.MakeFilePath(SQLiteHelpz.SQLiteFileExt);
            var connectionString = string.Format(SQLiteHelpz.ConnectionStringMask, tempFilePath);
            var sut = new SQLiteConnectionString(connectionString);

            // Act
            var result = sut.DatabaseName;

            // Assert
            result.Should().Be(Path.GetFileName(tempFilePath));
        }

    }
}
