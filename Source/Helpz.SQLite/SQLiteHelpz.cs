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
using System.IO;

namespace Helpz.SQLite
{
    public static class SQLiteHelpz
    {
        public static string CreateTempConnectionString()
        {
            var tempFilePath = TempFileHelpz.MakeFilePath(SQLiteFileExt);
            return string.Format(ConnectionStringMask, tempFilePath);
        }

        public static SQLiteConnectionString CreateLabeledConnectionString(string label)
        {
            var basePath = Environment.GetEnvironmentVariable("HELPZ_SQLITE_DATABASE_PATH")
                ?? Path.GetTempPath();
            var filePath = Path.Combine(basePath, $"{DateTime.Now.ToFileTime()}-{label}{SQLiteFileExt}");
            var connectionString = string.Format(ConnectionStringMask, filePath);

            return new SQLiteConnectionString(connectionString);
        }
        public static ISQLiteDatabase CreateDatabase(string label, bool dropOnDispose = true)
        {
            var connectionString = CreateLabeledConnectionString(label);

            return new SQLiteDatabase(connectionString, dropOnDispose);
        }

        public static string SQLiteFileExt { get; } = ".sqlite";
        public static string ConnectionStringMask { get; } = "Data Source={0};Version=3;";
    }
}
