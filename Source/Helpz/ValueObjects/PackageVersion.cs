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
using System.Text.RegularExpressions;
using Helpz.Core;

namespace Helpz.ValueObjects
{
    public class PackageVersion : SingleValueObject<string>
    {
        private static readonly Regex VersionParser = new Regex(@"^(?<version>[0-9\.]+)(-(?<pre>.+)){0,1}$");

        public PackageVersion(string value) : base(value)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value));

            var match = VersionParser.Match(value);
            if (!match.Success)
            {
                throw new ArgumentException($"NuGet package version '{value}' is invalid");
            }

            Version = Version.Parse(match.Groups["version"].Value);
            Patch = match.Groups["pre"].Success
                ? match.Groups["pre"].Value
                : string.Empty;
        }

        public Version Version { get; }
        public string Patch { get; }
        public bool IsPrerelease => !string.IsNullOrEmpty(Patch);

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Version;
            yield return Patch;
        }
    }
}