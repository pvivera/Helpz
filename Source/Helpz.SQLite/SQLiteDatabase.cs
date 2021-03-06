﻿// The MIT License (MIT)
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
using System.IO;

namespace Helpz.SQLite
{
    public class SQLiteDatabase : ISQLiteDatabase
    {
        public SQLiteDatabase(SQLiteConnectionString connectionString, bool dropOnDispose = true)
        {
            ConnectionString = connectionString;
            DropOnDispose = dropOnDispose;
            ConnectionString.Ping();
        }

        public void Dispose()
        {
            if (!DropOnDispose || !File.Exists(ConnectionString.DatabaseFilePath)) return;

            ConnectionString.Close();
            File.Delete(ConnectionString.DatabaseFilePath);
        }

        public void Execute(string sql)
        {
            ConnectionString.Execute(sql);
        }

        public void Ping()
        {
            ConnectionString.Ping();
        }

        public void WithConnection(Action<SQLiteConnection> action)
        {
            ConnectionString.WithConnection(action);
        }

        public T WithConnection<T>(Func<SQLiteConnection, T> action)
        {
            return ConnectionString.WithConnection(action);
        }

        public SQLiteConnectionString ConnectionString { get; }

        public bool DropOnDispose { get; }
    }
}