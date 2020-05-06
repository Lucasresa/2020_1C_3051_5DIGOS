using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Utils;
using static TGC.Group.Model.Terrains.Terrain;

namespace TGC.Group.Model.MeshBuilders
{
    class MeshBuilder
    {
        private Random random;
        private int meshTerrainOffset = 300;

        private TGCVector3 scale = new TGCVector3(10, 10, 10);
        private TGCVector3 scale_vegetation = new TGCVector3(7, 7, 7);

        public int MeshTerrainOffset { set { meshTerrainOffset = value; } }

        public MeshBuilder()
        {
            random = new Random();
        }

        #region MeshCreation
        public TgcMesh CreateNewMeshCopy(MeshType meshType)
        {
            return MeshDuplicator.GetDuplicateMesh(meshType);
        }

        public TgcMesh CreateNewScaledMesh(MeshType meshType)
        {
            var scaledMesh = CreateNewMeshCopy(meshType);
            if(isVegetation(meshType))
                scaledMesh.Transform = TGCMatrix.Scaling(scale_vegetation);
            else
                scaledMesh.Transform = TGCMatrix.Scaling(scale);
            return scaledMesh;
        }

        private bool isVegetation(MeshType meshType)
        {
            return ( meshType == MeshType.alga || meshType == MeshType.alga_2 || 
                     meshType == MeshType.alga_3 || meshType == MeshType.alga_4 );
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
        public bool LocateMeshInTerrain(ref TgcMesh mesh, Perimeter terrainArea,
                                 SmartTerrain terrain)
        {
            var XZPosition = getXZPositionByPerimeter(terrainArea);
            var XPosition = XZPosition.Item1;
            var ZPosition = XZPosition.Item2;

            if (!terrain.interpoledHeight(XPosition, ZPosition, out float YPosition))
                throw new Exception("The Mesh: " + mesh.Name + " calculated position was outside of terrain");

            var position = new TGCVector3(XPosition, YPosition, ZPosition);
            var normalObjeto = terrain.NormalVectorGivenXZ(position.X, position.Z);
            var rotation = calculatedRotation(normalObjeto);

            mesh.Transform *= TGCMatrix.RotationYawPitchRoll(rotation.Y, rotation.X, rotation.Z) * TGCMatrix.Translation(position);
            mesh.Position = position;
            return true;
        }

        public bool LocateMeshUpToTerrain(ref TgcMesh mesh, Perimeter terrainArea,
                                 SmartTerrain terrain, float maxYPosition)
        {
            var XZPosition = getXZPositionByPerimeter(terrainArea);
            var XPosition = XZPosition.Item1;
            var ZPosition = XZPosition.Item2;

            if (!terrain.interpoledHeight(XPosition, ZPosition, out float YPosition))
                throw new Exception("The Mesh: " + mesh.Name + " calculated position was outside of terrain");

            YPosition = random.Next((int)YPosition + meshTerrainOffset, (int)maxYPosition);
            mesh.Transform *= TGCMatrix.Translation(XPosition, YPosition, ZPosition);
            var position = new TGCVector3(XPosition, YPosition, ZPosition);
            mesh.Position = position;
            return true;
        }

        public void LocateMeshesInTerrain(ref List<TgcMesh> meshes, Perimeter terrainArea,
                                             SmartTerrain terrain)
        {
            meshes.ForEach(mesh => LocateMeshInTerrain(ref mesh, terrainArea, terrain));
        }

        public void LocateMeshesUpToTerrain(ref List<TgcMesh> meshes, Perimeter terrainArea,
                                             SmartTerrain terrain, float maxYPosition)
        {
            meshes.ForEach(mesh => LocateMeshUpToTerrain(ref mesh, terrainArea, terrain, maxYPosition));
        }
        #endregion

        #region Utils
        // Retorna una tupla con el valor de X y Z (X, Z)
        private Tuple<float, float> getXZPositionByPerimeter(Perimeter perimeter)
        {
            var XMin = (int)perimeter.xMin;
            var XMax = (int)perimeter.xMax;
            var ZMin = (int)perimeter.zMin;
            var ZMax = (int)perimeter.zMax;

            return new Tuple<float, float>(random.Next(XMin, XMax), random.Next(ZMin, ZMax));
        }

        private TGCVector3 calculatedRotation(TGCVector3 normalObjeto)
        {
            var objectInclinationX = FastMath.Atan2(normalObjeto.X, normalObjeto.Y);
            var objectInclinationZ = FastMath.Atan2(normalObjeto.X, normalObjeto.Y);

            float rotationX = -objectInclinationX;
            float rotationZ = -objectInclinationZ;

            return new TGCVector3(rotationX, 0, rotationZ);
        }
        #endregion
    }
}
