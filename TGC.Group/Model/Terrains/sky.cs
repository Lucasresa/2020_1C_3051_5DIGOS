using System.Collections.Generic;
using System.Linq;
using BulletSharp;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Terrain;
using TGC.Group.Model.Bullet;
using TGC.Group.Utils;
using static TGC.Core.Terrain.TgcSkyBox;
using static TGC.Group.Model.Terrains.Terrain;
using static TGC.Group.Model.GameModel;

namespace TGC.Group.Model.Terrains
{
    class Sky
    {
        #region Atributos
        private struct Constants
        {
            public static TGCVector3 size = new TGCVector3(9000, 9000, 9000);
            public static TGCVector3 center = new TGCVector3(0, 1800, 0);
        }

        private TgcSkyBox sky;
        private string MediaDir, ShadersDir;
        private CameraFPS Camera;
        public Perimeter currentPerimeter;
        #endregion

        #region Constructor
        public Sky(string mediaDir, string shadersDir, CameraFPS camera)
        {
            sky = new TgcSkyBox
            {
                Size = Constants.size,
                Center = Constants.center
            };

            MediaDir = mediaDir;
            ShadersDir = shadersDir;
            Camera = camera;
            LoadSkyBox();
        }
        #endregion

        #region Metodos
        private void LoadSkyBox()
        {
            var texturesPath = MediaDir + "SkyBox\\";
            sky.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "sup.jpg");
            sky.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "inf.jpg");
            sky.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "izq.jpg");
            sky.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "der.jpg");
            sky.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "front.jpg");
            sky.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "post.jpg");
            sky.SkyEpsilon = 30f;

            sky.Init();
            calculatePerimeter();
        }

        public virtual void Update()
        {
            calculatePerimeter();
        }

        public virtual void Render()
        {
            sky.Center = new TGCVector3(Camera.position.X, sky.Center.Y, Camera.position.Z);
            sky.Render();
        }

        public virtual void Dispose()
        {
            sky.Dispose();
        }

        public bool Contains(RigidBody rigidBody)
        {
            var posX = rigidBody.CenterOfMassPosition.X;
            var posZ = rigidBody.CenterOfMassPosition.Z;
            return inPerimeterSkyBox(posX, posZ);
        }

        public bool Contains(TgcMesh vegetation)
        {
            var posX = vegetation.Position.X;
            var posZ = vegetation.Position.Z;
            vegetation.AlphaBlendEnable = true;
            return inPerimeterSkyBox(posX, posZ);
        }

        public TGCVector3 getSkyboxCenter()
        {
            return sky.Center;
        }

        private bool inPerimeterSkyBox(float posX, float posZ)
        {
            return posX < currentPerimeter.xMax && posX > currentPerimeter.xMin && posZ < currentPerimeter.zMax && posZ > currentPerimeter.zMin;
        }

        private void calculatePerimeter()
        {
            var size = sky.Size.X / 2;

            currentPerimeter.xMin = sky.Center.X - size;
            currentPerimeter.xMax = sky.Center.X + size;
            currentPerimeter.zMin = sky.Center.Z - size;
            currentPerimeter.zMax = sky.Center.Z + size;
        }
        #endregion
    }
}
