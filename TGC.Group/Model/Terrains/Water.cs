using TGC.Core.Mathematica;

namespace TGC.Group.Model.Terrains
{
    class Water : World
    {
        private TGCVector3 waterPosition = new TGCVector3(0, 3500, 0);

        public Water(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Position = waterPosition;
            FILE_HEIGHTMAPS = @"Heightmaps\oceano.jpg";
            FILE_TEXTURES = @"Textures\agua.jpg";
            SCALEY = 1;
            LoadWorld();
        }
    }
}
