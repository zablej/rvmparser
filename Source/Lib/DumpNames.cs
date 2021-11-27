using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Lib
{
    public class DumpNames : StoreVisitor
    {

        public override void init(Store store)
        {
            Debug.Assert(stack == null);
            Debug.Assert(stack_p == 0);
            stack = new char[store.groupCountAllocated()];
        };

        public override bool done()
        {
            Debug.Assert(stack_p == 0);
            Debug.Assert(stack != null);
            stack = null;

            return true;
        }

        public override void beginFile(Group group)
        {
            @out.Write("File:\n");
            @out.Write("    info:     \"%s\"\n", group.file.info);
            @out.Write("    note:     \"%s\"\n", group.file.note);
            @out.Write("    date:     \"%s\"\n", group.file.date);
            @out.Write("    user:     \"%s\"\n", group.file.user);
            @out.Write("    encoding: \"%s\"\n", group.file.encoding);
        }

        public override void endFile()
        {

        }

        public override void beginModel(Group group)
        {
            @out.Write("Model:\n");
            @out.Write("    project:  \"%s\"\n", group.model.project);
            @out.Write("    name:     \"%s\"\n", group.model.name);
        }

        public override void endModel()
        {

        }

        public override void beginGroup(Group group)
        {
            printGroupTail();
            printed = false;
            geometries = 0;
            facetgroups = 0;

            stack[stack_p++] = group.group.name;
            for (uint i = 0; i + 1 < stack_p; i++)
            {
                @out.Write("    ");
            }
            @out.Write("%s", group.group.name);

        }

        public override void EndGroup()
        {
            printGroupTail();
            Debug.Assert(stack_p != 0);
            stack_p--;
        }

        public override void geometry(Geometry geometry)
        {
            if (geometry.kind == Geometry.Kind.FacetGroup)
            {
                facetgroups++;
            }
            else
            {
                geometries++;
            }
        }

        public void setOutput(StreamWriter o)
        {
            @out = o;
        }

        private StreamWriter @out = new StreamWriter(Console.OpenStandardError());
        private char[] stack = null;
        private uint stack_p = 0;
        private uint facetgroups = 0;
        private uint geometries = 0;
        private bool printed = true;

        private void printGroupTail()
        {
            if (printed) return;
            printed = true;

            @out.Write("\n");

            if (geometries != 0)
            {
                for (uint i = 0; i < stack_p; i++)
                {
                    @out.Write("    ");
                }
                @out.Write(" pgeos=%d\n", geometries);
            }

            if (facetgroups != 0)
            {
                for (uint i = 0; i < stack_p; i++)
                {
                    @out.Write("    ");
                }
                @out.Write(" fgrps=%d\n", facetgroups);
            }
        }
    }
}
