using System;
using System.Diagnostics;

//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
//class Store;

//C++ TO C# CONVERTER NOTE: C# has no need of forward class declarations:
//struct Triangulation;

public delegate void Logger(uint level, string msg, params object[] LegacyParamArray);


public class Arena : System.IDisposable
{
//C++ TO C# CONVERTER TODO TASK: C# has no equivalent to ' = default':
//  Arena() = default;
//C++ TO C# CONVERTER TODO TASK: C# has no equivalent to ' = delete':
//  Arena(const Arena&) = delete;
//C++ TO C# CONVERTER TODO TASK: C# has no equivalent to ' = delete':
//  Arena& operator =(const Arena&) = delete;

  public void Dispose()
  {
	  clear();
  }

  public uint8_t first = null;
  public uint8_t curr = null;
  public size_t fill = 0;
  public size_t size = 0;

  public object alloc(size_t bytes)
  {
	const size_t pageSize = 1024 * 1024;

	if (bytes == 0)
	{
		return null;
	}

	var padded = (bytes + 7) & ~7;

	if (size < fill + padded)
	{
	  fill = sizeof(uint8_t*);
	  size = Math.Max(pageSize, fill + padded);

	  var page = (uint8_t)GlobalMembers.xmalloc(new size_t(size));
	  (uint8_t)page = null;

	  if (first == null)
	  {
		first = page;
		curr = page;
	  }
	  else
	  {
		(uint8_t)curr = page; // update next
		curr = page;
	  }
	}

	Debug.Assert(first != null);
	Debug.Assert(curr != null);
	Debug.Assert((uint8_t)curr == null);
	Debug.Assert(fill + padded <= size);

	var rv = curr + fill;
	fill += padded;
	return rv;
  }

  public object dup(object src, size_t bytes)
  {
	var dst = alloc(new size_t(bytes));
//C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
	memcpy(dst, src, bytes);
	return dst;
  }

  public void clear()
  {
	var c = first;
	while (c != null)
	{
	  var n = (uint8_t)c;
	  c = null;
	  c = n;
	}
	first = null;
	curr = null;
	fill = 0;
	size = 0;
  }

//C++ TO C# CONVERTER WARNING: The original C++ template specifier was replaced with a C# generic specifier, which may not produce the same behavior:
//ORIGINAL LINE: template<typename T>
  public T alloc<T>()
  {
	  return new(alloc(sizeof(T))) default(T);
  }
}

public class BufferBase : System.IDisposable
{
  protected string ptr = null;

  public void Dispose()
  {
//C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
	  free();
  }

  protected void free()
  {
	if (ptr)
	{
		global::free(ptr - sizeof(size_t));
	}
  }

  protected void _accommodate(size_t typeSize, size_t count)
  {
	if (count == 0)
	{
		return;
	}
	if (ptr && count <= ((size_t)ptr)[-1])
	{
		return;
	}

//C++ TO C# CONVERTER TODO TASK: The memory management function 'free' has no equivalent in C#:
	free();

	ptr = (string)GlobalMembers.xmalloc(typeSize * count + sizeof(size_t)) + sizeof(size_t);
	((size_t)ptr)[-1] = count;
  }

}

//C++ TO C# CONVERTER WARNING: The original C++ template specifier was replaced with a C# generic specifier, which may not produce the same behavior:
//ORIGINAL LINE: template<typename T>
public class Buffer <T>: BufferBase
{
  public T data()
  {
	  return (T)ptr;
  }
  public T this[size_t ix]
  {
	  get
	  {
		  return data()[ix];
	  }
	  set
	  {
		  data()[ix] = value;
	  }
  }
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: const T* data() const
  public T data()
  {
	  return (T)ptr;
  }
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: const T& operator [](size_t ix) const
  public T this[size_t ix]
  {
	  get
	  {
		  return data()[ix];
	  }
	  set
	  {
		  data()[ix] = value;
	  }
  }
  public void accommodate(size_t count)
  {
	  _accommodate(sizeof(T), new size_t(count));
  }
}


public class Map : System.IDisposable
{
//C++ TO C# CONVERTER TODO TASK: C# has no equivalent to ' = default':
//  Map() = default;
//C++ TO C# CONVERTER TODO TASK: C# has no equivalent to ' = delete':
//  Map(const Map&) = delete;
//C++ TO C# CONVERTER TODO TASK: C# has no equivalent to ' = delete':
//  Map& operator =(const Map&) = delete;


  public void Dispose()
  {
	keys = null;
	vals = null;
  }

  public uint64_t[] keys = null;
  public uint64_t[] vals = null;
  public size_t fill = 0;
  public size_t capacity = 0;

  public void clear()
  {
	for (uint i = 0; i < capacity; i++)
	{
	  keys[i] = 0;
	  vals[i] = 0;
	}
	fill = 0;
  }

  public bool get(ref uint64_t val, uint64_t key)
  {
	Debug.Assert(key != 0);
	if (fill == 0)
	{
		return false;
	}

	var mask = capacity - 1;
	for (var i = size_t(GlobalMembers.hash_uint64(new uint64_t(key))); true; i++)
	{ // linear probing
//C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
//ORIGINAL LINE: i = i & mask;
	  i.CopyFrom(i & mask);
	  if (keys[i] == key)
	  {
		val = vals[i];
		return true;
	  }
	  else if (keys[i] == 0)
	  {
		return false;
	  }
	}
  }

  public uint64_t get(uint64_t key)
  {
	uint64_t rv = 0;
	get(ref rv, new uint64_t(key));
	return new uint64_t(rv);
  }

  public void insert(uint64_t key, uint64_t value)
  {
	Debug.Assert(key != 0); // null is used to denote no-key
	//assert(value != 0);   // null value is used to denote not found

	if (capacity <= 2 * fill)
	{
	  var old_capacity = capacity;
	  var old_keys = keys;
	  var old_vals = vals;

	  fill = 0;
	  capacity = capacity != null ? 2 * capacity : 16;
	  keys = (uint64_t)GlobalMembers.xcalloc(new size_t(capacity), sizeof(uint64_t));
	  vals = (uint64_t)GlobalMembers.xmalloc(capacity * sizeof(uint64_t));

	  uint g = 0;
	  for (size_t i = 0; i < old_capacity; i++)
	  {
		if (old_keys[i] != null)
		{
		  insert(old_keys[i], old_vals[i]);
		  g++;
		}
	  }

	  old_keys = null;
	  old_vals = null;
	}

	Debug.Assert(GlobalMembers.isPow2(new size_t(capacity)));
	var mask = capacity - 1;
	for (var i = size_t(GlobalMembers.hash_uint64(new uint64_t(key))); true; i++)
	{ // linear probing
//C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
//ORIGINAL LINE: i = i & mask;
	  i.CopyFrom(i & mask);
	  if (keys[i] == key)
	  {
		vals[i] = value;
		break;
	  }
	  else if (keys[i] == 0)
	  {
		keys[i] = key;
		vals[i] = value;
		fill++;
		break;
	  }
	}

  }
}

public class StringInterning
{
  public Arena arena = new Arena();
  public Map map = new Map();

  public string intern(string a, string b)
  {
	var length = b - a;
	var hash = GlobalMembers.fnv_1a(a, length);
	hash = hash ? hash : 1;

	var intern = (StringHeader)map.get(hash);
	for (var * it = intern; it != null; it = it.next)
	{
	  if (it.length == length)
	  {
		if (string.Compare(it.string, 0, a, 0, length) == 0)
		{
		  return it.string;
		}
	  }
	}

	var newIntern = (StringHeader)arena.alloc(sizeof(StringHeader) + length);
	newIntern.next = intern;
	newIntern.length = length;
//C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
	memcpy(newIntern.string, a, length);
	newIntern.string = StringFunctions.ChangeCharacter(newIntern.string, length, '\0');
	map.insert(hash, uint64_t(newIntern));
	return newIntern.string;
  }

  public string intern(string str)
  {
	return intern(str, str.Substring(str.Length));
  }
}


//C++ TO C# CONVERTER NOTE: C# does not allow anonymous namespaces:
//namespace

//C++ TO C# CONVERTER WARNING: The original C++ template specifier was replaced with a C# generic specifier, which may not produce the same behavior:
//ORIGINAL LINE: template<typename T>


//C++ TO C# CONVERTER NOTE: C# does not allow anonymous namespaces:
//namespace

  public class StringHeader
  {
	public StringHeader next;
	public size_t length = new size_t();
	public string string = new string(new char[1]);
  }

