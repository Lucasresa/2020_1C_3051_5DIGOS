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
            public static string NAME_NORMAL_FISH = "NORMALFISH";
            public static string NAME_YELLOW_FISH = "YELLOWFISH";
            public static int QUANTITY_CORAL_NORMAL = 20;
            public static int QUANTITY_CORAL_TREE = 20;
            public static int QUANTITY_CORAL_SPIRAL = 20;
            public static int QUANTITY_ORE_IRON = 20;
            public static int QUANTITY_ORE_SILVER = 20;
            public static int QUANTITY_ORE_GOLD = 20;
            public static int QUANTITY_ROCK = 20;
            public static int QUANTITY_NORMAL_FISH = 20;
            public static int QUANTITY_YELLOW_FISH = 20;
            public static TGCVector3 Scale = new TGCVector3(10, 10, 10);
        }

        private TgcMesh coralNormal;
        private TgcMesh coralTree;
        private TgcMesh coralSpiral;
        private TgcMesh oreIron;
        private TgcMesh oreSilver;
        private TgcMesh oreGold;
        private TgcMesh rock;
        private TgcMesh normalFish;
        private TgcMesh yellowFish;
        private readonly string MediaDir, ShadersDir;
        private readonly BulletRigidBodyFactory RigidBodyFactory = BulletRigidBodyFactory.Instance;

        public List<TypeCommon> ListCorals = new List<TypeCommon>();
        public List<TypeCommon> ListOres = new List<TypeCommon>();
        public List<TypeCommon> ListRock = new List<TypeCommon>();
        public List<TypeCommon> ListFishes = new List<TypeCommon>();

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
            GenerateDuplicates(coralNormal, ref ListCorals, quantity: Constants.QUANTITY_CORAL_NORMAL);
            GenerateDuplicates(coralTree, ref ListCorals, quantity: Constants.QUANTITY_CORAL_TREE);
            GenerateDuplicates(coralSpiral, ref ListCorals, quantity: Constants.QUANTITY_CORAL_SPIRAL);
            GenerateDuplicates(oreIron, ref ListOres, quantity: Constants.QUANTITY_ORE_IRON);
            GenerateDuplicates(oreSilver, ref ListOres, quantity: Constants.QUANTITY_ORE_SILVER);
            GenerateDuplicates(oreGold, ref ListOres, quantity: Constants.QUANTITY_ORE_GOLD);
            GenerateDuplicates(normalFish, ref ListFishes, quantity: Constants.QUANTITY_NORMAL_FISH, createRB: false);
            GenerateDuplicates(yellowFish, ref ListFishes, quantity: Constants.QUANTITY_YELLOW_FISH, createRB: false);
        }

        private void InitializerFishes()
        {
            LoadInitial(ref coralNormal, Constants.NAME_CORAL_NORMAL);
            LoadInitial(ref coralSpiral, Constants.NAME_CORAL_SPIRAL);
            LoadInitial(ref coralTree, Constants.NAME_CORAL_TREE);
            LoadInitial(ref oreGold, Constants.NAME_ORE_GOLD);
            LoadInitial(ref oreIron, Constants.NAME_ORE_IRON);
            LoadInitial(ref oreSilver, Constants.NAME_ORE_SILVER);
            LoadInitial(ref rock, Constants.NAME_ROCK);
            LoadInitial(ref normalFish, Constants.NAME_NORMAL_FISH);
            LoadInitial(ref yellowFish, Constants.NAME_YELLOW_FISH);
        }

        private void LoadInitial(ref TgcMesh mesh, string meshName)
        {
            mesh = new TgcSceneLoader().loadSceneFromFile(MediaDir + meshName + "-TgcScene.xml").Meshes[0];
            mesh.Name = meshName;
        }

        public void LocateObjects()
        {
            ListCorals.ForEach(coral => { coral.Body.Translate(coral.mesh.Position.ToBulletVector3()); coral.mesh.BoundingBox.scaleTranslate(coral.mesh.Position, Constants.Scale); });
            ListOres.ForEach(ore => { ore.Body.Translate(ore.mesh.Position.ToBulletVector3()); ore.mesh.BoundingBox.scaleTranslate(ore.mesh.Position, Constants.Scale); });
            ListRock.ForEach(rock => { rock.Body.Translate(rock.mesh.Position.ToBulletVector3()); rock.mesh.BoundingBox.scaleTranslate(rock.mesh.Position, Constants.Scale); });
            ListFishes.ForEach(fish => fish.mesh.BoundingBox.scaleTranslate(fish.mesh.Position, Constants.Scale));
        }

        private void CreateRigidBody(ref TypeCommon common)
        {
            common.Body = RigidBodyFactory.CreateRigidBodyFromTgcMesh(common.mesh);
            common.Body.CenterOfMassTransform = TGCMatrix.Translation(common.mesh.Position).ToBulletMatrix();
            common.Body.CollisionShape.LocalScaling = Constants.Scale.ToBulletVector3();
        }

        public void GenerateDuplicates(TgcMesh common, ref List<TypeCommon> commons, int quantity, bool createRB = true)
        {
            foreach (int index in Enumerable.Range(0, quantity))
            {
                TypeCommon newCommon = new TypeCommon
                {
                    ID = index,
                    name = common.Name + "_" + index
                };
                newCommon.mesh = common.createMeshInstance(newCommon.name);
                newCommon.mesh.Transform = TGCMatrix.Scaling(Constants.Scale);
                if (createRB)
                    CreateRigidBody(ref newCommon);
                commons.Add(newCommon);
            }
        }

        public void Render()
        {
            ListCorals.ForEach(coral => coral.mesh.Render());
            ListOres.ForEach(ore => ore.mesh.Render());
            ListRock.ForEach(rock => rock.mesh.Render());
        }
    }
}
