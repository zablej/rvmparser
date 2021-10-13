using System.Diagnostics;

public class DumpNames : StoreVisitor
{
  public override void init(Store store)
  {
	Debug.Assert(arena == null);
	Debug.Assert(stack == null);
	Debug.Assert(stack_p == 0);
	arena = new Arena();
	stack = (string)arena.alloc(sizeof(string) * store.groupCountAllocated());
  }

  public override bool done()
  {
	Debug.Assert(stack_p == 0);
	Debug.Assert(stack);
	if (arena != null)
	{
		arena.Dispose();
	}

	arena = null;
	stack = null;

	return true;
  }

  public override void beginFile(Group group)
  {
	fprintf(@out, "File:\n");
	fprintf(@out, "    info:     \"%s\"\n", group.file.info);
	fprintf(@out, "    note:     \"%s\"\n", group.file.note);
	fprintf(@out, "    date:     \"%s\"\n", group.file.date);
	fprintf(@out, "    user:     \"%s\"\n", group.file.user);
	fprintf(@out, "    encoding: \"%s\"\n", group.file.encoding);
  }

  public override void endFile()
  {
  }

  public override void beginModel(Group group)
  {
	fprintf(@out, "Model:\n");
	fprintf(@out, "    project:  \"%s\"\n", group.model.project);
	fprintf(@out, "    name:     \"%s\"\n", group.model.name);
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
	  fprintf(@out, "    ");
	}
	fprintf(@out, "%s", group.group.name);

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

  public void setOutput(FILE o)
  {
	  @out = o;
  }

  private Arena arena = null;
  private FILE @out = stderr;
  private string[] stack = null;
  private uint stack_p = 0;
  private uint facetgroups = 0;
  private uint geometries = 0;
  private bool printed = true;

  private void printGroupTail()
  {
	if (printed)
	{
		return;
	}
	printed = true;

	fprintf(@out, "\n");

	if (geometries != 0)
	{
	  for (uint i = 0; i < stack_p; i++)
	  {
		fprintf(@out, "    ");
	  }
	  fprintf(@out, " pgeos=%d\n", geometries);
	}

	if (facetgroups != 0)
	{
	  for (uint i = 0; i < stack_p; i++)
	  {
		fprintf(@out, "    ");
	  }
	  fprintf(@out, " fgrps=%d\n", facetgroups);
	}
  }
}

