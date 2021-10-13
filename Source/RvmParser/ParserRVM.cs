using System;
using System.Collections.Generic;
using System.Diagnostics;

//C++ TO C# CONVERTER NOTE: C# does not allow anonymous namespaces:
//namespace

  public class Context
  {
	public Store store;
	public string buf;
	public uint buf_size;
	public List<Group> group_stack = new List<Group>();
  }

//C++ TO C# CONVERTER TODO TASK: Unions are not supported in C#:
//  union