using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Corales;
using TGC.Group.Model.Minerals;
using TGC.Group.Model.Vegetation;

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

            Meshes.Add(MeshType.normalCoral, new NormalCoral(MediaDir, TGCVector3.Empty, "normal_coral"));
            Meshes.Add(MeshType.treeCoral, new TreeCoral(MediaDir, TGCVector3.Empty, "tree_coral"));
            Meshes.Add(MeshType.spiralCoral, new SpiralCoral(MediaDir, TGCVector3.Empty, "spiral_coral"));
            Meshes.Add(MeshType.ironOre, new IronOre(MediaDir, TGCVector3.Empty, "iron"));
            Meshes.Add(MeshType.silverOre, new SilverOre(MediaDir, TGCVector3.Empty, "silver"));
            Meshes.Add(MeshType.goldOre, new GoldOre(MediaDir, TGCVector3.Empty, "gold"));
            Meshes.Add(MeshType.ironOreCommon, new IronOreCommon(MediaDir, TGCVector3.Empty, "iron-n"));
            Meshes.Add(MeshType.silverOreCommon, new SilverOreCommon(MediaDir, TGCVector3.Empty, "silver-n"));
            Meshes.Add(MeshType.goldOreCommon, new GoldOreCommon(MediaDir, TGCVector3.Empty, "gold-n"));
            Meshes.Add(MeshType.rock, new Rock(MediaDir, TGCVector3.Empty, "rock-n"));
            Meshes.Add(MeshType.alga, new Alga(MediaDir, TGCVector3.Empty, "alga"));
            Meshes.Add(MeshType.algaRed, new AlgaRed(MediaDir, TGCVector3.Empty, "algaRed"));
            
        }

        public static TgcMesh GetDuplicateMesh(MeshType meshType)
        {
            var originalMesh = Meshes[meshType].Mesh;
            return originalMesh.createMeshInstance(originalMesh.Name + MeshCounter++);
        }

    }
}
