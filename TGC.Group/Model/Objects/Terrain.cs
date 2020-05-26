using BulletSharp;
using TGC.Core.BulletPhysics;

namespace TGC.Group.Model.Objects
{
    class Terrain : World
    {
        private readonly BulletRigidBodyFactory RigidBodyFactory = BulletRigidBodyFactory.Instance;
        public RigidBody Body { get; set; }

        public Terrain(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Init();
        }

        private void Init()
        {
            FILE_HEIGHTMAPS = "Heightmaps\\suelo.jpg";
            FILE_TEXTURES = "Textures\\sand.jpg";
            FILE_EFFECT = "TerrainShader.fx";
            Technique = "Default";
            LoadWorld();
            Body = RigidBodyFactory.CreateSurfaceFromHeighMap(world.getVertices());
        }

        public override void Dispose()
        {
            Body.Dispose();
            base.Dispose();
        }
    }
}