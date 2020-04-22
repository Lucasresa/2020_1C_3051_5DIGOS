using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.MeshBuilders
{
    static class MeshDuplicator
    {

        private static Dictionary<MeshType, CommonMesh> Meshes = new Dictionary<MeshType, CommonMesh>();
        public static string MediaDir;
        private static int MeshCounter = 0;
        public static void InitOriginalMeshes()
        {
            if (MediaDir == null)
            {
                throw new Exception("MediaDir variable is null, set a value first");
            }

            Meshes.Add(MeshType.normalCoral, new CommonMesh(MediaDir, TGCVector3.Empty, "normal_coral"));
            Meshes.Add(MeshType.treeCoral, new CommonMesh(MediaDir, TGCVector3.Empty, "tree_coral"));
            Meshes.Add(MeshType.spiralCoral, new CommonMesh(MediaDir, TGCVector3.Empty, "spiral_coral"));
            Meshes.Add(MeshType.ironOre, new CommonMesh(MediaDir, TGCVector3.Empty, "iron"));
            Meshes.Add(MeshType.silverOre, new CommonMesh(MediaDir, TGCVector3.Empty, "silver"));
            Meshes.Add(MeshType.goldOre, new CommonMesh(MediaDir, TGCVector3.Empty, "gold"));
            Meshes.Add(MeshType.ironOreCommon, new CommonMesh(MediaDir, TGCVector3.Empty, "iron-n"));
            Meshes.Add(MeshType.silverOreCommon, new CommonMesh(MediaDir, TGCVector3.Empty, "silver-n"));
            Meshes.Add(MeshType.goldOreCommon, new CommonMesh(MediaDir, TGCVector3.Empty, "gold-n"));
            Meshes.Add(MeshType.rock, new CommonMesh(MediaDir, TGCVector3.Empty, "rock-n"));
            Meshes.Add(MeshType.alga, new CommonMesh(MediaDir, TGCVector3.Empty, "alga"));           
            Meshes.Add(MeshType.normalFish, new CommonMesh(MediaDir, TGCVector3.Empty, "fish"));           
            Meshes.Add(MeshType.yellowFish, new CommonMesh(MediaDir, TGCVector3.Empty, "yellow_fish"));           
        }

        public static TgcMesh GetDuplicateMesh(MeshType meshType)
        {
            var originalMesh = Meshes[meshType].Mesh;
            return originalMesh.createMeshInstance(originalMesh.Name + MeshCounter++);
        }

        public static void DisposeOriginalMeshes() { foreach (CommonMesh c in Meshes.Values) c.Dispose(); }

    }
}
