using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RvmParser.Storing
{
	public class Group
	{
		public enum Kind
		{
			File,
			Model,
			Group
		}

		public enum Flags
		{
			None = 0,
			ClientFlagStart = 1
		}

		public Group Next { get; set; }
		public IList<Group> Groups = new List<Group>();
		public IList<Attribute> Attributes = new List<Attribute>();

		public Kind Kindd { get; set; } = Kind.Group;
		public Flags Flag { get; set; } = Flags.None;

		//C++ TO C# CONVERTER TODO TASK: Unions are not supported in C#:
		//  union
		//  {
		//	struct
		//	{
		//	  const char* info;
		//	  const char* note;
		//	  const char* date;
		//	  const char* user;
		//	  const char* encoding;
		//	}
		//	file;
		//	struct
		//	{
		//	  ListHeader<Color> colors;
		//	  const char* project;
		//	  const char* name;
		//	}
		//	model;
		//	struct
		//	{
		//	  ListHeader<Geometry> geometries;
		//	  const char* name;
		//	  BBox3f bboxWorld;
		//	  uint material;
		//	  int id = 0;
		//	  float translation[3];
		//	  uint clientTag; // For use by passes to stuff temporary info
		//	}
		//	group;
		//  };

	}
}
