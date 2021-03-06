using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Lib
{
    public class ChunkTiny : StoreVisitor
    {
        public ChunkTiny(Flatten flatten, uint vertexThreshold)
        {
            this.flatten = new Flatten(flatten);
            this.vertexThreshold = vertexThreshold;
        }

        public override void init(Store store)
        {
            stack = new StackItem[store.groupCountAllocated()];
            stack_p = 0;
        }

        public override void geometry(Geometry geometry)
        {
            Debug.Assert(stack_p != 0);
            var item = stack[stack_p - 1];

            if (geometry.kind == Geometry.Kind.Line)
            {
                item.vertices += 2;
            }
            else
            {
                Debug.Assert(geometry.triangulation is null);
                item.vertices += geometry.triangulation.vertices_n;
            }
        }

        public override void beginGroup(Group group)
        {
            var item = stack[stack_p++];
            item.group = group;
            item.vertices = 0;
            item.keep = false;
        }

        public override void EndGroup()
        {
            Debug.Assert(stack_p != 0);

            var item = stack[stack_p - 1];
            if (stack_p == 1 || (stack[stack_p - 2].keep) || vertexThreshold < item.vertices)
            {
                flatten.keepTag(item.group.group.name);
                for (uint i = 0; i < stack_p; i++)
                {
                    stack[i].keep = true;
                }
            }
            else if (1 < stack_p)
            {
                stack[stack_p - 2].vertices += item.vertices;
            }

            stack_p--;
        }

        protected Flatten flatten;
        protected Arena arena;
        protected class StackItem
        {
            public uint vertices;
            public Group group;
            public bool keep;
        };
        protected StackItem[] stack = null;
        protected uint stack_p = 0;

        protected uint vertexThreshold;
    }
}
