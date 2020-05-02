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
            if (meshType == MeshType.alga) scaledMesh.AlphaBlendEnable = true;
            //TODO: ESTO QUE ESTOY AGREGANDO ES PARA QUE PUEDA PROBAR LOS CUERPOS RIGIDOS, SI LO DEJAMOS NOS VAN A CAGAR A PEDO
            scaledMesh.Transform = TGCMatrix.Scaling(scale, scale, scale);
            scaledMesh.Scale = new TGCVector3(scale, scale, scale);
            return scaledMesh;
        }

        public List<TgcMesh> CreateNewScaledMeshes(MeshType meshType, int quantity, float scale = 1)
        {
            var meshes = new List<TgcMesh>();
            foreach (int _ in Enumerable.Range(1, quantity))
                meshes.Add(CreateNewScaledMesh(meshType, scale));
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
            //TODO: ESTO QUE ESTOY AGREGANDO ES PARA QUE PUEDA PROBAR LOS CUERPOS RIGIDOS, SI LO DEJAMOS NOS VAN A CAGAR A PEDO
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
