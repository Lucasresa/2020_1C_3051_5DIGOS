using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Camara;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.Terrain;
using TGC.Group.Utils;

namespace TGC.Group.Model.Terrains
{
    class Sky
    {
        private TgcSkyBox sky;

        private string MediaDir;
        private string ShadersDir;

        private CameraFPS Camera;

        public Sky(string mediaDir, string shadersDir, TgcCamera camera)
        {
            sky = new TgcSkyBox
            {
                Size = new TGCVector3(9000, 9000, 9000),
                Center = new TGCVector3(0, 1800, 0)
            };

            MediaDir = mediaDir;
            ShadersDir = shadersDir;
            Camera = (CameraFPS)camera;
        }

        public void LoadSkyBox()
        {         
            var texturesPath = MediaDir + "SkyBox\\";
            // TODO: Habria que encontrar imagenes con mayor resolucion para el SkyBox
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
            sky.Center = new TGCVector3(Camera.Position.X, sky.Center.Y, Camera.Position.Z);
            sky.Render();
        }

        public virtual void Dispose()
        {
            sky.Dispose();
        }
    }
}
