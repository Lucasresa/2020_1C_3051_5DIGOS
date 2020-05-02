using BulletSharp;
using TGC.Group.Model.Terrains;

namespace TGC.Group.Model.Bullet.Bodies
{
    class TerrainRigidBody : RigidBody
    {
        private Terrain Terrain;

        public TerrainRigidBody(Terrain terrain)
        {
            Terrain = terrain;
        }

        public override void Init()
        {
            rigidBody = rigidBodyFactory.CreateSurfaceFromHeighMap(Terrain.world.getVertices());
        }

        public override void Render()
        {
            Terrain.Render();
        }

        public override void Dispose()
        {
            Terrain.Dispose();
            rigidBody.Dispose();
        }
    }
}
