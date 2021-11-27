//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Lib
//{
//    public class BufferBase
//    {
//        protected char[] ptr = null;

//        ~BufferBase() { free(); }

//        protected void free();

//        protected void _accommodate(UIntPtr typeSize, UIntPtr count)
//        {
//            if (count == UIntPtr.Zero)
//                return;
//            if (ptr && count <= ((UIntPtr[])ptr)[-1]) return;

//            free();

//            ptr = (char*)xmalloc(typeSize * count + sizeof(size_t)) + sizeof(size_t);
//            ((size_t*)ptr)[-1] = count;
//        }

//    }

//    public class Buffer<T> : BufferBase
//    {
//        public T data() { return (T)ptr; }
//        public T operator[](UIntPtr ix) 
//            { 
//            return data()[ix]; 
//}
//    public T data()
//    {
//        return (T)ptr;
//    }
//    public T operator[](UIntPtr ix) const { return data()[ix]; }
//public void accommodate(UIntPtr count) { _accommodate(sizeof(T), count); }
//}


//public class Map
//{
//    public Map() = default;
//  public Map(const Map&) = delete;
//  public Map& operator=(const Map&) = delete;


//  ~Map();

//    public ulong? keys = null;
//    public ulong? vals = null;
//    public UIntPtr fill = new UIntPtr(0);
//    public UIntPtr capacity = new UIntPtr(0);

//    public void clear();

//    public bool get(ulong val, ulong key);
//    public ulong get(ulong key);

//    public void insert(ulong key, ulong value);
//}

//public class StringInterning
//{
//    public Arena arena;
//    public Map map;

//    public char intern(char a, char b);
//    public char intern(char str);  // null terminanted
//}
//}
