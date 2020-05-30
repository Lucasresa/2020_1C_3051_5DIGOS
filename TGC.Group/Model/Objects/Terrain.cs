using BulletSharp;
using TGC.Core.BulletPhysics;

namespace TGC.Group.Model.Objects
{
    internal class Terrain : World
    {
        private readonly BulletRigidBodyFactory RigidBodyFactory = BulletRigidBodyFactory.Instance;
        public RigidBody Body { get; set; }

        public Terrain(string mediaDir, string shadersDir) : base(mediaDir, shadersDir) => Init();

        private void Init()
        {
            FILE_HEIGHTMAPS = "Heightmaps\\suelo.jpg";
            FILE_TEXTURES = "Textures\\sand.jpg";
            FILE_EFFECT = "SmartTerrain.fx";
            Technique = "DiffuseMap";
            LoadWorld();
            Body = RigidBodyFactory.CreateSurfaceFromHeighMap(world.GetVertices());
        }

        public override void Dispose()
        {
            Body.Dispose();
            base.Dispose();
        }
    }
}