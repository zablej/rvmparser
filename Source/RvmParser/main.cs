using System;
using System.Diagnostics;

#if _WIN32

#define WIN32_LEAN_AND_MEAN
#define NOMINMAX

#else

//C++ TO C# CONVERTER WARNING: The following #include directive was ignored:
//#include <sys/types.h>
//C++ TO C# CONVERTER WARNING: The following #include directive was ignored:
//#include <sys/stat.h>
//C++ TO C# CONVERTER WARNING: The following #include directive was ignored:
//#include <fcntl.h>
//C++ TO C# CONVERTER WARNING: The following #include directive was ignored:
//#include <unistd.h>
//C++ TO C# CONVERTER WARNING: The following #include directive was ignored:
//#include <sys/mman.h>

#endif

//C++ TO C# CONVERTER NOTE: C# does not allow anonymous namespaces:
//namespace