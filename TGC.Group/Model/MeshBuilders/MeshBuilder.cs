using System;
using System.Collections.Generic;
using System.Linq;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Utils;

namespace TGC.Group.Model.MeshBuilders
{
    class MeshBuilder
    {
        private Random random;
        private int meshTerrainOffset = 300;

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

        public TgcMesh CreateNewScaledMesh(MeshType meshType, float scale)
        {
            var scaledMesh = CreateNewMeshCopy(meshType);
            scaledMesh.Scale = new TGCVector3(scale, scale, scale);
            return scaledMesh;
        }

        public List<TgcMesh> CreateNewScaledMeshes(MeshType meshType, int quantity, float scale = 1)
        {
            var meshes = new List<TgcMesh>();
            foreach (int _ in Enumerable.Range(1, quantity))
                meshes.Add( CreateNewScaledMesh(meshType, scale) );
            return meshes;
        }

        #endregion

        #region Location
        public bool LocateMeshInTerrain(ref TgcMesh mesh, Tuple<float, float> positionRangeX, Tuple<float, float> positionRangeZ,
                                 SmartTerrain terrain)
        {
            var XZPosition = getXZPositionByRange(positionRangeX, positionRangeZ);
            var XPosition = XZPosition.Item1;
            var ZPosition = XZPosition.Item2;
            if (!terrain.interpoledHeight(XPosition, ZPosition, out float YPosition))
                throw new Exception("The Mesh: " + mesh.Name + " calculated position was outside of terrain");

            mesh.Position = new TGCVector3(XPosition, YPosition, ZPosition);
            terrain.AdaptToSurface(mesh);
            return true;
        }

        public bool LocateMeshUpToTerrain(ref TgcMesh mesh, Tuple<float, float> positionRangeX, Tuple<float, float> positionRangeZ,
                                 SmartTerrain terrain, float maxYPosition)
        {
            var XZPosition = getXZPositionByRange(positionRangeX, positionRangeZ);
            var XPosition = XZPosition.Item1;
            var ZPosition = XZPosition.Item2;
            if (!terrain.interpoledHeight(XPosition, ZPosition, out float YPosition))
                throw new Exception("The Mesh: " + mesh.Name + " calculated position was outside of terrain");

            YPosition = random.Next((int)YPosition + meshTerrainOffset, (int)maxYPosition);
            mesh.Position = new TGCVector3(XPosition, YPosition, ZPosition);
            return true;
        }

        public void LocateMeshesInTerrain(ref List<TgcMesh> meshes, Tuple<float, float> positionRangeX, Tuple<float, float> positionRangeZ,
                                             SmartTerrain terrain)
        {
            meshes.ForEach(mesh => LocateMeshInTerrain(ref mesh, positionRangeX, positionRangeZ, terrain));
        }

        public void LocateMeshesUpToTerrain(ref List<TgcMesh> meshes, Tuple<float, float> positionRangeX, Tuple<float, float> positionRangeZ,
                                             SmartTerrain terrain, float maxYPosition)
        {
            meshes.ForEach(mesh => LocateMeshUpToTerrain(ref mesh, positionRangeX, positionRangeZ, terrain, maxYPosition));
        }
        #endregion

        #region Utils
        // Retorna una tupla con el valor de X y Z (X, Z)
        private Tuple<float, float> getXZPositionByRange(Tuple<float, float> positionRangeX,
                                                        Tuple<float, float> positionRangeZ)
        {
            var XMin = (int)positionRangeX.Item1;
            var XMax = (int)positionRangeX.Item2;
            var ZMin = (int)positionRangeZ.Item1;
            var ZMax = (int)positionRangeZ.Item2;

            return new Tuple<float, float>(random.Next(XMin, XMax), random.Next(ZMin, ZMax));
        }

        

        #endregion
    }
}
