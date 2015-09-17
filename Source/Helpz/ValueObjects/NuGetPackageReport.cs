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
using System.Linq;
using System.Text;

namespace Helpz.ValueObjects
{
    public class NuGetPackageReport
    {
        public string Id { get; }
        public IReadOnlyCollection<PackageVersion> Versions { get; }
        public IReadOnlyCollection<VisualStudioProject> VisualStudioProjects { get; }

        public NuGetPackageReport(
            string id,
            IEnumerable<VisualStudioProject> visualStudioProjects)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentException(nameof(id));
            if (visualStudioProjects == null) throw new ArgumentNullException(nameof(visualStudioProjects));

            Id = id;
            VisualStudioProjects = visualStudioProjects
                .OrderBy(p => p.Name)
                .ToList();
            Versions = VisualStudioProjects
                .Select(p => p.GetPackageVersion(id))
                .Distinct()
                .OrderByDescending(v => v.Version)
                .ToList();
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(Id);
            foreach (var visualStudioProject in VisualStudioProjects)
            {
                var packageVersion = visualStudioProject.GetPackageVersion(Id);
                stringBuilder.AppendLine($"{packageVersion} - {visualStudioProject.Name}");
            }
            return stringBuilder.ToString();
        }
    }
}
