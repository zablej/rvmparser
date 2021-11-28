using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Lib
{
    public class Flatten
    {
        public Flatten(Store srcStore)
        {
            this.srcStore = srcStore;
            populateSrcTags();
        }

        // newline seperated bufffer of tags to keep
        public void setKeep(void* ptr, size_t size)
        {
            var a = (char)ptr;
            var b = a + size;

            while (true)
            {
                while (a < b && (a == '\n' || a == '\r')) a++;
                var c = a;
                while (a < b && (a != '\n' && a != '\r')) a++;

                if (c < a)
                {
                    var d = a - 1;
                    while (c < d && (d[-1] != '\t')) --d;

                    var str = srcStore.strings.intern(d, a);
                    ulong val;
                    if (srcTags.get(val, (ulong)str))
                    {
                        tags.insert((ulong)str, (ulong)currentIndex);
                        activeTags++;
                    }
                    currentIndex++;
                }
                else
                {
                    break;
                }
            }
        }


        // insert a single tag into keep set
        public void keepTag(char tag)
        {
            var str = srcStore.strings.intern(tag);
            ulong val = default;
            if (srcTags.get(val, (ulong)str))
            {
                tags.insert((ulong)str, (ulong)currentIndex);
                ((Group)val).group.id = (int)currentIndex;
                activeTags++;
            }
            currentIndex++;
        }

        // Tags submitted as selected, may be way more than actual number of tags. But consistent between different stores.
        public uint selectedTagsCount() { return currentIndex; }

        public uint activeTagsCount() { return activeTags; }

        public Store run()
        {
            dstStore = new Store();

            // populateSrcTags was run by the constructor, and setKeep and keepTags has changed some group.index from ~0u.
            // set group.index of parents of selected nodes to ~1u so we can retain them in the culling pass.
            for (var srcRoot = srcStore.getFirstRoot(); srcRoot != null; srcRoot = srcRoot.next)
            {
                Debug.Assert(srcRoot.kind == Group.Kind.File);
                for (var srcModel = srcRoot.groups.first; srcModel != null; srcModel = srcModel.next)
                {
                    Debug.Assert(srcModel.kind == Group.Kind.Model);
                    for (var srcGroup = srcModel.groups.first; srcGroup != null; srcGroup = srcGroup.next)
                    {
                        anyChildrenSelectedAndTagRecurse(srcGroup);
                    }
                }
            }

            // Create a fresh copy
            for (var srcRoot = srcStore.getFirstRoot(); srcRoot != null; srcRoot = srcRoot.next)
            {

                Debug.Assert(srcRoot.kind == Group.Kind.File);
                var dstRoot = dstStore.cloneGroup(null, srcRoot);

                for (var srcModel = srcRoot.groups.first; srcModel != null; srcModel = srcModel.next)
                {

                    Debug.Assert(srcModel.kind == Group.Kind.Model);
                    var dstModel = dstStore.cloneGroup(dstRoot, srcModel);

                    for (var srcGroup = srcModel.groups.first; srcGroup != null; srcGroup = srcGroup.next)
                    {
                        buildPrunedCopyRecurse(dstModel, srcGroup, 0);
                    }
                }
            }

            dstStore.updateCounts();
            var rv = dstStore;
            dstStore = null;
            return rv;
        }

        private Map srcTags;  // All tags in source store
        private Map tags;

        private uint pass = 0;

        private uint currentIndex = 0;
        private uint activeTags = 0;

        private Store srcStore = null;
        private Store dstStore = null;

        private Group[] stack = null;
        private uint stack_p = 0;
        private uint ignore_n = 0;

        private void populateSrcTagsRecurse(Group srcGroup)
        {
            srcGroup.group.id = -1;
            srcTags.insert((ulong)srcGroup.group.name, (ulong)srcGroup);
            for (var srcChild = srcGroup.groups.first; srcChild != null; srcChild = srcChild.next)
            {
                populateSrcTagsRecurse(srcChild);
            }
        }

        private void populateSrcTags()
        {
            // Sets all group.index to ~0u, and records the tag-names in srcTags. Nothing is selected yet.
            // name of groups are already interned in srcGroup
            for (var srcRoot = srcStore.getFirstRoot(); srcRoot != null; srcRoot = srcRoot.next)
            {
                for (var srcModel = srcRoot.groups.first; srcModel != null; srcModel = srcModel.next)
                {
                    for (var srcGroup = srcModel.groups.first; srcGroup != null; srcGroup = srcGroup.next)
                    {
                        populateSrcTagsRecurse(srcGroup);
                    }
                }
            }
        }

        private bool anyChildrenSelectedAndTagRecurse(Group srcGroup, int id = -1)
        {
            ulong val = default;
            if (tags.get(val, (ulong)srcGroup.group.name))
            {
                srcGroup.group.id = (int)val;
            }
            else
            {
                srcGroup.group.id = id;
            }

            bool anyChildrenSelected = false;
            for (var srcChild = srcGroup.groups.first; srcChild != null; srcChild = srcChild.next)
            {
                anyChildrenSelected = anyChildrenSelectedAndTagRecurse(srcChild, srcGroup.group.id) || anyChildrenSelected;
            }

            return srcGroup.group.id != -1;
        }

        private void buildPrunedCopyRecurse(Group dstParent, Group srcGroup, uint level)
        {
            Debug.Assert(srcGroup.kind == Group.Kind.Group);

            // Only groups can contain geometry, so we must make sure that we have at least one group even when none is selected.
            // Also, some subsequent stages require that we do not have geometry in the first level of groups.
            if (srcGroup.group.id == -1 && level < 2)
            {
                srcGroup.group.id = -2;
            }

            if (srcGroup.group.id != -1)
            {
                dstParent = dstStore.cloneGroup(dstParent, srcGroup);
                dstParent.group.id = srcGroup.group.id;
            }

            for (var srcGeo = srcGroup.group.geometries.first; srcGeo != null; srcGeo = srcGeo.next)
            {
                dstStore.cloneGeometry(dstParent, srcGeo);
            }

            for (var srcChild = srcGroup.groups.first; srcChild != null; srcChild = srcChild.next)
            {
                buildPrunedCopyRecurse(dstParent, srcChild, level + 1);
            }
        }
    }
}
