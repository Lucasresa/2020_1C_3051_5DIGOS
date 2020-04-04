using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.Terrain;
using TGC.Group.Utils;

namespace TGC.Group.Model
{
    class Sky : GameModel
    {
        private TgcSkyBox sky;

        public Sky(string mediaDir, string shadersDir) : base(mediaDir,shadersDir)
        {
            sky = new TgcSkyBox
            {
                Size = new TGCVector3(7000, 7000, 7000),
                Center = new TGCVector3(0, 1500, 0)
            };
        }

        public override void Init()
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

        public override void Update()
        {
            PreUpdate();

            //Se cambia el valor por defecto del farplane para evitar cliping de farplane.
            D3DDevice.Instance.Device.Transform.Projection = TGCMatrix.PerspectiveFovLH(D3DDevice.Instance.FieldOfView, D3DDevice.Instance.AspectRatio,
                    D3DDevice.Instance.ZNearPlaneDistance, D3DDevice.Instance.ZFarPlaneDistance * 2f).ToMatrix();
         
            PostUpdate();
        }

        public override void Render()
        {
            sky.Render();
        }

        public override void Dispose()
        {
            //Liberar recursos del SkyBox
            sky.Dispose();
        }
    }
}
