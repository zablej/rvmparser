using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class StoreVisitor
    {
        public virtual void init(Store store) { }

        public virtual bool done() { return true; }

        public virtual void beginFile(Group group) { }

        public virtual void endFile() { }

        public virtual void beginModel(Group group) { }

        public virtual void endModel() { }

        public virtual void beginGroup(Group group) { }

        public virtual void doneGroupContents(Group group) { }

        public virtual void EndGroup() { }

        public virtual void beginChildren(Group container) { }

        public virtual void endChildren() { }

        public virtual void beginAttributes(Group container) { }

        public virtual void attribute(char? key, char? val) { }

        public virtual void endAttributes(Group container) { }

        public virtual void beginGeometries(Group container) { }

        public virtual void geometry(Geometry geometry) { }

        public virtual void endGeometries() { }
    }
}
