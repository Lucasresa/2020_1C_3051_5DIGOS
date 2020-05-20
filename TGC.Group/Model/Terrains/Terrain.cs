using System.IO;

namespace TGC.Group.Model.Terrains
{
    class Terrain : World
    {
        public Terrain(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            FILE_HEIGHTMAPS = "Heightmaps\\suelo.jpg";
            FILE_TEXTURES = "Textures\\sandy.png";
            FILE_EFFECT = "TerrainShader.fx";
            LoadWorld();
        }
    }
}