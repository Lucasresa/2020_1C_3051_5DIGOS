using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;

namespace TGC.Group.Model.Objects
{
    class Bubble
    {
        private readonly string MediaDir;
        private TGCSphere BubbleTemplate;
        public List<TGCSphere> Bubbles = new List<TGCSphere>();
        public List<TGCSphere> BubblesAux = new List<TGCSphere>();
        private readonly float Speed = 70;
        private TGCVector3 Scale = new TGCVector3(25, 25, 25);
        private float Time = 0;
        
        public Bubble(string mediaDir)
        {
            MediaDir = mediaDir;
            Init();
        }

        private void Init()
        {
            var texture = TgcTexture.createTexture(MediaDir + @"\Textures\bubble.png");
            BubbleTemplate = new TGCSphere(30, texture, new TGCVector3(0, 0, 0))
            {
                AlphaBlendEnable = true
            };
            BubbleTemplate.updateValues();
            BubbleTemplate.Transform = TGCMatrix.Scaling(Scale);

            for (int index = 1; index <= 250; index++)
            {
                Bubbles.Add(BubbleTemplate.clone());
                BubblesAux.Add(BubbleTemplate.clone());
            }
        }

        public void SetShader(Effect effect, string technique)
        {
            Bubbles.ForEach(bubble => { bubble.Effect = effect; bubble.Technique = technique; });
            BubblesAux.ForEach(bubble => { bubble.Effect = effect; bubble.Technique = technique; });
        }

        public void Render() => Bubbles.ForEach(bubble => bubble.Render());

        public void Dispose()
        {
            BubbleTemplate.Dispose();
            Bubbles.ForEach(bubble => bubble.Dispose());
        }

        public void Update(float elapsedTime, MeshBuilder meshBuilder, Skybox skybox)
        {
            Time += elapsedTime;
            Bubbles.ForEach(bubble => {
                bubble.Transform *= TGCMatrix.Translation(TGCVector3.Up * Speed * elapsedTime);
                if (bubble.Transform.Origin.Y > 3400)
                    BubblesAux.Add(bubble);                    
            });
            Bubbles.RemoveAll(bubble => BubblesAux.Contains(bubble));

            if (Time > 10 && BubblesAux.Count > 100)
            {
                var bubbles = BubblesAux.Take(100).ToList();
                bubbles.ForEach(bubble => bubble.Transform = TGCMatrix.Scaling(Scale));
                meshBuilder.LocateMeshesInWorld(meshes: ref bubbles, area: skybox.CurrentPerimeter);
                Bubbles.AddRange(bubbles);
                BubblesAux.RemoveRange(0, 100);
                Time = 0;
            }
        }
    }
}
