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
using System.Reflection;
using System.Xml.Linq;
using Helpz.ValueObjects;

namespace Helpz.VisualStudioProjects
{
    public static class VisualStudioProjectHelpz
    {
        public static IEnumerable<VisualStudioProject> FindVisualStudioProjects(
            Assembly startFromAssembly,
            string fileOrDirectoryInRoot)
        {
            var rootFolder = PathHelpz.GetProjectRootPath(startFromAssembly, fileOrDirectoryInRoot);
            return FindVisualStudioProjects(rootFolder);
        }

        public static IEnumerable<VisualStudioProject> FindVisualStudioProjects(string rootFolder)
        {
            return FindVisualStudioProjectFiles(rootFolder)
                .Select(LoadVisualStudioProjectFile);
        }

        public static VisualStudioProject LoadVisualStudioProjectFile(string projectFilePath)
        {
            if (!File.Exists(projectFilePath))
            {
                throw new ArgumentException($"Could not find file '{projectFilePath}'");
            }

            var directoryPath = Path.GetDirectoryName(projectFilePath);
            if (directoryPath == null)
            {
                throw new ArgumentException($"Could not get directory for '{projectFilePath}'");
            }

            var packageFilePath = Path.Combine(directoryPath, "packages.config");
            var nuGetPackages = File.Exists(packageFilePath)
                ? LoadPackagesFile(packageFilePath)
                : Enumerable.Empty<NuGetPackage>();

            var projectName = Path.GetFileNameWithoutExtension(projectFilePath);

            return new VisualStudioProject(
                directoryPath,
                projectFilePath,
                projectName,
                nuGetPackages);
        }

        public static IEnumerable<NuGetPackage> LoadPackagesFile(string packageFilePath)
        {
            if (!File.Exists(packageFilePath))
            {
                throw new ArgumentException($"Location '{packageFilePath}' does not exists");
            }

            var xDocument = XDocument.Load(packageFilePath);
            if (xDocument?.Root == null)
            {
                throw new ArgumentException($"Packages file '{packageFilePath}' seems invalid");
            }

            return xDocument
                .Root
                .Descendants("package")
                .Select(n => new NuGetPackage(
                    n.Attribute("id").Value,
                    new PackageVersion(n.Attribute("version").Value)));
        }

        private static IEnumerable<string> FindVisualStudioProjectFiles(string rootFolder)
        {
            try
            {
                var visualStudioProjects = Directory.GetFiles(rootFolder)
                    .Where(f => f.EndsWith(".csproj"));
                var combined = Directory.GetDirectories(rootFolder)
                    .SelectMany(FindVisualStudioProjectFiles)
                    .Concat(visualStudioProjects);
                return combined;
            }
            catch (PathTooLongException)
            {
                return Enumerable.Empty<string>();
            }
        }
    }
}