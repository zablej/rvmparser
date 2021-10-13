//C++ TO C# CONVERTER NOTE: C# does not allow anonymous namespaces:
//namespace

  public class Anchor
  {
	public Geometry[] geo = null;
	public Vec3f p = new Vec3f();
	public Vec3f d = new Vec3f();
	public uint o;
	public Connection.Flags flags;
	public uint8_t matched = 0;
  }

  public class Context
  {
	public Store store;
	public Logger logger;
	public Buffer<Anchor> anchors = new Buffer<Anchor>();
	public readonly float epsilon = 0.001f;
	public uint anchors_n = 0;

	public uint anchors_max = 0;

	public uint anchors_total = 0;
	public uint anchors_matched = 0;
  }