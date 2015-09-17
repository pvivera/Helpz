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
using FluentAssertions;
using Helpz.ValueObjects;
using NUnit.Framework;

namespace Helpz.Tests.ValueObjects
{
    public class PackageVersionTests
    {
        [TestCase("1.0", 1, 0, "", false)]
        [TestCase("0.1", 0, 1, "", false)]
        [TestCase("1.0-alpha", 1, 0, "alpha", true)]
        [TestCase("0.42-build.23", 0, 42, "build.23", true)]
        public void ValidVersions(string version, int expectedMajor, int expectedMinor, string expectedPatch, bool expectedIsPrerelease)
        {
            // Act
            var expectedVersion = new Version(expectedMajor, expectedMinor);
            var packageVersion = new PackageVersion(version);

            // Assert
            packageVersion.Version.Should().Be(expectedVersion);
            packageVersion.Patch.Should().Be(expectedPatch);
            packageVersion.IsPrerelease.Should().Be(expectedIsPrerelease);
        }
    }
}
