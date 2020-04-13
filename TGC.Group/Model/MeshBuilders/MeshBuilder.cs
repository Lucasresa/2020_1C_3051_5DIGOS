using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Corales;
using TGC.Group.Utils;

namespace TGC.Group.Model.MeshBuilders
{
    class MeshBuilder
    {
        private Random random;
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
            var MeshPosition = new TGCVector3(XPosition, YPosition, ZPosition);
            mesh.Position = MeshPosition;
            terrain.AdaptToSurface(mesh);
            return true;
        }

        public void LocateMeshesInTerrain(ref List<TgcMesh> meshes, Tuple<float, float> positionRangeX, Tuple<float, float> positionRangeZ,
                                             SmartTerrain terrain)
        {
            meshes.ForEach(mesh => LocateMeshInTerrain(ref mesh, positionRangeX, positionRangeZ, terrain));
        }

        #endregion

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

    }
}
