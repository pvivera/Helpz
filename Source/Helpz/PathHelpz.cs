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
using System.Linq;
using System.Reflection;

namespace Helpz
{
    public static class PathHelpz
    {
        public static string GetCodeBase(Assembly assembly)
        {
            var codebase = assembly.GetName().CodeBase;
            var uri = new UriBuilder(codebase);
            var path = Path.GetFullPath(Uri.UnescapeDataString(uri.Path));
            return Path.GetDirectoryName(path);
        }

        public static string GetProjectRootPath(Assembly startFromAssembly, string fileOrDirectoryInRoot)
        {
            var codeBase = GetCodeBase(startFromAssembly);
            return GetProjectRootPath(codeBase, fileOrDirectoryInRoot);
        }

        public static string GetProjectRootPath(string startFromDirectory, string fileOrDirectoryInRoot)
        {
            if (!Directory.Exists(startFromDirectory))
            {
                throw new ArgumentException($"Could not find a directory here '{startFromDirectory}'");
            }

            if (Path.IsPathRooted(startFromDirectory))
            {
                startFromDirectory = Path.GetFullPath(startFromDirectory);
            }

            var directory = startFromDirectory;
            while (true)
            {
                var files = Directory.GetFiles(directory).Select(Path.GetFileName);
                var directories = Directory.GetDirectories(directory);

                if (files.Contains(fileOrDirectoryInRoot) || directories.Contains(fileOrDirectoryInRoot))
                {
                    return directory;
                }

                var directoryInfo = Directory.GetParent(directory);
                if (directoryInfo == null)
                {
                    throw new ArgumentException(string.Format(
                        "Could not find a parent directory containing file '{0}' starting from '{1}'",
                        fileOrDirectoryInRoot,
                        startFromDirectory));
                }

                directory = directoryInfo.FullName;
            }
        }
    }
}