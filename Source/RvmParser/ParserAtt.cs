using System.Diagnostics;

//C++ TO C# CONVERTER NOTE: C# does not allow anonymous namespaces:
//namespace

  public class StackItem
  {
	public readonly string id;
	public Group group;
  }

  public class Context
  {
	public Store store;
	public Logger logger;
	public string headerInfo = null; //Header Information
	public string buf;
	public uint buf_size;
	public uint line;
	public uint spaces;
	public uint tabs;

	public StackItem[] stack = null;
	public uint stack_p = 0;
	public uint stack_c = 0;

	public bool create;
  }