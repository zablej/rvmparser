public class Vec2f
{
//C++ TO C# CONVERTER TODO TASK: C# has no equivalent to ' = default':
//  Vec2f() = default;
//C++ TO C# CONVERTER TODO TASK: C# has no equivalent to ' = default':
//  Vec2f(const Vec2f&) = default;
  public Vec2f(float x)
  {
	  this.x = x;
	  this.y = x;
  }
  public Vec2f(ref float ptr)
  {
	  this.x = ptr[0];
	  this.y = ptr[1];
  }
  public Vec2f(float x, float y)
  {
	  this.x = x;
	  this.y = y;
  }
//C++ TO C# CONVERTER TODO TASK: Unions are not supported in C#:
//  union
//  {
//	struct
//	{
//	  float x;
//	  float y;
//	};
//	float data[2];
//  };
  public float this[uint i]
  {
	  get
	  {
		  return data[i];
	  }
	  set
	  {
		  data[i] = value;
	  }
  }
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: const float& operator [](uint i) const
  public float this[uint i]
  {
	  get
	  {
		  return data[i];
	  }
	  set
	  {
		  data[i] = value;
	  }
  }
}


public class Vec3f
{
//C++ TO C# CONVERTER TODO TASK: C# has no equivalent to ' = default':
//  Vec3f() = default;
//C++ TO C# CONVERTER TODO TASK: C# has no equivalent to ' = default':
//  Vec3f(const Vec3f&) = default;
  public Vec3f(float x)
  {
	  this.x = x;
	  this.y = x;
	  this.z = x;
  }
  public Vec3f(ref float ptr)
  {
	  this.x = ptr[0];
	  this.y = ptr[1];
	  this.z = ptr[2];
  }
  public Vec3f(float x, float y, float z)
  {
	  this.x = x;
	  this.y = y;
	  this.z = z;
  }
  public Vec3f(Vec2f a, float z)
  {
	  this.x = a.x;
	  this.y = a.y;
	  this.z = z;
  }

//C++ TO C# CONVERTER TODO TASK: Unions are not supported in C#:
//  union
//  {
//	struct
//	{
//	  float x;
//	  float y;
//	  float z;
//	};
//	float data[3];
//  };
  public float this[uint i]
  {
	  get
	  {
		  return data[i];
	  }
	  set
	  {
		  data[i] = value;
	  }
  }
//C++ TO C# CONVERTER WARNING: 'const' methods are not available in C#:
//ORIGINAL LINE: const float& operator [](uint i) const
  public float this[uint i]
  {
	  get
	  {
		  return data[i];
	  }
	  set
	  {
		  data[i] = value;
	  }
  }
}


public class BBox3f
{
//C++ TO C# CONVERTER TODO TASK: C# has no equivalent to ' = default':
//  BBox3f() = default;
//C++ TO C# CONVERTER TODO TASK: C# has no equivalent to ' = default':
//  BBox3f(const BBox3f&) = default;
  public BBox3f(Vec3f min, Vec3f max)
  {
	  this.min = min;
	  this.max = max;
  }
//C++ TO C# CONVERTER TODO TASK: The implementation of the following method could not be found:
//  BBox3f(BBox3f bbox, float margin);

//C++ TO C# CONVERTER TODO TASK: Unions are not supported in C#:
//  union
//  {
//	struct
//	{
//	  Vec3f min;
//	  Vec3f max;
//	};
//	float data[6];
//  };
}

public class Mat3f
{
//C++ TO C# CONVERTER TODO TASK: C# has no equivalent to ' = default':
//  Mat3f() = default;
//C++ TO C# CONVERTER TODO TASK: C# has no equivalent to ' = default':
//  Mat3f(const Mat3f&) = default;
  public Mat3f(float[] ptr)
  {
	  for (uint i = 0; i < 3 * 3; i++)
	  {
		  data[i] = ptr[i];
	  }
  }
  public Mat3f(float m00, float m01, float m02, float m10, float m11, float m12, float m20, float m21, float m22)
  {
	  this.m00 = m00;
	  this.m10 = m10;
	  this.m20 = m20;
	  this.m01 = m01;
	  this.m11 = m11;
	  this.m21 = m21;
	  this.m02 = m02;
	  this.m12 = m12;
	  this.m22 = m22;
  }


//C++ TO C# CONVERTER TODO TASK: Unions are not supported in C#:
//  union
//  {
//	struct
//	{
//	  float m00;
//	  float m10;
//	  float m20;
//	  float m01;
//	  float m11;
//	  float m21;
//	  float m02;
//	  float m12;
//	  float m22;
//	};
//	Vec3f cols[3];
//	float data[3 * 3];
//  };
}


public class Mat3x4f
{
//C++ TO C# CONVERTER TODO TASK: C# has no equivalent to ' = default':
//  Mat3x4f() = default;
//C++ TO C# CONVERTER TODO TASK: C# has no equivalent to ' = default':
//  Mat3x4f(const Mat3x4f&) = default;
  public Mat3x4f(float[] ptr)
  {
	  for (uint i = 0; i < 4 * 3; i++)
	  {
		  data[i] = ptr[i];
	  }
  }

//C++ TO C# CONVERTER TODO TASK: Unions are not supported in C#:
//  union
//  {
//	struct
//	{
//	  float m00;
//	  float m10;
//	  float m20;
//
//	  float m01;
//	  float m11;
//	  float m21;
//
//	  float m02;
//	  float m12;
//	  float m22;
//
//	  float m03;
//	  float m13;
//	  float m23;
//	};
//	Vec3f cols[4];
//	float data[4 * 3];
//  };
}
