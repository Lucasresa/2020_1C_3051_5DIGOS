using TGC.Core.Mathematica;

namespace TGC.Group.Model.Terrains
{
    class Water : World
    {
        private TGCVector3 position = new TGCVector3(0, 3500, 0);

        public Water(string mediaDir, string shadersDir, TGCVector3 position) : base(mediaDir, shadersDir, position)
        {
            Position = this.position;
            FILE_HEIGHTMAPS = "Heightmaps\\oceano.jpg";
            FILE_TEXTURES = "Textures\\agua.jpg";
            SCALEY = 1;
            LoadWorld();
        }
    }
}
