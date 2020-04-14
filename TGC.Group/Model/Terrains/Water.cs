using System.Collections.Generic;
using TGC.Core.Mathematica;
using static TGC.Group.Model.Terrains.Terrain;

namespace TGC.Group.Model.Terrains
{
    class Water : World
    {
        public Water(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            FILE_HEIGHTMAPS = "Heightmaps\\oceano.jpg";
            FILE_TEXTURES = "Textures\\agua.jpg";
        }

        public override void LoadWorld(TGCVector3 position)
        {
            SCALEY = 1;
            base.LoadWorld(position);
        }

        public override Dictionary<string, Perimeter> getArea(float fila, float columna)
        {
            throw new System.NotImplementedException();
        }

        public override void splitToArea()
        {
            throw new System.NotImplementedException();
        }

    }
}
