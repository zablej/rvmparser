using System.Diagnostics;

public class AddGroupBBox : StoreVisitor
{
  public override void init(Store store)
  {
	this.store = store;
	stack = (Group)arena.alloc(sizeof(Group) * store.groupCountAllocated());
	stack_p = 0;
  }

  public override void geometry(Geometry geometry)
  {
	Debug.Assert(stack_p);
	var M = geometry.M_3x4;

	GlobalMembers.engulf(stack[stack_p - 1].group.bboxWorld, geometry.bboxWorld);
  }

  public override void beginGroup(Group group)
  {
	group.group.bboxWorld = GlobalMembers.createEmptyBBox3f();
	stack[stack_p] = group;
	stack_p++;
  }

  public override void EndGroup()
  {
	Debug.Assert(stack_p);
	stack_p--;

	var bbox = stack[stack_p].group.bboxWorld;
	if (!GlobalMembers.isEmpty(bbox) && 0 < stack_p)
	{
	  var parentBox = stack[stack_p - 1].group.bboxWorld;
	  GlobalMembers.engulf(parentBox, bbox);
	}
  }

  protected Store store = null;

  protected Arena arena = new Arena();
  protected Group[] stack = null;
  protected uint stack_p = 0;

}
