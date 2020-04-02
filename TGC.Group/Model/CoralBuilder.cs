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

        public CoralBuilder(string mediaDir)
        {
            MediaDir = mediaDir;
            random = new Random();
        }
        
        //TODO: Cambiar el vector4 por dos tuplas para que tenga mas sentido
        public Coral BuildCoral(CoralType coralType, TGCVector4 position)
        {
            var XZPosition = new TGCVector3(random.Next((int)position.X, (int)position.Y),
                                           0, 
                                           random.Next((int)position.Z, (int)position.W));

            switch (coralType)
            {
                case CoralType.normal:
                    return new NormalCoral(MediaDir, XZPosition);
                case CoralType.spiral:
                    return new SpiralCoral(MediaDir, XZPosition);
                case CoralType.tree:
                    return new TreeCoral(MediaDir, XZPosition);
                default:
                    throw new Exception("Unsupported coralType Object");
            }
        }

        public List<Coral> BuildCorals(SmartTerrain terrain, int objectsQuantity, TGCVector4 positionRange)
        {
            var corals = CreateRandomCorals(objectsQuantity, positionRange);

            corals.ForEach(coral => {
                coral.Init();
                if ( terrain.setObjectPosition(coral.Mesh) )
                    terrain.AdaptToSurface(coral.Mesh);
                else
                    corals.Remove(coral);
            });
            return corals;
        }

        private List<Coral> CreateRandomCorals(int quantity, TGCVector4 positionRange)
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
