using BulletSharp;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Terrain;
using TGC.Group.Utils;
using static TGC.Group.Model.GameModel;

namespace TGC.Group.Model.Objects
{
    class Skybox
    {
        #region Atributos
        private struct Constants
        {
            public static TGCVector3 size = new TGCVector3(9000, 9000, 9000);
            public static TGCVector3 center = new TGCVector3(0, 1800, 0);
        }

        private TgcSkyBox skybox;
        private string MediaDir, ShadersDir;
        private CameraFPS Camera;
        public Perimeter currentPerimeter;
        public float Radius { get { return Constants.size.X / 2; } }
        #endregion

        #region Constructor
        public Skybox(string mediaDir, string shadersDir, CameraFPS camera)
        {
            skybox = new TgcSkyBox
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
            skybox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "sup.jpg");
            skybox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "inf.jpg");
            skybox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "izq.jpg");
            skybox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "der.jpg");
            skybox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "front.jpg");
            skybox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "post.jpg");
            skybox.SkyEpsilon = 30f;

            skybox.Init();
            CalculatePerimeter();
        }

        public virtual void Update()
        {
            CalculatePerimeter();
        }

        public virtual void Render(Perimeter worldSize)
        {
            skybox.Center = new TGCVector3(FastMath.Clamp(Camera.Position.X, worldSize.xMin + Radius, worldSize.xMax - Radius),
                                        skybox.Center.Y,
                                        FastMath.Clamp(Camera.Position.Z, worldSize.zMin + Radius, worldSize.zMax - Radius));
            skybox.Render();
        }

        public virtual void Dispose()
        {
            skybox.Dispose();
        }

        public bool Contains(RigidBody rigidBody)
        {
            var posX = rigidBody.CenterOfMassPosition.X;
            var posZ = rigidBody.CenterOfMassPosition.Z;
            return InPerimeterSkyBox(posX, posZ);
        }

        public bool Contains(TgcMesh mesh)
        {
            var posX = mesh.Position.X;
            var posZ = mesh.Position.Z;
            mesh.AlphaBlendEnable = true;
            return InPerimeterSkyBox(posX, posZ);
        }

        public TGCVector3 GetSkyboxCenter()
        {
            return skybox.Center;
        }

        public bool InPerimeterSkyBox(float posX, float posZ)
        {
            return posX < currentPerimeter.xMax && posX > currentPerimeter.xMin && posZ < currentPerimeter.zMax && posZ > currentPerimeter.zMin;
        }

        public bool CameraIsNearBorder(CameraFPS camera)
        {
            return !InPerimeterSkyBox(camera.Position.X, camera.Position.Z);
        }

        private void CalculatePerimeter()
        {
            var size = skybox.Size.X / 2;

            currentPerimeter.xMin = skybox.Center.X - size;
            currentPerimeter.xMax = skybox.Center.X + size;
            currentPerimeter.zMin = skybox.Center.Z - size;
            currentPerimeter.zMax = skybox.Center.Z + size;
        }

        public void Render()
        {
            skybox.Render();
        }
        #endregion
    }
}
