using BulletSharp;
using TGC.Group.Model.Terrains;

namespace TGC.Group.Model.Bullet.Bodies
{
    class TerrainRigidBody : RigidBodies
    {
        private Terrain Terrain;

        public TerrainRigidBody(Terrain terrain)
        {
            Terrain = terrain;
        }

        public override void Init()
        {
            RigidBody = rigidBodyFactory.CreateSurfaceFromHeighMap(Terrain.world.getVertices());
        }

        public override void Dispose()
        {
            Terrain.Dispose();
            RigidBody.Dispose();
        }
    }
}
