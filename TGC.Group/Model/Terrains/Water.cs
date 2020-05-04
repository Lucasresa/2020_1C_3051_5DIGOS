using TGC.Core.Mathematica;

namespace TGC.Group.Model.Terrains
{
    class Water : World
    {
        public Water(string mediaDir, string shadersDir, TGCVector3 position) : base(mediaDir, shadersDir, position)
        {
            FILE_HEIGHTMAPS = "Heightmaps\\oceano.jpg";
            FILE_TEXTURES = "Textures\\agua.jpg";
            SCALEY = 1;
            LoadWorld();
        }
    }
}
