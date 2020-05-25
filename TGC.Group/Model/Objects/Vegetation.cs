using System.Collections.Generic;
using System.Linq;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Objects
{
    class Vegetation
    {
        public struct TypeVegetation
        {
            public int ID;
            public string name;
            public TgcMesh mesh;
        }

        struct Constants
        {
            public static string NAME_ALGA_1 = "alga1";
            public static string NAME_ALGA_2 = "alga2";
            public static string NAME_ALGA_3 = "alga3";
            public static string NAME_ALGA_4 = "alga4";
            public static int QUANTITY_ALGA_1 = 20;
            public static int QUANTITY_ALGA_2 = 20;
            public static int QUANTITY_ALGA_3 = 20;
            public static int QUANTITY_ALGA_4 = 20;
            public static TGCVector3 Scale = new TGCVector3(7, 7, 7);
        }

        private string MediaDir, ShadersDir;
        private TypeVegetation alga1;
        private TypeVegetation alga2;
        private TypeVegetation alga3;
        private TypeVegetation alga4;

        public List<TypeVegetation> ListAlgas = new List<TypeVegetation>();

        public Vegetation(string mediaDir, string shadersDir)
        {
            MediaDir = mediaDir;
            ShadersDir = shadersDir;
            Init();
        }

        public void Dispose()
        {
            ListAlgas.ForEach(vegetation => vegetation.mesh.Dispose());
        }

        private void Init()
        {
            InitializerVegetation();
            GenerateDuplicates(alga1, ref ListAlgas);
            GenerateDuplicates(alga2, ref ListAlgas);
            GenerateDuplicates(alga3, ref ListAlgas);
            GenerateDuplicates(alga4, ref ListAlgas);
        }

        private void InitializerVegetation()
        {
            alga1.name = Constants.NAME_ALGA_1;
            alga1.ID = Constants.QUANTITY_ALGA_1;
            LoadInitial(ref alga1);

            alga2.name = Constants.NAME_ALGA_2;
            alga2.ID = Constants.QUANTITY_ALGA_2;
            LoadInitial(ref alga2);

            alga3.name = Constants.NAME_ALGA_3;
            alga3.ID = Constants.QUANTITY_ALGA_3;
            LoadInitial(ref alga3);

            alga4.name = Constants.NAME_ALGA_4;
            alga4.ID = Constants.QUANTITY_ALGA_4;
            LoadInitial(ref alga4);
        }

        private void LoadInitial(ref TypeVegetation vegetation)
        {
            vegetation.mesh = new TgcSceneLoader().loadSceneFromFile(MediaDir + vegetation.name + "-TgcScene.xml").Meshes[0];
            vegetation.mesh.Name = vegetation.name;
        }

        public void GenerateDuplicates(TypeVegetation vegetation, ref List<TypeVegetation> vegetations)
        {
            foreach (int index in Enumerable.Range(0, vegetation.ID))
            {
                TypeVegetation newVegetation = new TypeVegetation
                {
                    ID = index,
                    name = vegetation.name + "_" + index
                };
                newVegetation.mesh = vegetation.mesh.createMeshInstance(newVegetation.name);
                newVegetation.mesh.AlphaBlendEnable = true;
                newVegetation.mesh.Transform = TGCMatrix.Scaling(Constants.Scale);
                newVegetation.mesh.BoundingBox.scaleTranslate(newVegetation.mesh.Position, Constants.Scale);
                vegetations.Add(newVegetation);
            }
        }

        public void Render()
        {
            ListAlgas.ForEach(vegetation => vegetation.mesh.Render());
        }
    }
}
