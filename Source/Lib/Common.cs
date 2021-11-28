using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Lib
{
//    public class BufferBase
//    {
//        protected uint[] ptr = null;

//        ~BufferBase() { free(); }

//        protected void free()
//        {
//            if (ptr != null)
//                ptr = null;
//        }

//        protected void _accommodate(uint typeSize, uint count)
//        {
//            if (count == 0)
//                return;
//            if (ptr != null && count <= ptr[ptr.Length - 1])
//                return;

//            free();

//            ptr = (char)xmalloc(typeSize * count + sizeof(uint)) + sizeof(uint);
//            ptr[ptr.Length - 1] = count;
//        }

//    }

//    public class Buffer<T> : BufferBase
//    {
//        public T data()
//        {
//            return (T)ptr;
//        }
//        public T this[uint ix] =>
//            data()[ix];

//        public T data()
//        {
//            return (T)ptr;
//        }

//        public void accommodate(uint count) { _accommodate(sizeof(T), count); }
//}


public class Map
{
    public Map() = default;
  public Map(const Map&) = delete;
  public Map& operator=(const Map&) = delete;


  ~Map()
    {
        free(keys);
        free(vals);
    }

    public ulong? keys = null;
    public ulong? vals = null;
    public uint fill = new uint(0);
    public uint capacity = new uint(0);

    public void clear()
    {
        for (uint i = 0; i < capacity; i++)
        {
            keys[i] = 0;
            vals[i] = 0;
        }
        fill = 0;
    }

    public bool get(ulong val, ulong key)
    {
        Debug.Assert(key != 0);
        if (fill == 0) return false;

        var mask = capacity - 1;
        for (var i = (uint)hash_uint64(key); true; i++)
        { // linear probing
            i = i & mask;
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
    public ulong get(ulong key)
    {
        ulong rv = 0;
        get(rv, key);
        return rv;
    }

    public void insert(ulong key, ulong value)
    {
        Debug.Assert(key != 0);     // null is used to denote no-key
                                    //assert(value != 0);   // null value is used to denote not found

        if (capacity <= 2 * fill)
        {
            var old_capacity = capacity;
            var old_keys = keys;
            var old_vals = vals;

            fill = 0;
            capacity = capacity ? 2 * capacity : 16;
            keys = (ulong[])xcalloc(capacity, sizeof(uint64_t));
            vals = (ulong[])xmalloc(capacity * sizeof(uint64_t));

            uint g = 0;
            for (uint i = 0; i < old_capacity; i++)
            {
                if (old_keys[i])
                {
                    insert(old_keys[i], old_vals[i]);
                    g++;
                }
            }

            free(old_keys);
            free(old_vals);
        }

        Debug.Assert(isPow2(capacity));
        var mask = capacity - 1;
        for (var i = (uint)hash_uint64(key); true; i++)
        { // linear probing
            i = i & mask;
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
    public Map map;

    public string intern(char a, char b);
    public string intern(string str);  // null terminanted
}

public static class Common
{
    public static bool isPow2<T>(int x) where T : IComparable
    {
        return x != 0 && (x & (x - 1)) == 0;
    }

    public static ulong hash_uint64(ulong x)
    {
        x *= 0xff51afd7ed558ccd;
        x ^= x >> 32;
        return x;
    }

    public static ulong fnv_1a(byte[] bytes, uint l)
    {
        ulong hash = 0xcbf29ce484222325;
        for (uint i = 0; i < l; i++)
        {
            hash = hash ^ bytes[i];
            hash = hash * 0x100000001B3;
        }
        return hash;
    }


    public static void xmalloc(uint size)
    {
        var rv = malloc(size);
        if (rv != null) return rv;

        fprintf(stderr, "Failed to allocate memory.");
        Environment.Exit(-1);
    }

    public static void xcalloc(uint count, uint size)
    {
        var rv = calloc(count, size);
        if (rv != null) return rv;

        fprintf(stderr, "Failed to allocate memory.");
        Environment.Exit(-1);
    }

    public static void xrealloc(void ptr, uint size)
    {
        var rv = realloc(ptr, size);
        if (rv != null) return rv;

        fprintf(stderr, "Failed to allocate memory.");
        Environment.Exit(-1);
    }
}
