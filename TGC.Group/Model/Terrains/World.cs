using TGC.Core.Mathematica;
using TGC.Group.Utils;
using static TGC.Group.Model.Terrains.Terrain;

namespace TGC.Group.Model.Terrains
{
    abstract class World
    {
        protected string FILE_HEIGHTMAPS;
        protected string FILE_TEXTURES;

        protected float SCALEXZ = 100f;
        protected float SCALEY = 10f;

        public SmartTerrain world = new SmartTerrain();

        private string MediaDir;
        private string ShadersDir;
        protected TGCVector3 Position;

        public World(string mediaDir, string shadersDir, TGCVector3 position)
        {
            MediaDir = mediaDir;
            ShadersDir = mediaDir;
            Position = position;
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
