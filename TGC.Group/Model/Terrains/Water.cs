using TGC.Core.Mathematica;

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
    }
}
