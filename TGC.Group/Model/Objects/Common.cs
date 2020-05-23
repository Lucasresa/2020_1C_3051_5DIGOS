using BulletSharp;
using System.Collections.Generic;
using System.Linq;
using TGC.Core.BulletPhysics;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Objects
{
    class Common
    {
        public struct TypeCommon
        {
            public string name;
            public TgcMesh mesh;
            public RigidBody Body { get; set; }
            public int quantity;
        }

        struct Constants
        {
            public static string NAME_CORAL_NORMAL = "normalCoral";
            public static string NAME_CORAL_TREE = "treeCoral";
            public static string NAME_CORAL_SPIRAL = "spiralCoral";
            public static string NAME_ORE_IRON = "iron";
            public static string NAME_ORE_SILVER = "silver";
            public static string NAME_ORE_GOLD = "gold";
            public static string NAME_ROCK = "rock";
            public static int QUANTITY_CORAL_NORMAL = 20;
            public static int QUANTITY_CORAL_TREE = 20;
            public static int QUANTITY_CORAL_SPIRAL = 20;
            public static int QUANTITY_ORE_IRON = 20;
            public static int QUANTITY_ORE_SILVER = 20;
            public static int QUANTITY_ORE_GOLD = 20;
            public static int QUANTITY_ROCK = 20;
            public static TGCVector3 Scale = new TGCVector3(10, 10, 10);
        }

        private TypeCommon coralNormal;
        private TypeCommon coralTree;
        private TypeCommon coralSpiral;
        private TypeCommon oreIron;
        private TypeCommon oreSilver;
        private TypeCommon oreGold;
        private TypeCommon rock;
        private int counter;
        private string MediaDir, ShadersDir;
        private readonly BulletRigidBodyFactory RigidBodyFactory = BulletRigidBodyFactory.Instance;

        public List<TypeCommon> ListCorals = new List<TypeCommon>();
        public List<TypeCommon> ListOres = new List<TypeCommon>();
        public List<TypeCommon> ListRock = new List<TypeCommon>();

        public Common(string mediaDir, string shadersDir)
        {
            MediaDir = mediaDir;
            ShadersDir = shadersDir;
            Init();
        }

        public void Dispose()
        {
            ListCorals.ForEach(coral => coral.mesh.Dispose());
            ListOres.ForEach(ore => ore.mesh.Dispose());
            ListRock.ForEach(rock => rock.mesh.Dispose());
        }

        private void Init()
        {
            InitializerFishes();
            GenerateDuplicates(coralNormal, ref ListCorals);
            GenerateDuplicates(coralTree, ref ListCorals);
            GenerateDuplicates(coralSpiral, ref ListCorals);
            GenerateDuplicates(oreIron, ref ListOres);
            GenerateDuplicates(oreSilver, ref ListOres);
            GenerateDuplicates(oreGold, ref ListOres);
            GenerateDuplicates(rock, ref ListRock);
        }

        private void InitializerFishes()
        {
            coralNormal.name = Constants.NAME_CORAL_NORMAL;
            coralNormal.quantity = Constants.QUANTITY_CORAL_NORMAL;
            LoadInitial(ref coralNormal);

            coralSpiral.name = Constants.NAME_CORAL_SPIRAL;
            coralSpiral.quantity = Constants.QUANTITY_CORAL_SPIRAL;
            LoadInitial(ref coralSpiral);

            coralTree.name = Constants.NAME_CORAL_TREE;
            coralTree.quantity = Constants.QUANTITY_CORAL_TREE;
            LoadInitial(ref coralTree);

            oreGold.name = Constants.NAME_ORE_GOLD;
            oreGold.quantity = Constants.QUANTITY_ORE_GOLD;
            LoadInitial(ref oreGold);

            oreIron.name = Constants.NAME_ORE_IRON;
            oreIron.quantity = Constants.QUANTITY_ORE_IRON;
            LoadInitial(ref oreIron);

            oreSilver.name = Constants.NAME_ORE_SILVER;
            oreSilver.quantity = Constants.QUANTITY_ORE_SILVER;
            LoadInitial(ref oreSilver);

            rock.name = Constants.NAME_ROCK;
            rock.quantity = Constants.QUANTITY_ROCK;
            LoadInitial(ref rock);
        }

        private void LoadInitial(ref TypeCommon common)
        {
            common.mesh = new TgcSceneLoader().loadSceneFromFile(MediaDir + common.name + "-TgcScene.xml").Meshes[0];
            common.mesh.Name = common.name;
            CreateRigidBody(ref common);
        }

        public void LocateBody()
        {
            ListCorals.ForEach(coral => coral.Body.Translate(coral.mesh.Position.ToBulletVector3()));
            ListOres.ForEach(ore => ore.Body.Translate(ore.mesh.Position.ToBulletVector3()));
            ListRock.ForEach(rock => rock.Body.Translate(rock.mesh.Position.ToBulletVector3()));
        }

        private void CreateRigidBody(ref TypeCommon common)
        {
            common.Body = RigidBodyFactory.CreateRigidBodyFromTgcMesh(common.mesh);
            common.Body.CenterOfMassTransform = TGCMatrix.Translation(common.mesh.Position).ToBulletMatrix();
            common.Body.CollisionShape.LocalScaling = Constants.Scale.ToBulletVector3();
        }

        public void GenerateDuplicates(TypeCommon common, ref List<TypeCommon> commons)
        {
            foreach (int _ in Enumerable.Range(1, common.quantity))
            {
                TypeCommon newCommon = new TypeCommon
                {
                    name = common.name + "_" + counter++,
                    quantity = 1
                };
                newCommon.mesh = common.mesh.createMeshInstance(newCommon.name);
                newCommon.mesh.Transform = TGCMatrix.Scaling(Constants.Scale);
                newCommon.mesh.BoundingBox.scaleTranslate(newCommon.mesh.Position, Constants.Scale);
                CreateRigidBody(ref newCommon);
                commons.Add(newCommon);
            }
            counter = 0;
        }

        public void Render()
        {
            ListCorals.ForEach(coral => coral.mesh.Render());
            ListOres.ForEach(ore => ore.mesh.Render());
            ListRock.ForEach(rock => rock.mesh.Render());
        }
    }
}
