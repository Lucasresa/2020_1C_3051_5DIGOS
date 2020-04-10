using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Group.Model.Minerals;
using TGC.Group.Utils;

namespace TGC.Group.Model
{
    class OreBuilder
    {
        public enum OreType
        {
            gold, silver, iron
        }

        private Random random;
        private string MediaDir;
        private IronOre ironOre;
        private SilverOre silverOre;
        private GoldOre goldOre;

        public OreBuilder(string mediaDir)
        {
            MediaDir = mediaDir;
            random = new Random();
            ironOre = new IronOre(MediaDir, TGCVector3.Empty);
            ironOre.LoadMesh();
            goldOre = new GoldOre(MediaDir, TGCVector3.Empty);
            goldOre.LoadMesh();
            silverOre = new SilverOre(MediaDir, TGCVector3.Empty);
            silverOre.LoadMesh();
        }

        public Ore BuildOre(OreType oreType, Tuple<float, float> positionRangeX, Tuple<float, float> positionRangeZ)
        {
            var XMin = (int)positionRangeX.Item1;
            var XMax = (int)positionRangeX.Item2;
            var ZMin = (int)positionRangeZ.Item1;
            var ZMax = (int)positionRangeZ.Item2;

            var XZPosition = new TGCVector3(random.Next(XMin, XMax),
                                           0,
                                           random.Next(ZMin, ZMax));

            Ore newOre;
            switch (oreType)
            {
                case OreType.gold:
                    newOre = new GoldOre(MediaDir, XZPosition)
                    {
                        Mesh = goldOre.Mesh.createMeshInstance(goldOre.Mesh.Name + "1")
                    };
                    break;
                case OreType.silver:
                    newOre = new SilverOre(MediaDir, XZPosition)
                    {
                        Mesh = silverOre.Mesh.createMeshInstance(silverOre.Mesh.Name + "1")
                    };
                    break;
                case OreType.iron:
                    newOre = new IronOre(MediaDir, XZPosition)
                    {
                        Mesh = ironOre.Mesh.createMeshInstance(ironOre.Mesh.Name + "1")
                    };
                    break;
                default:
                    throw new Exception("Unsupported oreType Object");
            }
            return newOre;
        }

        public void LocateMineralsInTerrain(SmartTerrain terrain, List<Ore> minerals)
        {
            minerals.ForEach(mineral =>
            {
                mineral.Init();
                if (terrain.setObjectPosition(mineral.Mesh))
                {
                    terrain.interpoledHeight(mineral.Mesh.Position.X, mineral.Mesh.Position.Z, out float YPosition);
                    mineral.Mesh.Transform = TGCMatrix.Translation(new TGCVector3(mineral.Mesh.Position.X,
                                                                                 YPosition,
                                                                                 mineral.Mesh.Position.Z));
                    terrain.AdaptToSurface(mineral.Mesh);
                }
                else
                    minerals.Remove(mineral);
            });
        }

        public List<Ore> CreateRandomMinerals(int quantity, Tuple<float, float> positionRangeX, Tuple<float, float> positionRangeZ)
        {
            var typesList = new List<OreType>();
            var minerals = new List<Ore>();

            foreach (string name in Enum.GetNames(typeof(OreType)))
                typesList.Add((OreType)Enum.Parse(typeof(OreType), name));

            foreach (int _ in Enumerable.Range(1, quantity))
            {
                minerals.Add(BuildOre(typesList[random.Next(0, typesList.Count)], positionRangeX, positionRangeZ));
            }
            return minerals;
        }
    }
}
