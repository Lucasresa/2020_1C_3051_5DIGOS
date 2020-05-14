using System.Collections.Generic;
using System.Linq;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Terrain;
using TGC.Group.Model.Bullet;
using TGC.Group.Utils;
using static TGC.Core.Terrain.TgcSkyBox;
using static TGC.Group.Model.Terrains.Terrain;

namespace TGC.Group.Model.Terrains
{
    class Sky
    {
        private TgcSkyBox sky;

        private string MediaDir;
        private string ShadersDir;

        public Perimeter perimeter;

        private CameraFPS Camera;

        public Sky(string mediaDir, string shadersDir, CameraFPS camera)
        {
            sky = new TgcSkyBox
            {
                Size = new TGCVector3(9000, 9000, 9000),
                Center = new TGCVector3(0, 1800, 0)
            };

            MediaDir = mediaDir;
            ShadersDir = shadersDir;
            Camera = camera;
            LoadSkyBox();
        }

        public void LoadSkyBox()
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
        }

        public virtual void Render()
        {
            var size = sky.Size.X / 2;
            sky.Center = new TGCVector3(Camera.position.X, sky.Center.Y, Camera.position.Z);
            perimeter.xMin = sky.Center.X - size;
            perimeter.xMax = sky.Center.X + size;
            perimeter.zMin = sky.Center.Z - size;
            perimeter.zMax = sky.Center.Z + size;
            sky.Render();
        }

        public virtual void Dispose()
        {
            sky.Dispose();
        }

        public bool Contains(RigidBody rigidBody)
        {
            if (rigidBody.isTerrain || rigidBody.isIndoorShip)
                return true;

            var posX = rigidBody.body.CenterOfMassPosition.X;
            var posZ = rigidBody.body.CenterOfMassPosition.Z;
            return (posX < perimeter.xMax && posX > perimeter.xMin &&
                     posZ < perimeter.zMax && posZ > perimeter.zMin);
        }

        public bool Contains(TgcMesh vegetation)
        {
            var posX = vegetation.Position.X;
            var posZ = vegetation.Position.Z;
            return (posX < perimeter.xMax && posX > perimeter.xMin &&
                     posZ < perimeter.zMax && posZ > perimeter.zMin);
        }

        public TGCVector3 getSkyboxCenter()
        {
            return sky.Center;
        }

    }
}
