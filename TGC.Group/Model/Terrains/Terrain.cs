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

        Perimeter quadrate = new Perimeter();

        private readonly float rows = 3;
        private readonly float columns = 3;

        public Dictionary<string, Perimeter> areas = new Dictionary<string, Perimeter>();

        public Terrain(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            FILE_HEIGHTMAPS = "Heightmaps\\suelo.jpg";
            FILE_TEXTURES = "Textures\\sandy.png";            
        }

        public Dictionary<string, Perimeter> getArea(float posX, float posZ)
        {
            IEnumerable<KeyValuePair<string, Perimeter>> area = 
            areas.Where(pair => pair.Value.xMin < posX && posX < pair.Value.xMax &&
                                pair.Value.zMin < posZ && posZ < pair.Value.zMax
                       );

            return area.ToDictionary(x => x.Key, x => x.Value);            
        }

        public void splitToArea()
        {
            int sideX = world.HeightmapData.GetLength(0);
            int sideZ = world.HeightmapData.GetLength(0);

            for (int row = 1; row <= rows; row++)
            {
                quadrate.xMin = world.convertToWorld((row - 1) * sideX / rows);
                quadrate.xMax = world.convertToWorld(row * sideX / rows);

                for (int column = 1; column <= columns; column++)
                {
                    quadrate.zMin = world.convertToWorld((column - 1) * sideZ / columns);
                    quadrate.zMax = world.convertToWorld(column * sideZ / columns);
                    areas.Add("Area" + row.ToString() + column.ToString(), quadrate);
                }
            }
        }
    }
}