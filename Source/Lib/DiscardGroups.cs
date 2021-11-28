using System;
using System.Collections.Generic;
using System.Text;

namespace Lib
{
    public class DiscardGroups
    {
        public class Context
        {
            public Map discardTags;
            public Store store;
            public Logger logger;
            public uint discarded = 0;
        }

        public void readTagList(Context context, object ptr, uint size)
        {
            var a = (string)ptr;
            var b = a + size;

            uint N = 0;
            while (true)
            {
                while (a < b && (a == '\n' || a == '\r')) a++;
                var c = a;
                while (a < b && (a != '\n' && a != '\r')) a++;

                if (c < a)
                {
                    var d = a - 1;
                    while (c < d && (d[-1] != '\t')) --d;

                    var str = context.store.strings.intern(d, a);
                    context.discardTags.insert(ulong(str), ulong(1 + N++));
                }
                else
                {
                    break;
                }
            }
            context.logger(0, "DiscardGroups: Read %d tags.", N);
        }


        public void pruneChildren(Context context, Group group)
        {

            ListHeader<Group> groupsNew = new ListHeader<Group>();
            groupsNew.clear();
            for (var child = group.groups.first; child != null;)
            {
                var next = child.next;
                if (context.discardTags.get(Convert.ToUInt64(child.group.name)))
                {
                    //context.logger(0, "Discarded %s", child.group.name);
                    context.discarded++;
                }
                else
                {
                    pruneChildren(context, child);
                    groupsNew.insert(child);
                }
                child = next;
            }
            group.groups = groupsNew;
        }


        public bool discardGroups(Store store, Logger logger, object ptr, uint size)
        {
            Context context = new Context();
            context.store = store;
            context.logger = logger;
            readTagList(context, ptr, size);

            for (var root = store.getFirstRoot(); root != null; root = root.next)
            {
                for (var model = root.groups.first; model != null; model = model.next)
                {
                    pruneChildren(context, model);
                }
            }
            context.logger(0, "DiscardGroups: Discarded %d groups.", context.discarded);

            return true;
        }
    }
}
