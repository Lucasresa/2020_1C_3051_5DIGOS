using TGC.Core.Mathematica;
using TGC.Group.Utils;
using static TGC.Group.Model.GameModel;

namespace TGC.Group.Model.Objects
{
    abstract class World
    {
        protected string FILE_HEIGHTMAPS, FILE_TEXTURES, FILE_EFFECT;
        protected float SCALEXZ = 300f, SCALEY = 12f;
        private readonly string MediaDir, ShadersDir;
        protected TGCVector3 Position = TGCVector3.Empty;
        public SmartTerrain world = new SmartTerrain();
        protected string Technique;

        public World(string mediaDir, string shadersDir)
        {
            MediaDir = mediaDir;
            ShadersDir = shadersDir;
        }

        public virtual void Render()
        {
            world.Render();
        }

        public virtual void Dispose()
        {
            world.Dispose();
        }

        public virtual void LoadWorld()
        {
            world.loadHeightmap(MediaDir + FILE_HEIGHTMAPS, SCALEXZ, SCALEY, Position);
            world.loadTexture(MediaDir + FILE_TEXTURES);
            world.loadEffect(ShadersDir + FILE_EFFECT, Technique);
        }

        public virtual Perimeter SizeWorld()
        {
            Perimeter perimeter = new Perimeter();

            var sizeX = world.HeightmapData.GetLength(0) * SCALEXZ / 2;
            var sizeZ = world.HeightmapData.GetLength(1) * SCALEXZ / 2;

            perimeter.xMax = sizeX;
            perimeter.xMin = -sizeX;
            perimeter.zMax = sizeZ;
            perimeter.zMin = -sizeZ;

            return perimeter;
        }

    }
}
