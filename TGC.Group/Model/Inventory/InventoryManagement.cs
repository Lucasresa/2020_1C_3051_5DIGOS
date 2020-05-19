using BulletSharp;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Text;
using TGC.Group.Model.Bullet.Bodies;
using TGC.Group.Model.Draw;
using TGC.Group.Utils;
using Text = TGC.Group.Model.Draw.Sprite;

namespace TGC.Group.Model.Inventory
{
    class InventoryManagement
    {
        #region Atributos
        struct Constants
        {
            public static (int width, int height) screen = (width: D3DDevice.Instance.Device.Viewport.Width, height: D3DDevice.Instance.Device.Viewport.Height);
        }

        private string MediaDir, ShadersDir;
        private TgcD3dInput Input;
        private bool showInventory { get; set; }
        private TgcText2D DrawText = new TgcText2D();
        public Sprite lookAt;
        private (int posX, int posY) mouseCenter;
        private Ray ray;

        public Dictionary<string, List<String>> items;
        private List<string> gold = new List<string>();
        private List<string> iron = new List<string>();
        private List<string> fish = new List<string>();
        private List<string> normalCoral = new List<string>();
        private List<string> rock = new List<string>();
        private List<string> silver = new List<string>();
        private List<string> spiralCoral = new List<string>();
        private List<string> treeCoral = new List<string>();
        private List<string> yellowFish = new List<string>();

        private Text textInfo = new Text();
        public bool hasAWeapon = false;
        public bool HasARod { get; set; } = false;
        public bool hasADivingHelmet = false;

        private bool lookWithPuntero = false;
        private bool showRecolectionInfo = false;
        private string recolectionName;
        private float timeShowRecolection;

        public int inHand = 0; // 0-Nada 1-Arma

        #endregion

        #region Constructor
        public InventoryManagement(string mediaDir, string shadersDir, TgcD3dInput input)
        {
            Input = input;
            MediaDir = mediaDir;
            ShadersDir = shadersDir;
            initializer();
        }
        #endregion

        #region Metodos
        private void initializer()
        {
            ray = new Ray(Input);
            initializerDiccionary();

            lookAt = new Sprite(MediaDir, ShadersDir);
            lookAt.setInitialSprite(new TGCVector2(1, 1), "mira");
            mouseCenter.posX = (Constants.screen.width - lookAt.sprite.texture.Size.Width) / 2;
            mouseCenter.posY = (Constants.screen.height - lookAt.sprite.texture.Size.Height) / 2;
            lookAt.sprite.Position = new TGCVector2(mouseCenter.posX, mouseCenter.posY);
        }


        public void Update(TgcD3dInput input, DiscreteDynamicsWorld dynamicsWorld, ref List<CommonRigidBody> commonRigidBody, ref List<FishMesh> fishes, bool lockCam, float elapsedTime)
        {
            if (lockCam)
                lookAt.sprite.Position = new TGCVector2(Cursor.Position.X, Cursor.Position.Y);
            else
                lookAt.sprite.Position = new TGCVector2(mouseCenter.posX, mouseCenter.posY);

            showInventory = lockCam;

            if (input.buttonPressed(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                findItem(dynamicsWorld, ref commonRigidBody, ref fishes);
                timeShowRecolection = 0;
            }

            timeShowRecolection += elapsedTime;

            if (timeShowRecolection > 1.00)
                showRecolectionInfo = false;

            if (hasAWeapon && input.keyPressed(Key.D1))
                inHand = 1;

            if (input.keyPressed(Key.D0))
                inHand = 0;
        }

        public void Render()
        {
            lookAt.Render();
            if (showInventory)
            {
                var text = "Inventario: " +
                           "\n\nGold: " + gold.Count +
                           "\nSilver: " + silver.Count +
                           "\nRocas: " + rock.Count +
                           "\niron: " + iron.Count +
                           "\nFish: " + fish.Count +
                           "\nYellow Fish: " + yellowFish.Count +
                           "\nSpiral Coral: " + spiralCoral.Count +
                           "\nNormal Coral: " + normalCoral.Count +
                           "\nTree Coral: " + treeCoral.Count;

                (int width, int height) size = (width: 500, height: 500);
                (int posX, int posY) position = (posX: (Constants.screen.width - size.width) / 2, posY: (Constants.screen.height - size.height) / 2);
                textInfo.drawText(text, Color.White, new Point(position.posX, position.posY), new Size(size.width, size.height), TgcText2D.TextAlign.LEFT, new Font("Arial Black", 12, FontStyle.Bold));
            }

            if (showRecolectionInfo)
            {
                Sprite txt = new Sprite();
                var text = "RECOLECTASTE " + recolectionName;
                (int width, int height) size = (width: 400, height: 10);
                (int posX, int posY) position = (posX: (Constants.screen.width - size.width) / 2, posY: (Constants.screen.height - size.height * 10) / 2);
                txt.drawText(text, Color.White, new Point(position.posX, position.posY), new Size(size.width, size.height), TgcText2D.TextAlign.LEFT, new Font("Arial Black", 10, FontStyle.Bold));
            }
        }

        public void Dispose()
        {
            lookAt.Dispose();
        }

        private void DisposeAll(List<CommonRigidBody> list)
        {
            list.ForEach(rigidBody => rigidBody.Dispose());
            list.RemoveRange(0, list.Count);
        }

        private bool isAFish(CommonRigidBody rigidBody)
        {
            var name = rigidBody.getName().ToLower();
            return name.Contains("fish");
        }

        private void splitItems(TgcMesh lookAtItem)
        {
            var name = lookAtItem.Name;
            var key = name.Substring(0, name.IndexOf("_"));
            items[key].Add(name);
        }

        private void findItem(DiscreteDynamicsWorld dynamicsWorld, ref List<CommonRigidBody> commonRigidBody, 
                                ref List<FishMesh> fishes)
        {
            var item = commonRigidBody.Find(rigidBody =>
            { return ray.intersectsWithObject(rigidBody.getAABB(), 500); });

            if (item != null)
            {

                splitItems(item.Mesh);
                showRecolectionOfType(item.Mesh);
                dynamicsWorld.RemoveRigidBody(item.body);
                commonRigidBody.Remove(item);
                item.Dispose();
            } else if(HasARod)
            {
                var fishItem = fishes.Find(fish => ray.intersectsWithObject(fish.Mesh.BoundingBox, 500));
                if (fishItem != null)
                {
                    splitItems(fishItem.Mesh);
                    showRecolectionOfType(fishItem.Mesh);
                    fishes.Remove(fishItem);
                    fishItem.Dispose();
                }
            }
        }

        public void changePointer()
        {
            if (lookWithPuntero)
                lookAt.setInitialSprite(new TGCVector2(1, 1), "mira");
            else
                lookAt.setInitialSprite(new TGCVector2(1, 1), "puntero");
            lookWithPuntero = !lookWithPuntero;
        }

        private void initializerDiccionary()
        {
            items = new Dictionary<string, List<string>>();
            items.Add("gold", gold);
            items.Add("silver", silver);
            items.Add("rock-n", rock);
            items.Add("iron", iron);
            items.Add("fish", fish);
            items.Add("yellowFish", yellowFish);
            items.Add("spiralCoral", spiralCoral);
            items.Add("normalCoral", normalCoral);
            items.Add("treeCoral", treeCoral);
        }
        private void showRecolectionOfType(TgcMesh collectedMesh)
        {
            string name = collectedMesh.Name.Split('_')[0]; 
            string showName = "";
            switch (name)
            {
                case "gold":
                    showName = "GOLD +1";
                    break;
                case "silver":
                    showName = "SILVER +1";
                    break;
                case "rock-n":
                    showName = "ROCK +1";
                    break;
                case "iron":
                    showName = "IRON +1";
                    break;
                case "fish":
                    showName = "FISH +1";
                    break;
                case "yellowFish":
                    showName = "YELLOW FISH +1";
                    break;
                case "spiralCoral":
                    showName = "SPIRAL CORAL +1";
                    break;
                case "normalCoral":
                    showName = "NORMAL CORAL +1";
                    break;
                case "treeCoral":
                    showName = "TREE CORAL +1";
                    break;
            }

            showRecolectionInfo = !showRecolectionInfo;
            recolectionName = showName;
        }

        #endregion
    }
}
