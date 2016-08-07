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
using Helpz.Core;

namespace Helpz.ValueObjects
{
    public class VisualStudioProject : ValueObject
    {
        public VisualStudioProject(
            string directoryPath,
            string filePath,
            string name)
        {
            if (string.IsNullOrEmpty(directoryPath)) throw new ArgumentNullException(nameof(directoryPath));
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException(nameof(filePath));
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            DirectoryPath = directoryPath;
            FilePath = filePath;
            Name = name;
        }

        public string DirectoryPath { get; }
        public string FilePath { get; }
        public string Name { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return DirectoryPath;
            yield return FilePath;
            yield return Name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}