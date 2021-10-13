//C++ TO C# CONVERTER NOTE: C# does not allow anonymous namespaces:
//namespace

  public class QueueItem
  {
	public Geometry from;
	public Connection connection;
	public Vec3f upWorld = new Vec3f();
  }


  public class Context
  {
	public Buffer<QueueItem> queue = new Buffer<QueueItem>();
	public Logger logger = null;
	public Store store = null;
	public uint front = 0;
	public uint back = 0;
	public uint connectedComponents = 0;
	public uint circularConnections = 0;
	public uint connections = 0;

  }