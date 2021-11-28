using System;
using System.Collections.Generic;
using System.Text;

namespace Lib
{
    public struct Vec2f
    {
        public Vec2f(Vec2f other)
        {
            x = other.x;
            y = other.y;
        }
        public Vec2f(float x)
        {
            this.x = this.y = x;
        }
        public Vec2f(float[] ptr)
        {
            x = ptr[0];
            y = ptr[1];
        }
        public Vec2f(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vec2f operator *(float a, Vec2f b)
        {
            return new Vec2f(a * b.x, a * b.y);
        }

        public static Vec2f operator -(Vec2f a, Vec2f b)
        {
            return new Vec2f(a.x - b.x, a.y - b.y);
        }

        public static Vec2f operator +(Vec2f a, Vec2f b)
        {
            return new Vec2f(a.x + b.x, a.y + b.y);
        }

        public float x;
        public float y;

        public float[] data => new float[2] { x, y };
        public float this[uint i] => data[i];
    }


    public struct Vec3f
    {
        public Vec3f(Vec3f other)
        {
            x = other.x;
            y = other.y;
            z = other.z;
        }
        public Vec3f(float x)
        {
            this.x = this.y = this.z = x;
        }
        public Vec3f(float[] ptr)
        {
            x = ptr[0];
            y = ptr[1];
            z = ptr[2];
        }
        public Vec3f(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public Vec3f(Vec2f vector2, float z)
        {
            this.x = vector2.x;
            this.y = vector2.y;
            this.z = z;
        }

        public static Vec3f cross(Vec3f a, Vec3f b)
        {
            return new Vec3f(a.y * b.z - a.z * b.y,
                         a.z * b.x - a.x * b.z,
                         a.x * b.y - a.y * b.x);
        }

        public static float dot(Vec3f a, Vec3f b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static Vec3f operator -(Vec3f a, Vec3f b) =>
                    new Vec3f(a.x - b.x, a.y - b.y, a.z - b.z);

        public static Vec3f operator +(Vec3f a, Vec3f b) =>
            new Vec3f(a.x + b.x, a.y + b.y, a.z + b.z);

        public static Vec3f operator *(float a, Vec3f b) =>
            new Vec3f(a * b.x, a * b.y, a * b.z);

        public static float lengthSquared(Vec3f a)
        {
            return dot(a, a);
        }

        public static float length(Vec3f a)
        {
            return (float)Math.Sqrt(dot(a, a));
        }

        public static float distanceSquared(Vec3f a, Vec3f b)
        {
            return lengthSquared(a - b);
        }

        public static float distance(Vec3f a, Vec3f b)
        {
            return length(a - b);
        }

        public static Vec3f normalize(Vec3f a)
        {
            return (1f / length(a)) * a;
        }

        public static void write(float dst, Vec3f a)
        {
            dst++ = a.data[0];
            dst++ = a.data[1];
            dst++ = a.data[2];
        }

        public static Vec3f max(Vec3f a, Vec3f b)
        {
            return new Vec3f(a.x > b.x ? a.x : b.x,
                         a.y > b.y ? a.y : b.y,
                         a.z > b.z ? a.z : b.z);
        }

        public static Vec3f min(Vec3f a, Vec3f b)
        {
            return new Vec3f(a.x < b.x ? a.x : b.x,
                         a.y < b.y ? a.y : b.y,
                         a.z < b.z ? a.z : b.z);
        }

        public static Vec3f mul(Mat3f A, Vec3f x)
        {
            var r = new Vec3f();
            for (uint k = 0; k < 3; k++)
            {
                r.data[k] = A.data[k] * x.data[0] + A.data[3 + k] * x.data[1] + A.data[6 + k] * x.data[2];
            }
            return r;
        }


        public static Vec3f mul(Mat3x4f A, Vec3f x)
        {
            var r = new Vec3f();
            for (uint k = 0; k < 3; k++)
            {
                r.data[k] = A.data[k] * x.data[0] + A.data[3 + k] * x.data[1] + A.data[6 + k] * x.data[2] + A.data[9 + k];
            }
            return r;
        }

        public float x;
        public float y;
        public float z;

        public float[] data => new float[3] { x, y, z };
        public float this[uint i] => data[i];
    }


    public struct BBox3f
    {
        public BBox3f(BBox3f other)
        {
            min = other.min;
            max = other.max;
        }
        public BBox3f(Vec3f min, Vec3f max)
        {
            this.min = min;
            this.max = max;
        }
        public BBox3f(BBox3f bbox, float margin)
        {
            min = bbox.min - new Vec3f(margin);
            max = bbox.max + new Vec3f(margin);
        }

        public static BBox3f createEmptyBBox3f()
        {
            return new BBox3f(new Vec3f(float.MaxValue, float.MaxValue, float.MaxValue), new Vec3f(-float.MaxValue, -float.MaxValue, -float.MaxValue));
        }

        public static void engulf(BBox3f target, Vec3f p)
        {
            target.min = Vec3f.min(target.min, p);
            target.max = Vec3f.max(target.max, p);
        }

        public static void engulf(BBox3f target, BBox3f other)
        {
            target.min = Vec3f.min(target.min, other.min);
            target.max = Vec3f.max(target.max, other.max);
        }

        public static float diagonal(BBox3f b)
        {
            return Vec3f.distance(b.min, b.max);
        }

        public static bool isEmpty(BBox3f b)
        {
            return b.max.x < b.min.x;
        }

        public static bool isNotEmpty(BBox3f b)
        {
            return b.min.x <= b.max.x;
        }

        public static float maxSideLength(BBox3f b)
        {
            var l = b.max - b.min;
            var t = l.x > l.y ? l.x : l.y;
            return l.z > t ? l.z : t;
        }

        public static bool isStrictlyInside(BBox3f a, BBox3f b)
        {
            var lx = a.min.x <= b.min.x;
            var ly = a.min.y <= b.min.y;
            var lz = a.min.z <= b.min.z;
            var ux = b.max.x <= a.max.x;
            var uy = b.max.y <= a.max.y;
            var uz = b.max.z <= a.max.z;
            return lx && ly && lz && ux && uy && uz;
        }

        public static bool isNotOverlapping(BBox3f a, BBox3f b)
        {
            var lx = b.max.x < a.min.x;
            var ly = b.max.y < a.min.y;
            var lz = b.max.z < a.min.z;
            var ux = a.max.x < b.min.x;
            var uy = a.max.y < b.min.y;
            var uz = a.max.z < b.min.z;
            return lx || ly || lz || ux || uy || uz;
        }

        public static bool isOverlapping(BBox3f a, BBox3f b)
        {
            return !isNotOverlapping(a, b);
        }

        public Vec3f min;
        public Vec3f max;
        public float[] data => new float[6] { min.x, min.y, min.z, max.x, max.y, max.z };
    }

    public struct Mat3f
    {
        public Mat3f(Mat3f other)
        {
            m00 = other.m00;
            m10 = other.m10;
            m20 = other.m20;
            m01 = other.m01;
            m11 = other.m11;
            m21 = other.m21;
            m02 = other.m02;
            m12 = other.m12;
            m22 = other.m22;
        }
        public Mat3f(float[] ptr)
        {
            m00 = ptr[0];
            m10 = ptr[1];
            m20 = ptr[2];
            m01 = ptr[3];
            m11 = ptr[4];
            m21 = ptr[5];
            m02 = ptr[6];
            m12 = ptr[7];
            m22 = ptr[8];
        }
        public Mat3f(float m00, float m01, float m02,
          float m10, float m11, float m12,
          float m20, float m21, float m22)
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

        public float m00;
        public float m10;
        public float m20;
        public float m01;
        public float m11;
        public float m21;
        public float m02;
        public float m12;
        public float m22;
        public Vec3f[] cols => new Vec3f[3]
        {
            new Vec3f(m00, m10, m20),
            new Vec3f(m01, m11, m21),
            new Vec3f(m02, m12, m22)
        };
        public float[] data => new float[9]
        {
            m00,
            m10,
            m20,
            m01,
            m11,
            m21,
            m02,
            m12,
            m22
        };
    }


    public struct Mat3x4f
    {
        public Mat3x4f(Mat3x4f other)
        {
            m00 = other.m00;
            m10 = other.m10;
            m20 = other.m20;
            m01 = other.m01;
            m11 = other.m11;
            m21 = other.m21;
            m02 = other.m02;
            m12 = other.m12;
            m22 = other.m22;
            m03 = other.m03;
            m13 = other.m13;
            m23 = other.m23;
        }
        public Mat3x4f(float[] ptr)
        {
            m00 = ptr[0];
            m10 = ptr[1];
            m20 = ptr[2];
            m01 = ptr[3];
            m11 = ptr[4];
            m21 = ptr[5];
            m02 = ptr[6];
            m12 = ptr[7];
            m22 = ptr[8];
            m03 = ptr[9];
            m13 = ptr[10];
            m23 = ptr[11];
        }

        public static float getScale(Mat3x4f M)
        {
            return LinAlgOps.getScale(new Mat3f(M.data));
        }

        public float m00;
        public float m10;
        public float m20;

        public float m01;
        public float m11;
        public float m21;

        public float m02;
        public float m12;
        public float m22;

        public float m03;
        public float m13;
        public float m23;

        public Vec3f[] cols => new Vec3f[4]
        {
            new Vec3f(m00, m10, m20),
            new Vec3f(m01, m11, m21),
            new Vec3f(m02, m12, m22),
            new Vec3f(m03, m13, m23),
        };
        public float[] data => new float[12]
        {
            m00,
            m10,
            m20,
            m01,
            m11,
            m21,
            m02,
            m12,
            m22,
            m03,
            m13,
            m23
        };
    };
}
