using System.Collections.Generic;
using System.Diagnostics;

namespace RvmParser.Storing
{
    public class Store
    {
        #region Public Fields

        public Arena Arena { get; set;} = new Arena();

        public Arena ArenaTriangulation { get; set; } = new Arena();

        public Connectivity Conn { get; set; }

        public Stats Stats { get; set; }

        public StringInterning Strings { get; set;} = new StringInterning();

        #endregion Public Fields

        #region Private Fields

        private readonly List<Connection> connections = new List<Connection>();

        private readonly List<DebugLine> debugLines = new List<DebugLine>();

        private readonly List<Group> roots = new List<Group>();

        private string errorStr = null;

        private uint numEmptyLeaves;

        private uint numGeometries;

        private uint numGeometriesAllocated;

        private uint numGroups;

        private uint numGroupsAllocated;

        private uint numLeaves;

        private uint numNonEmptyNonLeaves;

        #endregion Private Fields

        #region Public Constructors

        public Store()
        {
            SetErrorString("");
        }

        #endregion Public Constructors

        #region Public Methods

        public void AddDebugLine(float[] a, float[] b, uint color)
        {
            var line = new DebugLine();
            for (uint k = 0; k < 3; k++)
            {
                line.A[k] = a[k];
                line.B[k] = b[k];
            }
            line.Color = color;
            GlobalMembers.Insert(debugLines, line);
        }

        public void Apply(StoreVisitor visitor)
        {
            visitor.Init(this);
            do
            {
                for (var file = roots.First; file != null; file = file.Next)
                {
                    Debug.Assert(file.kind == Group.Kind.File);
                    visitor.BeginFile(file);

                    for (var model = file.Groups.First; model != null; model = model.Next)
                    {
                        Debug.Assert(model.kind == Group.Kind.Model);
                        visitor.BeginModel(model);

                        for (var group = model.Groups.First; group != null; group = group.Next)
                        {
                            Apply(visitor, group);
                        }
                        visitor.EndModel();
                    }

                    visitor.endFile();
                }
            } while (!visitor.Done());
        }

        public Geometry CloneGeometry(Group parent, Geometry src)
        {
            var dst = newGeometry(parent);
            dst.Kind = src.Kind;
            //C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
            //ORIGINAL LINE: dst->M_3x4 = src->M_3x4;
            dst.M_3x4.CopyFrom(src.M_3x4);
            //C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
            //ORIGINAL LINE: dst->bboxLocal = src->bboxLocal;
            dst.BBoxLocal.CopyFrom(src.BBoxLocal);
            //C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
            //ORIGINAL LINE: dst->bboxWorld = src->bboxWorld;
            dst.BBoxWorld.CopyFrom(src.BBoxWorld);
            dst.ID = src.ID;
            dst.SampleStartAngle = src.SampleStartAngle;
            switch (dst.Kindd)
            {
                case Geometry.Kind.Pyramid:
                case Geometry.Kind.Box:
                case Geometry.Kind.RectangularTorus:
                case Geometry.Kind.CircularTorus:
                case Geometry.Kind.EllipticalDish:
                case Geometry.Kind.SphericalDish:
                case Geometry.Kind.Snout:
                case Geometry.Kind.Cylinder:
                case Geometry.Kind.Sphere:
                case Geometry.Kind.Line:
                    //C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
                    memcpy(dst.snout, src.snout, sizeof(src.snout));
                    break;

                case Geometry.Kind.FacetGroup:
                    dst.facetGroup.polygons_n = src.facetGroup.polygons_n;
                    dst.facetGroup.polygons = (Polygon)Arena.alloc(sizeof(Polygon) * dst.facetGroup.polygons_n);
                    for (uint k = 0; k < dst.facetGroup.polygons_n; k++)
                    {
                        var dst_poly = dst.facetGroup.polygons[k];
                        var src_poly = src.facetGroup.polygons[k];
                        dst_poly.contours_n = src_poly.contours_n;
                        dst_poly.contours = (Contour)Arena.alloc(sizeof(Contour) * dst_poly.contours_n);
                        for (uint i = 0; i < dst_poly.contours_n; i++)
                        {
                            var dst_cont = dst_poly.contours[i];
                            var src_cont = src_poly.contours[i];
                            dst_cont.vertices_n = src_cont.vertices_n;
                            dst_cont.vertices = (float)Arena.dup(src_cont.vertices, 3 * sizeof(float) * dst_cont.vertices_n);
                            dst_cont.normals = (float)Arena.dup(src_cont.normals, 3 * sizeof(float) * dst_cont.vertices_n);
                        }
                    }
                    break;

                default:
                    Debug.Assert(false && "Geometry has invalid kind.");
                    break;
            }

            if (src.colorName)
            {
                dst.colorName = Strings.intern(src.colorName);
            }
            dst.color = src.color;

            if (src.triangulation != null)
            {
                dst.triangulation = Arena.alloc<Triangulation>();

                var stri = src.triangulation;
                var dtri = dst.triangulation;
                dtri.error = stri.error;
                dtri.id = stri.id;
                if (stri.vertices_n != 0)
                {
                    dtri.vertices_n = stri.vertices_n;
                    dtri.vertices = (float)Arena.dup(stri.vertices, 3 * sizeof(float) * dtri.vertices_n);
                    dtri.normals = (float)Arena.dup(stri.normals, 3 * sizeof(float) * dtri.vertices_n);
                    dtri.texCoords = (float)Arena.dup(stri.texCoords, 2 * sizeof(float) * dtri.vertices_n);
                }
                if (stri.triangles_n != 0)
                {
                    dtri.triangles_n = stri.triangles_n;
                    dtri.indices = (uint)Arena.dup(stri.indices, 3 * sizeof(uint) * dtri.triangles_n);
                }
            }

            return dst;
        }

        public Group cloneGroup(Group parent, Group src)
        {
            var dst = newGroup(parent, src.kind);
            switch (src.kind)
            {
                case Group.Kind.File:
                    dst.file.info = Strings.intern(src.file.info);
                    dst.file.note = Strings.intern(src.file.note);
                    dst.file.date = Strings.intern(src.file.date);
                    dst.file.user = Strings.intern(src.file.user);
                    dst.file.encoding = Strings.intern(src.file.encoding);
                    break;

                case Group.Kind.Model:
                    dst.model.project = Strings.intern(src.model.project);
                    dst.model.name = Strings.intern(src.model.name);
                    break;

                case Group.Kind.Group:
                    dst.group.name = Strings.intern(src.group.name);
                    dst.group.bboxWorld = src.group.bboxWorld;
                    dst.group.material = src.group.material;
                    dst.group.id = src.group.id;
                    for (uint k = 0; k < 3; k++)
                    {
                        dst.group.translation[k] = src.group.translation[k];
                    }
                    break;

                default:
                    Debug.Assert(false && "Group has invalid kind.");
                    break;
            }

            for (var* src_att = src.attributes.first; src_att != null; src_att = src_att.next)
            {
                var dst_att = newAttribute(dst, Strings.intern(src_att.key));
                dst_att.val = Strings.intern(src_att.val);
            }

            return dst;
        }

        //C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
        //ORIGINAL LINE: uint emptyLeafCount() const
        public uint emptyLeafCount()
        {
            return numEmptyLeaves;
        }

        //C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
        //ORIGINAL LINE: const char* errorString() const
        public string errorString()
        {
            return errorStr;
        }

        public Group findRootGroup(string name)
        {
            for (var* file = roots.first; file != null; file = file.next)
            {
                Debug.Assert(file.kind == Group.Kind.File);
                for (var* model = file.groups.first; model != null; model = model.next)
                {
                    //fprintf(stderr, "model '%s'\n", model->model.name);
                    Debug.Assert(model.kind == Group.Kind.Model);
                    for (var* group = model.groups.first; group != null; group = group.next)
                    {
                        //fprintf(stderr, "group '%s' %p\n", group->group.name, (void*)group->group.name);
                        if (group.group.name == name)
                        {
                            return group;
                        }
                    }
                }
            }
            return null;
        }

        public void forwardGroupIdToGeometries()
        {
            for (var* root = roots.first; root != null; root = root.next)
            {
                GlobalMembers.storeGroupIndexInGeometriesRecurse(root);
            }
        }

        //C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
        //ORIGINAL LINE: uint geometryCount_() const
        public uint geometryCount_()
        {
            return numGeometries;
        }

        //C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
        //ORIGINAL LINE: uint geometryCountAllocated() const
        public uint geometryCountAllocated()
        {
            return numGeometriesAllocated;
        }

        public Attribute getAttribute(Group group, string key)
        {
            for (var* attribute = group.attributes.first; attribute != null; attribute = attribute.next)
            {
                if (attribute.key == key)
                {
                    return attribute;
                }
            }
            return null;
        }

        public Group getDefaultModel()
        {
            var file = roots.first;
            if (file == null)
            {
                file = newGroup(null, Group.Kind.File);
                file.file.info = Strings.intern("");
                file.file.note = Strings.intern("");
                file.file.date = Strings.intern("");
                file.file.user = Strings.intern("");
                file.file.encoding = Strings.intern("");
            }
            var model = file.groups.first;
            if (model == null)
            {
                model = newGroup(file, Group.Kind.Model);
                model.model.project = Strings.intern("");
                model.model.name = Strings.intern("");
            }
            return model;
        }

        public Connection getFirstConnection()
        {
            //C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
            //ORIGINAL LINE: return connections.first;
            return new Connection(connections.first);
        }

        public DebugLine getFirstDebugLine()
        {
            //C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
            //ORIGINAL LINE: return debugLines.first;
            return new DebugLine(debugLines.first);
        }

        public Group getFirstRoot()
        {
            //C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
            //ORIGINAL LINE: return roots.first;
            return new Group(roots.first);
        }

        //C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
        //ORIGINAL LINE: uint groupCount_() const
        public uint groupCount_()
        {
            return numGroups;
        }

        //C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
        //ORIGINAL LINE: uint groupCountAllocated() const
        public uint groupCountAllocated()
        {
            return numGroupsAllocated;
        }

        //C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
        //ORIGINAL LINE: uint leafCount() const
        public uint leafCount()
        {
            return numLeaves;
        }

        public Attribute newAttribute(Group group, string key)
        {
            var attribute = Arena.alloc<Attribute>();
            attribute.key = key;
            GlobalMembers.insert(group.attributes, attribute);
            return attribute;
        }

        public Color NewColor(Group parent)
        {
            Debug.Assert(parent != null);
            Debug.Assert(parent.Kindd == Group.Kind.Model);
            var color = new Color();
            GlobalMembers.Insert(parent.Model.Colors, color);
            return color;
        }

        public Connection newConnection()
        {
            var connection = Arena.alloc<Connection>();
            GlobalMembers.insert(connections, connection);
            return connection;
        }

        public Geometry newGeometry(Group parent)
        {
            Debug.Assert(parent != null);
            Debug.Assert(parent.Kind == Group.Kind.Group);

            var geo = Arena.alloc<Geometry>();
            geo.next = null;
            geo.id = numGeometriesAllocated++;

            GlobalMembers.insert(parent.group.geometries, geo);
            return geo;
        }

        public Group newGroup(Group parent, Group.Kind kind)
        {
            var grp = Arena.alloc<Group>();
            //C++ TO C# CONVERTER TODO TASK: The memory management function 'memset' has no equivalent in C#:
            memset(grp, 0, sizeof(Group));

            if (parent == null)
            {
                //C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
                //ORIGINAL LINE: insert(roots, grp);
                GlobalMembers.insert(roots, new Group(grp));
            }
            else
            {
                //C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
                //ORIGINAL LINE: insert(parent->groups, grp);
                GlobalMembers.insert(parent.groups, new Group(grp));
            }

            grp.kind = kind;
            numGroupsAllocated++;
            //C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
            //ORIGINAL LINE: return grp;
            return new Group(grp);
        }

        //C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
        //ORIGINAL LINE: uint nonEmptyNonLeafCount() const
        public uint nonEmptyNonLeafCount()
        {
            return numNonEmptyNonLeaves;
        }

        public void SetErrorString(string str)
        {
            var l = str.Length;
            errorStr = Strings.intern(str, str + l);
        }

        public void updateCounts()
        {
            numGroups = 0;
            numLeaves = 0;
            numEmptyLeaves = 0;
            numNonEmptyNonLeaves = 0;
            numGeometries = 0;
            for (var* root = roots.first; root != null; root = root.next)
            {
                updateCountsRecurse(root);
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void apply(StoreVisitor visitor, Group group)
        {
            Debug.Assert(group.kind == Group.Kind.Group);
            visitor.beginGroup(group);

            if (group.attributes.first != null)
            {
                visitor.beginAttributes(group);
                for (var* a = group.attributes.first; a != null; a = a.next)
                {
                    visitor.attribute(a.key, a.val);
                }
                visitor.endAttributes(group);
            }

            if (group.kind == Group.Kind.Group && group.group.geometries.first != null)
            {
                visitor.beginGeometries(group);
                for (var* geo = group.group.geometries.first; geo != null; geo = geo.next)
                {
                    visitor.geometry(geo);
                }
                visitor.endGeometries();
            }

            visitor.doneGroupContents(group);

            if (group.groups.first != null)
            {
                visitor.beginChildren(group);
                for (var* g = group.groups.first; g != null; g = g.next)
                {
                    apply(visitor, g);
                }
                visitor.endChildren();
            }

            visitor.EndGroup();
        }

        private void updateCountsRecurse(Group group)
        {
            for (var* child = group.groups.first; child != null; child = child.next)
            {
                updateCountsRecurse(child);
            }

            numGroups++;
            if (group.groups.first == null)
            {
                numLeaves++;
            }

            if (group.kind == Group.Kind.Group)
            {
                if (group.groups.first == null && group.group.geometries.first == null)
                {
                    numEmptyLeaves++;
                }

                if (group.groups.first != null && group.group.geometries.first != null)
                {
                    numNonEmptyNonLeaves++;
                }

                for (var* geo = group.group.geometries.first; geo != null; geo = geo.next)
                {
                    numGeometries++;
                }
            }
        }

        #endregion Private Methods
    }
}