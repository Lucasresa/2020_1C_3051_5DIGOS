using BulletSharp;
using TGC.Core.BulletPhysics;
using TGC.Group.Model.Terrains;

namespace TGC.Group.Model.Bullet.Bodies
{
    class TerrainRigidBody
    {
        private Terrain Terrain;
        private BulletRigidBodyFactory rigidBodyFactory = BulletRigidBodyFactory.Instance;
        public RigidBody body;

        public TerrainRigidBody(Terrain terrain)
        {
            Terrain = terrain;
            Init();
        }

        public void Init()
        {
            body = rigidBodyFactory.CreateSurfaceFromHeighMap(Terrain.world.getVertices());
        }

        public void Render()
        {
            Terrain.Render();
        }

        public void Dispose()
        {
            Terrain.Dispose();
            body.Dispose();
        }
    }
}
