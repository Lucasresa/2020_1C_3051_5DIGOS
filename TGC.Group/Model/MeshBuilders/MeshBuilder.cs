using System;
using System.Collections.Generic;
using System.Linq;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Utils;
using static TGC.Group.Model.GameModel;

namespace TGC.Group.Model.MeshBuilders
{
    class MeshBuilder
    {
        #region Atributos
        private struct Constants
        {
            public static int meshTerrainOffset = 300;
            public static int maxYPosition = 200;
            public static TGCVector3 scale = new TGCVector3(10, 10, 10);
            public static TGCVector3 scale_vegetation = new TGCVector3(7, 7, 7);
        }
        private Random random;
        List<MeshType> vegetation = new List<MeshType>();
        List<MeshType> fishes = new List<MeshType>();
        #endregion

        #region Constructor
        public MeshBuilder()
        {
            random = new Random();
        }
        #endregion

        #region Metodos

        #region MeshCreation
        public TgcMesh CreateNewMeshCopy(MeshType meshType)
        {
            return MeshDuplicator.GetDuplicateMesh(meshType);
        }

        public TgcMesh CreateNewScaledMesh(MeshType meshType)
        {
            var scaledMesh = CreateNewMeshCopy(meshType);

            if (isVegetation(meshType))
                scaledMesh.Transform = TGCMatrix.Scaling(Constants.scale_vegetation);
            else
                scaledMesh.Transform = TGCMatrix.Scaling(Constants.scale);

            return scaledMesh;
        }

        public List<TgcMesh> CreateNewScaledMeshes(MeshType meshType, int quantity)
        {
            var meshes = new List<TgcMesh>();
            foreach (int _ in Enumerable.Range(1, quantity))
                meshes.Add(CreateNewScaledMesh(meshType));
            return meshes;
        }
        #endregion

        #region Location       
        private bool LocateMeshInWorld(MeshType type, ref TgcMesh mesh, Perimeter terrainArea, SmartTerrain terrain, SmartTerrain water)
        {
            
            var pairXZ = getXZPositionByPerimeter(terrainArea);

            if (!terrain.interpoledHeight(pairXZ.XPosition, pairXZ.ZPosition, out float YPosition))
                throw new Exception("The Mesh: " + mesh.Name + " calculated position was outside of terrain");

            if (isFish(type))
                locateFish(ref mesh, pairXZ, (int)water.Center.Y, YPosition);
            else
                locateMeshesTypeTerrain(ref mesh, pairXZ, terrain, YPosition);
            
            return true;
        }

        public void LocateMeshesInWorld(MeshType type, ref List<TgcMesh> meshes, Perimeter terrainArea, SmartTerrain terrain, SmartTerrain water)
        {
            meshes.ForEach(mesh => LocateMeshInWorld(type, ref mesh, terrainArea, terrain, water));
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

        private TGCVector3 calculateRotation(TGCVector3 normalObjeto)
        {
            var objectInclinationX = FastMath.Atan2(normalObjeto.X, normalObjeto.Y);
            var objectInclinationZ = FastMath.Atan2(normalObjeto.X, normalObjeto.Y);
            var rotation = new TGCVector3(-objectInclinationX, 0, -objectInclinationZ);
            return rotation;
        }

        private void locateMeshesTypeTerrain(ref TgcMesh mesh, (int XPosition, int ZPosition) pairXZ, SmartTerrain terrain ,float YPosition)
        {
            var position = new TGCVector3(pairXZ.XPosition, YPosition, pairXZ.ZPosition);
            var rotation = calculateRotation(terrain.NormalVectorGivenXZ(position.X, position.Z));
            mesh.Transform *= TGCMatrix.RotationYawPitchRoll(rotation.Y, rotation.X, rotation.Z) * TGCMatrix.Translation(position);
            mesh.Position = position;
        }

        private void locateFish(ref TgcMesh mesh, (int XPosition, int ZPosition) pairXZ, int heightWater, float YPosition)
        {
            YPosition = random.Next((int)YPosition + Constants.meshTerrainOffset, heightWater - Constants.maxYPosition);
            var position = new TGCVector3(pairXZ.XPosition, YPosition, pairXZ.ZPosition);
            mesh.Transform *= TGCMatrix.Translation(pairXZ.XPosition, YPosition, pairXZ.ZPosition);
            mesh.Position = position;
        }

        private bool isVegetation(MeshType type)
        {            
            vegetation.Add(MeshType.alga);
            vegetation.Add(MeshType.alga_2);
            vegetation.Add(MeshType.alga_3);
            vegetation.Add(MeshType.alga_4);
            return vegetation.Contains(type);
        }

        private bool isFish(MeshType type)
        {           
            fishes.Add(MeshType.normalFish);
            fishes.Add(MeshType.yellowFish);
            return fishes.Contains(type);
        }
        #endregion

        #endregion
    }
}
