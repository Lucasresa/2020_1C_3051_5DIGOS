using System.Collections.Generic;
using TGC.Core.Mathematica;
using static TGC.Group.Model.Terrains.Terrain;

namespace TGC.Group.Model.Terrains
{
    class Water : World
    {
        //Factor necesario para que el agua tenga el mismo tamanio que el terreno
        const float FACTOR = 0.5859375f;
        public Water(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            FILE_HEIGHTMAPS = "Heightmaps\\oceano.jpg";
            FILE_TEXTURES = "Textures\\agua.jpg";
        }

        public override void LoadWorld(TGCVector3 position)
        {
            SCALEY = 1;
            SCALEXZ *= FACTOR;
            base.LoadWorld(position);
        }
    }
}
