using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Group.Utils;
using static TGC.Group.Model.Terrains.Terrain;

namespace TGC.Group.Model.Terrains
{
    abstract class World
    {
        protected string FILE_HEIGHTMAPS;
        protected string FILE_TEXTURES;

        protected float SCALEXZ = 20f;
        protected float SCALEY = 8.4f;

        public SmartTerrain world = new SmartTerrain();

        private string MediaDir;
        private string ShadersDir;
        
        public World(string mediaDir, string shadersDir)
        {
            MediaDir = mediaDir;
            ShadersDir = mediaDir;
        }

        public virtual void Render()
        {
            world.render();
        }

        public virtual void Dispose()
        {
            world.dispose();
        }

        public virtual void LoadWorld(TGCVector3 position)
        {
            world.loadHeightmap(MediaDir + FILE_HEIGHTMAPS, SCALEXZ, SCALEY, position);
            world.loadTexture(MediaDir + FILE_TEXTURES);
        }

        public virtual Tuple<float,float> SizeWorld()
        {
            var sizeX = world.HeightmapData.GetLength(0) * SCALEXZ / 2;
            var sizeZ = world.HeightmapData.GetLength(1) * SCALEXZ / 2;

            return new Tuple<float, float>(sizeX, sizeZ);
        }
    }
}
