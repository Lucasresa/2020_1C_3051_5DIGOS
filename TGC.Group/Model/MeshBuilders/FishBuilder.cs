using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Group.Model.Fishes;
using TGC.Group.Model.Minerals;
using TGC.Group.Utils;

namespace TGC.Group.Model
{
    class FishBuilder
    {
        private Random random;
        private string MediaDir;
        private Fish OriginalNormalFish;
        private Fish OriginalYellowFish;
        public FishBuilder(string mediaDir)
        {
            MediaDir = mediaDir;
            random = new Random();
            OriginalNormalFish = new Fish(MediaDir, TGCVector3.Empty, "fish");
            OriginalNormalFish.LoadMesh();
            OriginalYellowFish = new Fish(MediaDir, TGCVector3.Empty, "yellow_fish");
            OriginalYellowFish.LoadMesh();
        }

        public Fish BuildFish(string fishType, Tuple<float, float> positionRangeX, Tuple<float, float> positionRangeZ)
        {
            var XMin = (int)positionRangeX.Item1;
            var XMax = (int)positionRangeX.Item2;
            var ZMin = (int)positionRangeZ.Item1;
            var ZMax = (int)positionRangeZ.Item2;

            var XZPosition = new TGCVector3(random.Next(XMin, XMax),
                                           0,
                                           random.Next(ZMin, ZMax));

            Fish newFish;
            switch (fishType)
            {
                case "fish":
                    newFish = new Fish(MediaDir, XZPosition, fishType)
                    {
                        Mesh = OriginalNormalFish.Mesh.createMeshInstance(OriginalNormalFish.Mesh.Name + "Normal1")
                    };
                    break;
                case "yellow_fish":
                    newFish = new Fish(MediaDir, XZPosition, fishType)
                    {
                        Mesh = OriginalYellowFish.Mesh.createMeshInstance(OriginalYellowFish.Mesh.Name + "Yellow1")
                    };
                    break;
                default:
                    throw new Exception("Unsupported fishType Object");
            }
            return newFish;
        }

        public void LocateFishesInTerrain(SmartTerrain terrain, List<Fish> fishes, float waterHeight)
        {
            fishes.ForEach(fish =>
            {
                fish.Init();
                if ( terrain.interpoledHeight(fish.Mesh.Position.X, fish.Mesh.Position.Z, out float YPosition) )
                {
                    var fishSize = (int)fish.Mesh.Scale.Y;
                    fish.Mesh.Position = new TGCVector3 (fish.Mesh.Position.X,
                                                         random.Next((int)YPosition + fishSize, (int)waterHeight - fishSize*2), 
                                                         fish.Mesh.Position.Z);
                }
                else
                    fishes.Remove(fish);
            });
        }

        public List<Fish> CreateRandomFishes(int quantity, Tuple<float, float> positionRangeX, Tuple<float, float> positionRangeZ)
        {
            var typesList = new List<string>() { "fish", "yellow_fish" };
            var minerals = new List<Fish>();

            foreach (int _ in Enumerable.Range(1, quantity))
            {
                minerals.Add(BuildFish(typesList[random.Next(0, typesList.Count)], positionRangeX, positionRangeZ));
            }
            return minerals;
        }
    }
}
