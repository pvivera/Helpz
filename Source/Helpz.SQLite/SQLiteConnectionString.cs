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
using System.Data.SQLite;
using System.Linq;
using Helpz.Core;
using System.Text.RegularExpressions;
using System.IO;

namespace Helpz.SQLite
{
    public class SQLiteConnectionString : SingleValueObject<string>
    {
        private static readonly Regex DatabaseExtract = new Regex(@"(\w+\.(sqlite|db))", RegexOptions.Compiled);
        private static readonly Regex DatabaseFilenameExtract = new Regex(@"(?:=)(?<filename>.+\.sqlite|db)", RegexOptions.Compiled);

        public SQLiteConnectionString(string value)
            : base(value)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value));

            var matchName = DatabaseExtract.Match(value);
            if (!matchName.Success)
            {
                throw new ArgumentException($"Cannot find database name in '{value}'");
            }
            DatabaseName = matchName.Value;

            var matchPath = DatabaseFilenameExtract.Match(Value);
            if (!matchPath.Success)
            {
                throw new ArgumentException($"Cannot find database file path in '{Value}'");
            }
            DatabaseFilePath = matchPath.Groups["filename"].Value;

            var directory = Path.GetDirectoryName(DatabaseFilePath);
            if (!Directory.Exists(directory))
            {
                throw new ArgumentException($"File path {directory} does not exist");
            }

            _sqliteConnection = new SQLiteConnection(value);
            _sqliteConnection.Open();
        }

        internal void Close()
        {
            _sqliteConnection.Close();
        }

        public void Execute(string sql)
        {
            Console.WriteLine("Executing SQL: {0}", sql);

            WithConnection(c =>
            {
                using (var sqlCommand = new SQLiteCommand(sql, c))
                {
                    sqlCommand.ExecuteNonQuery();
                }
            });
        }

        public void Ping()
        {
            Execute("SELECT 1");
        }

        public void WithConnection(Action<SQLiteConnection> action)
        {
            WithConnection(c =>
            {
                action(c);
                return 0;
            });
        }

        public T WithConnection<T>(Func<SQLiteConnection, T> action)
        {
            using (var sqliteConnection = new SQLiteConnection(Value))
            {
                sqliteConnection.Open();
                return action(sqliteConnection);
            }
        }

        public string DatabaseFilePath { get; }

        public string DatabaseName { get; }

        private SQLiteConnection _sqliteConnection;
    }

}
