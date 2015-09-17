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

using System.Linq;
using System.Collections.Generic;
using Helpz.ValueObjects;
using Helpz.VisualStudioProjects;

namespace Helpz.Extensions
{
    public static class NuGetProjectExtensions
    {
        public static IEnumerable<KeyValuePair<string, IEnumerable<VisualStudioProject>>> SelectVisualStudioProjectsWithDifferentPackageVersions(
            this IEnumerable<VisualStudioProject> visualStudioProjects)
        {
            return (
                from vp in visualStudioProjects
                from np in vp.NuGetPackages.Values
                let a = new
                    {
                        Project = vp,
                        Package = np,
                    }
                group a by a.Package.Id into g
                where g.GroupBy(a => a.Package.Version).Count() > 1
                select new KeyValuePair<string, IEnumerable<VisualStudioProject>>(
                    g.Key,
                    g.Select(a => a.Project))
                );
        }
    }
}
