// The MIT License (MIT)
//
// Copyright (c) 2015 Rasmus Mikkelsen
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
using System.Threading;

namespace Helpz.Core
{
    public class FileBackup
    {
        public string FilePath { get; }
        public byte[] Content { get; }

        public FileBackup(
            string filePath,
            byte[] content)
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException(nameof(filePath));

            FilePath = filePath;
            Content = content;
        }

        public static FileBackup Create(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new ArgumentException($"File '{filePath}' does not exist!");
            }

            if (!Path.IsPathRooted(filePath))
            {
                filePath = Path.GetFullPath(filePath);
            }

            Console.WriteLine(@"Backing up file '{0}'", filePath);
            var content = File.ReadAllBytes(filePath);

            return new FileBackup(
                filePath,
                content);
        }

        public void Restore()
        {
            Console.WriteLine(@"Restoring file backup '{0}'", FilePath);
           
            for (var i = 0;; i++)
            {
                try
                {
                    File.WriteAllBytes(FilePath, Content);
                    break;
                }
                catch (IOException)
                {
                    if (i >= 5) throw;
                    Thread.Sleep(TimeSpan.FromSeconds(0.2));
                }
            }
        }

        public override string ToString()
        {
            return FilePath;
        }
    }
}