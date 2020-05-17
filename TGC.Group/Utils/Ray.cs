using System;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;
using TGC.Core.Input;
using TGC.Core.Mathematica;

namespace TGC.Group.Utils
{
    class Ray
    {
        private TgcD3dInput Input;
        private TgcPickingRay pickingRay;

        public Ray(TgcD3dInput input)
        {
            Input = input;
            pickingRay = new TgcPickingRay(Input);
        }

        public bool intersectsWithObject(TgcBoundingAxisAlignBox objectAABB, float distance)
        {
            pickingRay.updateRay();

            bool intersected = TgcCollisionUtils.intersectRayAABB(pickingRay.Ray, objectAABB, out TGCVector3 collisionPoint);
            bool inSight = Math.Sqrt(TGCVector3.LengthSq(pickingRay.Ray.Origin, collisionPoint)) < distance;

            return intersected && inSight;
        }
    }
}
