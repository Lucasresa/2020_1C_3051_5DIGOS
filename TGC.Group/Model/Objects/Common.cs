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
            public int ID;
            public string name;
            public TgcMesh mesh;
            public RigidBody Body { get; set; }
        }

        struct Constants
        {
            public static string NAME_CORAL_NORMAL = "NORMALCORAL";
            public static string NAME_CORAL_TREE = "TREECORAL";
            public static string NAME_CORAL_SPIRAL = "SPIRALCORAL";
            public static string NAME_ORE_GOLD = "GOLD";
            public static string NAME_ORE_IRON = "IRON";
            public static string NAME_ORE_SILVER = "SILVER";
            public static string NAME_ROCK = "ROCK";
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
        private readonly string MediaDir, ShadersDir;
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
            coralNormal.ID = Constants.QUANTITY_CORAL_NORMAL;
            LoadInitial(ref coralNormal);

            coralSpiral.name = Constants.NAME_CORAL_SPIRAL;
            coralSpiral.ID = Constants.QUANTITY_CORAL_SPIRAL;
            LoadInitial(ref coralSpiral);

            coralTree.name = Constants.NAME_CORAL_TREE;
            coralTree.ID = Constants.QUANTITY_CORAL_TREE;
            LoadInitial(ref coralTree);

            oreGold.name = Constants.NAME_ORE_GOLD;
            oreGold.ID = Constants.QUANTITY_ORE_GOLD;
            LoadInitial(ref oreGold);

            oreIron.name = Constants.NAME_ORE_IRON;
            oreIron.ID = Constants.QUANTITY_ORE_IRON;
            LoadInitial(ref oreIron);

            oreSilver.name = Constants.NAME_ORE_SILVER;
            oreSilver.ID = Constants.QUANTITY_ORE_SILVER;
            LoadInitial(ref oreSilver);

            rock.name = Constants.NAME_ROCK;
            rock.ID = Constants.QUANTITY_ROCK;
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
            ListCorals.ForEach(coral => { coral.Body.Translate(coral.mesh.Position.ToBulletVector3()); coral.mesh.BoundingBox.scaleTranslate(coral.mesh.Position, Constants.Scale); });
            ListOres.ForEach(ore => { ore.Body.Translate(ore.mesh.Position.ToBulletVector3()); ore.mesh.BoundingBox.scaleTranslate(ore.mesh.Position, Constants.Scale); });
            ListRock.ForEach(rock => { rock.Body.Translate(rock.mesh.Position.ToBulletVector3()); rock.mesh.BoundingBox.scaleTranslate(rock.mesh.Position, Constants.Scale); });
        }

        private void CreateRigidBody(ref TypeCommon common)
        {
            common.Body = RigidBodyFactory.CreateRigidBodyFromTgcMesh(common.mesh);
            common.Body.CenterOfMassTransform = TGCMatrix.Translation(common.mesh.Position).ToBulletMatrix();
            common.Body.CollisionShape.LocalScaling = Constants.Scale.ToBulletVector3();
        }

        public void GenerateDuplicates(TypeCommon common, ref List<TypeCommon> commons)
        {
            foreach (int index in Enumerable.Range(1, common.ID))
            {
                TypeCommon newCommon = new TypeCommon
                {
                    ID = index,
                    name = common.name + "_" + index
                };
                newCommon.mesh = common.mesh.createMeshInstance(newCommon.name);
                newCommon.mesh.Transform = TGCMatrix.Scaling(Constants.Scale);
                CreateRigidBody(ref newCommon);
                commons.Add(newCommon);
            }
        }

        public void Render()
        {
            ListCorals.ForEach(coral => { coral.mesh.Render(); });
            ListOres.ForEach(ore => ore.mesh.Render());
            ListRock.ForEach(rock => rock.mesh.Render());
        }
    }
}
