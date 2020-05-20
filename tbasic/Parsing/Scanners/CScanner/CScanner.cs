// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tbasic.Parsing
{
    internal class CScanner : DefaultScanner
    {
        public override bool NextVariable(out IEnumerable<char> name, out IList<IEnumerable<char>> indices)
        {
            if (NextValidIdentifier(out name)) {
                NextIndices(out indices);
                return true;
            }
            else {
                name = null; indices = null;
                return false;
            }
        }

        public override bool NextIndices(out IList<IEnumerable<char>> indices)
        {
            var lazy_indices = new Lazy<List<IEnumerable<char>>>();
            int start = FindNonWhiteSpace();

            while (CharAt(start) == '[') {
                SkipGroup();
                lazy_indices.Value.Add(StringSegment.Create(InternalBuffer, start, Position - start));
                start = FindNonWhiteSpace();
            }

            indices = lazy_indices.IsValueCreated ? lazy_indices.Value : null;
            return (indices != null);
        }
    }
}
