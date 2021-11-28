using System;
using System.Collections.Generic;
using System.Text;

namespace Lib
{
    public static class LinAlgOps
    {

        public static Mat3f inverse(Mat3f M)
        {
            var c0 = M.cols[0];
            var c1 = M.cols[1];
            var c2 = M.cols[2];

            var r0 = Vec3f.cross(c1, c2);
            var r1 = Vec3f.cross(c2, c0);
            var r2 = Vec3f.cross(c0, c1);

            var invDet = 1f / Vec3f.dot(r2, c2);

            return new Mat3f(invDet * r0.x, invDet * r0.y, invDet * r0.z,
                         invDet * r1.x, invDet * r1.y, invDet * r1.z,
                         invDet * r2.x, invDet * r2.y, invDet * r2.z);
        }

        public static Mat3f mul(this Mat3f A, Mat3f B)
        {
            var A00 = A.cols[0].x; var A10 = A.cols[0].y; var A20 = A.cols[0].z;
            var A01 = A.cols[1].x; var A11 = A.cols[1].y; var A21 = A.cols[1].z;
            var A02 = A.cols[2].x; var A12 = A.cols[2].y; var A22 = A.cols[2].z;

            var B00 = B.cols[0].x; var B10 = B.cols[0].y; var B20 = B.cols[0].z;
            var B01 = B.cols[1].x; var B11 = B.cols[1].y; var B21 = B.cols[1].z;
            var B02 = B.cols[2].x; var B12 = B.cols[2].y; var B22 = B.cols[2].z;

            return new Mat3f(A00 * B00 + A01 * B10 + A02 * B20,
                         A00 * B01 + A01 * B11 + A02 * B21,
                         A00 * B02 + A01 * B12 + A02 * B22,

                         A10 * B00 + A11 * B10 + A12 * B20,
                         A10 * B01 + A11 * B11 + A12 * B21,
                         A10 * B02 + A11 * B12 + A12 * B22,

                         A20 * B00 + A21 * B10 + A22 * B20,
                         A20 * B01 + A21 * B11 + A22 * B21,
                         A20 * B02 + A21 * B12 + A22 * B22);
        }

        public static float getScale(Mat3f M)
        {
            float sx = Vec3f.length(M.cols[0]);
            float sy = Vec3f.length(M.cols[1]);
            float sz = Vec3f.length(M.cols[2]);
            var t = sx > sy ? sx : sy;
            return sz > t ? sz : t;
        }

        public static BBox3f transform(Mat3x4f M, BBox3f bbox)
        {
            Vec3f[] p = new Vec3f[8]
            {
                Vec3f.mul(M, new Vec3f(bbox.min.x, bbox.min.y, bbox.min.z)),
                Vec3f.mul(M, new Vec3f(bbox.min.x, bbox.min.y, bbox.max.z)),
                Vec3f.mul(M, new Vec3f(bbox.min.x, bbox.max.y, bbox.min.z)),
                Vec3f.mul(M, new Vec3f(bbox.min.x, bbox.max.y, bbox.max.z)),
                Vec3f.mul(M, new Vec3f(bbox.max.x, bbox.min.y, bbox.min.z)),
                Vec3f.mul(M, new Vec3f(bbox.max.x, bbox.min.y, bbox.max.z)),
                Vec3f.mul(M, new Vec3f(bbox.max.x, bbox.max.y, bbox.min.z)),
                Vec3f.mul(M, new Vec3f(bbox.max.x, bbox.max.y, bbox.max.z))
            };
            return new BBox3f(Vec3f.min(Vec3f.min(Vec3f.min(p[0], p[1]), Vec3f.min(p[2], p[3])), Vec3f.min(Vec3f.min(p[4], p[5]), Vec3f.min(p[6], p[7]))),
                          Vec3f.min(Vec3f.min(Vec3f.min(p[0], p[1]), Vec3f.min(p[2], p[3])), Vec3f.min(Vec3f.min(p[4], p[5]), Vec3f.min(p[6], p[7]))));
        }
    }
}
