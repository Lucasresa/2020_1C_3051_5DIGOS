using System;
using TGC.Core.Mathematica;
using TGC.Core.Textures;

namespace TGC.Group.Model.Objects
{
    internal class Water : World
    {
        private TGCVector3 waterPosition = new TGCVector3(0, 3500, 0);

        public Water(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Position = waterPosition;
            FILE_HEIGHTMAPS = @"Heightmaps\oceano.jpg";
            FILE_TEXTURES = @"Textures\water.png";
            FILE_EFFECT = "SmartTerrain.fx";
            Technique = "Olas";
            SCALEY = 1;
            LoadWorld();
        }

        public void Update(float elapsedTime) => world.SetTimeForWaves(elapsedTime);
    }
}
