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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using Helpz.Core;

namespace Helpz.Applications
{
    public static class ConfigHelpz
    {
        private static readonly string[] ConfigurationXPaths =
            {
                "/configuration/connectionStrings/add[@name='{0}']/@connectionString",
                "/configuration/appSettings/add[@key='{0}']/@value"
            };

        public static FileBackup Update(
            string filePath,
            IEnumerable<KeyValuePair<string, string>> keyValuePairs)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(filePath);

            var notFound = new List<string>();
            foreach (var keyValuePair in keyValuePairs)
            {
                var found = false;
                foreach (var xPath in ConfigurationXPaths)
                {
                    var xmlNode = xmlDocument.SelectSingleNode(string.Format(xPath, keyValuePair.Key));
                    if (xmlNode == null)
                    {
                        continue;
                    }

                    xmlNode.Value = keyValuePair.Value;
                    found = true;
                    break;
                }

                if (!found)
                {
                    notFound.Add(keyValuePair.Key);
                }
            }

            if (notFound.Any())
            {
                throw new ArgumentException(string.Format(
                    "Could not find the configurations '{0}' in file '{1}'",
                    string.Join(", ", notFound),
                    filePath));
            }

            var fileBackup = FileBackup.Create(filePath);

            for (var i = 0;; i++)
            {
                try
                {
                    xmlDocument.Save(filePath);
                    break;
                }
                catch (IOException)
                {
                    if (i >= 5) throw;
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                }
            }

            Console.WriteLine(
                "ConfigurationHelper: Updated configuration values{0}  - {1}",
                Environment.NewLine,
                string.Join(
                    $"{Environment.NewLine}  - ",
                    keyValuePairs.Select(kv => $"{kv.Key}: {kv.Value}")));

            return fileBackup;
        }
    }
}