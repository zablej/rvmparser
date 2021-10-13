using System.Diagnostics;

public class Flatten
{
  public Flatten(Store srcStore)
  {
	  this.srcStore = srcStore;
	populateSrcTags();
  }

  // newline seperated bufffer of tags to keep
  public void setKeep(object ptr, size_t size)
  {
//C++ TO C# CONVERTER TODO TASK: Pointer arithmetic is detected on this variable, so pointers on this variable are left unchanged:
	var * a = (string)ptr;
	var b = a + size;

	while (true)
	{
	  while (a < b && (*a == '\n' || *a == '\r'))
	  {
		  a++;
	  }
	  var c = a;
	  while (a < b && (*a != '\n' && *a != '\r'))
	  {
		  a++;
	  }

	  if (c < a)
	  {
		var d = a - 1;
		while (c < d && (d[-1] != '\t'))
		{
			--d;
		}

		var str = srcStore.strings.intern(d, a);
		uint64_t val = new uint64_t();
		if (srcTags.get(val, uint64_t(str)))
		{
		  tags.insert(uint64_t(str), uint64_t(currentIndex));
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
  public void keepTag(string tag)
  {
	var str = srcStore.strings.intern(tag);
	uint64_t val = new uint64_t();
	if (srcTags.get(val, uint64_t(str)))
	{
	  tags.insert(uint64_t(str), uint64_t(currentIndex));
	  ((Group)val).group.id = int32_t(currentIndex);
	  activeTags++;
	}
	currentIndex++;
  }

  // Tags submitted as selected, may be way more than actual number of tags. But consistent between different stores.
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: uint selectedTagsCount() const
  public uint selectedTagsCount()
  {
	  return new uint32_t(currentIndex);
  }

//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: uint activeTagsCount() const
  public uint activeTagsCount()
  {
	  return new uint32_t(activeTags);
  }

  public Store run()
  {
	dstStore = new Store();

	// populateSrcTags was run by the constructor, and setKeep and keepTags has changed some group.index from ~0u.
	// set group.index of parents of selected nodes to ~1u so we can retain them in the culling pass.
	for (var * srcRoot = srcStore.getFirstRoot(); srcRoot != null; srcRoot = srcRoot.next)
	{
	  Debug.Assert(srcRoot.kind == Group.Kind.File);
	  for (var * srcModel = srcRoot.groups.first; srcModel != null; srcModel = srcModel.next)
	  {
		Debug.Assert(srcModel.kind == Group.Kind.Model);
		for (var * srcGroup = srcModel.groups.first; srcGroup != null; srcGroup = srcGroup.next)
		{
		  anyChildrenSelectedAndTagRecurse(srcGroup);
		}
	  }
	}

	// Create a fresh copy
	for (var * srcRoot = srcStore.getFirstRoot(); srcRoot != null; srcRoot = srcRoot.next)
	{

	  Debug.Assert(srcRoot.kind == Group.Kind.File);
	  var dstRoot = dstStore.cloneGroup(null, srcRoot);

	  for (var * srcModel = srcRoot.groups.first; srcModel != null; srcModel = srcModel.next)
	  {

		Debug.Assert(srcModel.kind == Group.Kind.Model);
		var dstModel = dstStore.cloneGroup(dstRoot, srcModel);

		for (var * srcGroup = srcModel.groups.first; srcGroup != null; srcGroup = srcGroup.next)
		{
		  buildPrunedCopyRecurse(dstModel, srcGroup, 0);
		}
	  }
	}

	dstStore.updateCounts();
	var rv = dstStore;
	dstStore = null;
//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
//ORIGINAL LINE: return rv;
	return new Store(rv);
  }

  private Map srcTags = new Map(); // All tags in source store
  private Map tags = new Map();

  private Arena arena = new Arena();
  private uint pass = 0;

  private uint32_t currentIndex = 0;
  private uint32_t activeTags = 0;

  private Store srcStore = null;
  private Store dstStore = null;

  private Group[] stack = null;
  private uint stack_p = 0;
  private uint ignore_n = 0;

  private void populateSrcTagsRecurse(Group srcGroup)
  {
	srcGroup.group.id = -1;
	srcTags.insert(uint64_t(srcGroup.group.name), uint64_t(srcGroup));
	for (var * srcChild = srcGroup.groups.first; srcChild != null; srcChild = srcChild.next)
	{
	  populateSrcTagsRecurse(srcChild);
	}
  }

  private void populateSrcTags()
  {
	// Sets all group.index to ~0u, and records the tag-names in srcTags. Nothing is selected yet.
	// name of groups are already interned in srcGroup
	for (var * srcRoot = srcStore.getFirstRoot(); srcRoot != null; srcRoot = srcRoot.next)
	{
	  for (var * srcModel = srcRoot.groups.first; srcModel != null; srcModel = srcModel.next)
	  {
		for (var * srcGroup = srcModel.groups.first; srcGroup != null; srcGroup = srcGroup.next)
		{
		  populateSrcTagsRecurse(srcGroup);
		}
	  }
	}
  }

  private bool anyChildrenSelectedAndTagRecurse(Group srcGroup, int32_t id = -1)
  {
	uint64_t val = new uint64_t();
	if (tags.get(val, uint64_t(srcGroup.group.name)))
	{
	  srcGroup.group.id = int32_t(val);
	}
	else
	{
	  srcGroup.group.id = id;
	}

	bool anyChildrenSelected = false;
	for (var * srcChild = srcGroup.groups.first; srcChild != null; srcChild = srcChild.next)
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

	for (var * srcGeo = srcGroup.group.geometries.first; srcGeo != null; srcGeo = srcGeo.next)
	{
	  dstStore.cloneGeometry(dstParent, srcGeo);
	}

	for (var * srcChild = srcGroup.groups.first; srcChild != null; srcChild = srcChild.next)
	{
	  buildPrunedCopyRecurse(dstParent, srcChild, level + 1);
	}
  }
}

