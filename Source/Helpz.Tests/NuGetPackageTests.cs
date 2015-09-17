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
using System.Linq;
using FluentAssertions;
using Helpz.NuGetPackages;
using Helpz.ValueObjects;
using NUnit.Framework;

namespace Helpz.Tests
{
    public class NuGetPackageTests
    {
        [Test]
        public void NuGetPackageReportGeneration()
        {
            // Arrange
            var visualStudioProjects = new[]
            {
                new VisualStudioProject("-", "-", "OLD", new[]
                {
                    new NuGetPackage("package1", new PackageVersion("1.0")),
                    new NuGetPackage("package2", new PackageVersion("1.0"))
                }),
                new VisualStudioProject("-", "-", "NEW", new[]
                {
                    new NuGetPackage("package1", new PackageVersion("2.0")),
                    new NuGetPackage("package2", new PackageVersion("1.0"))
                })
            };

            // Act
            var nuGetPackageReports = visualStudioProjects
                .CreateNuGetPackageReports()
                .WhereDifferentVersionsExists()
                .ToList();
            nuGetPackageReports.Should().HaveCount(1);

            var nuGetPackageReport = nuGetPackageReports.Single();
            nuGetPackageReport.Id.Should().Be("package1");

            Console.WriteLine(nuGetPackageReports.PrettyPrint());
        }
    }
}