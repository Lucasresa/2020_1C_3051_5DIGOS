using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Group.Model.Corales;
using TGC.Tools.TerrainEditor;

namespace TGC.Group.Model
{
    public enum CoralType
    {
        normal, spiral, tree
    }

    public enum CoralSize
    {
        small, medium, large
    }

    class CoralBuilder
    {
        private Random random;
        private string MediaDir;
        private TreeCoral treeCoral;
        private SpiralCoral spiralCoral;
        private NormalCoral normalCoral;

        public CoralBuilder(string mediaDir)
        {
            MediaDir = mediaDir;
            random = new Random();
            treeCoral = new TreeCoral(MediaDir, TGCVector3.Empty);
            treeCoral.LoadMesh();
            normalCoral = new NormalCoral(MediaDir, TGCVector3.Empty);
            normalCoral.LoadMesh();
            spiralCoral = new SpiralCoral(MediaDir, TGCVector3.Empty);
            spiralCoral.LoadMesh();
        }
        
        //TODO: Cambiar el vector4 por dos tuplas para que tenga mas sentido
        public Coral BuildCoral(CoralType coralType, TGCVector4 position)
        {
            var XZPosition = new TGCVector3(random.Next((int)position.X, (int)position.Y),
                                           0, 
                                           random.Next((int)position.Z, (int)position.W));

            Coral newCoral;
            switch (coralType)
            {
                case CoralType.normal:
                    newCoral = new NormalCoral(MediaDir, XZPosition)
                    {
                        Mesh = normalCoral.Mesh.createMeshInstance(normalCoral.Mesh.Name + "1")
                    };
                    break;
                case CoralType.spiral:
                    newCoral = new SpiralCoral(MediaDir, XZPosition)
                    {
                        Mesh = spiralCoral.Mesh.createMeshInstance(spiralCoral.Mesh.Name + "1")
                    };
                    break;
                case CoralType.tree:
                    newCoral = new TreeCoral(MediaDir, XZPosition)
                    {
                        Mesh = treeCoral.Mesh.createMeshInstance(treeCoral.Mesh.Name + "1")
                    };                    
                    break;
                default:
                    throw new Exception("Unsupported coralType Object");
            }
            return newCoral;
        }
        // return bool interpoledHeight(float x, float z, out float y)
        public void LocateCoralsInTerrain(SmartTerrain terrain, List<Coral> corals)
        {
            corals.ForEach(coral =>
            {
                coral.Init();
                if (terrain.setObjectPosition(coral.Mesh))
                {
                    terrain.interpoledHeight(coral.Mesh.Position.X, coral.Mesh.Position.Z, out float YPosition);
                    coral.Mesh.Transform = TGCMatrix.Translation(new TGCVector3 (coral.Mesh.Position.X,
                                                                                 YPosition,
                                                                                 coral.Mesh.Position.Z ));
                    terrain.AdaptToSurface(coral.Mesh);
                }
                else
                    corals.Remove(coral);
            });
        }

        public List<Coral> CreateRandomCorals(int quantity, TGCVector4 positionRange)
        {
            var typesList = new List<CoralType>();
            var corals = new List<Coral>();

            foreach ( string name in CoralType.GetNames(typeof(CoralType)) ) 
                typesList.Add((CoralType)Enum.Parse(typeof(CoralType), name));

            foreach ( int _ in Enumerable.Range(1, quantity) ) 
            {
                corals.Add( BuildCoral(typesList[random.Next(0, typesList.Count)] , positionRange) );
            }
            return corals;
        }

    }
}
