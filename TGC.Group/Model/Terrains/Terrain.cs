using System.Collections.Generic;
using System.Linq;

namespace TGC.Group.Model.Terrains
{
    class Terrain : World
    {
        public struct Perimeter
        {
            public float xMin;
            public float xMax;
            public float zMin;
            public float zMax;
        }

        Perimeter square = new Perimeter();

        private readonly float ROWS = 4;
        private readonly float COLUMNS = 4;

        public Dictionary<string, Perimeter> areas = new Dictionary<string, Perimeter>();

        public Terrain(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            FILE_HEIGHTMAPS = "Heightmaps\\suelo.jpg";
            FILE_TEXTURES = "Textures\\sandy.png";            
        }

        public override Perimeter getArea(float posX, float posZ)
        {
            return areas.FirstOrDefault(pair => posX > pair.Value.xMin && posX < pair.Value.xMax &&
                                                posZ > pair.Value.zMin && posZ < pair.Value.zMax)
                                        .Value;           
        }

        public override void splitToArea()
        {
            int sideX = world.HeightmapData.GetLength(0);
            int sideZ = world.HeightmapData.GetLength(1);

            for (int row = 1; row <= ROWS; row++)
            {
                square.xMin = world.convertToWorld((row - 1) * sideX / ROWS);
                square.xMax = world.convertToWorld(row * sideX / ROWS);

                for (int column = 1; column <= COLUMNS; column++)
                {
                    square.zMin = world.convertToWorld((column - 1) * sideZ / COLUMNS);
                    square.zMax = world.convertToWorld(column * sideZ / COLUMNS);
                    areas.Add("Area" + row.ToString() + column.ToString(), square);
                }
            }
        }
    }
}