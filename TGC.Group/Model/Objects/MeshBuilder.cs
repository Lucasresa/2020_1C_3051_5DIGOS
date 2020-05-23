using System;
using System.Collections.Generic;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Utils;
using static TGC.Group.Model.GameModel;
using static TGC.Group.Model.Objects.Common;
using static TGC.Group.Model.Objects.Fish;
using static TGC.Group.Model.Objects.Vegetation;
namespace TGC.Group.Model.Objects
{
    class MeshBuilder
    {
        private struct Constants
        {
            public static int meshTerrainOffset = 300;
            public static int maxYPosition = 200;
        }

        private Random random;
        private Terrain Terrain;
        private Water Water;

        public MeshBuilder(Terrain terrain, Water water)
        {
            random = new Random();
            Terrain = terrain;
            Water = water;
        }

        public void LocateMeshInWorld(ref TgcMesh mesh, Perimeter area)
        {
            var pairXZ = getXZPositionByPerimeter(area);
            Terrain.world.interpoledHeight(pairXZ.XPosition, pairXZ.ZPosition, out float YPosition);

            if (IsFish(mesh.Name))
                LocateFish(ref mesh, pairXZ, (int)Water.world.Center.Y, YPosition);
            else
                LocateMeshesTypeTerrain(ref mesh, pairXZ, Terrain.world, YPosition);
        }

        public void LocateMeshesInWorld(ref List<TypeCommon> meshes, Perimeter area)
        {
            meshes.ForEach(common => LocateMeshInWorld(ref common.mesh, area));
        }

        public void LocateMeshesInWorld(ref List<TypeFish> meshes, Perimeter area)
        {
            meshes.ForEach(fish => LocateMeshInWorld(ref fish.mesh, area));
        }

        public void LocateMeshesInWorld(ref List<TypeVegetation> meshes, Perimeter area)
        {
            meshes.ForEach(vegetation => LocateMeshInWorld(ref vegetation.mesh, area));
        }

        private (int XPosition, int ZPosition) getXZPositionByPerimeter(Perimeter perimeter)
        {
            var XMin = (int)perimeter.xMin;
            var XMax = (int)perimeter.xMax;
            var ZMin = (int)perimeter.zMin;
            var ZMax = (int)perimeter.zMax;

            var xPosition = random.Next(XMin, XMax);
            var zPosition = random.Next(ZMin, ZMax);

            return (XPosition: xPosition, ZPosition: zPosition);
        }

        private TGCVector3 CalculateRotation(TGCVector3 normalObjeto)
        {
            var objectInclinationX = FastMath.Atan2(normalObjeto.X, normalObjeto.Y);
            var objectInclinationZ = FastMath.Atan2(normalObjeto.X, normalObjeto.Y);
            var rotation = new TGCVector3(-objectInclinationX, 0, -objectInclinationZ);
            return rotation;
        }

        private void LocateMeshesTypeTerrain(ref TgcMesh mesh, (int XPosition, int ZPosition) pairXZ, SmartTerrain terrain, float YPosition)
        {
            var position = new TGCVector3(pairXZ.XPosition, YPosition, pairXZ.ZPosition);
            var rotation = CalculateRotation(terrain.NormalVectorGivenXZ(position.X, position.Z));
            mesh.Transform *= TGCMatrix.RotationYawPitchRoll(rotation.Y, rotation.X, rotation.Z) * TGCMatrix.Translation(position);
            mesh.Position = position;
        }

        private void LocateFish(ref TgcMesh mesh, (int XPosition, int ZPosition) pairXZ, int heightWater, float YPosition)
        {
            YPosition = random.Next((int)YPosition + Constants.meshTerrainOffset, heightWater - Constants.maxYPosition);
            var position = new TGCVector3(pairXZ.XPosition, YPosition, pairXZ.ZPosition);
            mesh.Transform *= TGCMatrix.Translation(pairXZ.XPosition, YPosition, pairXZ.ZPosition);
            mesh.Position = position;
        }

        private bool IsFish(string name)
        {
            return FastUtils.Contains(name, "fish");
        }
    }
}
