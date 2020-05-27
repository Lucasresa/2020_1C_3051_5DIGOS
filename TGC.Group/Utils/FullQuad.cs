using Microsoft.DirectX.Direct3D;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.Shaders;
using System.Drawing;
using TGC.Core.Textures;

namespace TGC.Group.Utils
{
    class FullQuad
    {
        private Effect Effect { get; set; }
        private Surface DepthStencil { get; set; }
        private Texture RenderTarget { get; set; }
        private Surface OldDepthStencil { get; set; }
        private Surface OldRenderTarget { get; set; }
        private Surface Surf { get; set; }
        private VertexBuffer FullScreenQuad { get; set; }
        private Texture RenderTarget2D { get; set; }
        private readonly Device Device = D3DDevice.Instance.Device;
        private float Time = 0;

        public string ShadersDir { get; set; }
        public bool RenderEffectTeleport { get; set; }

        public FullQuad(string shadersDir) => ShadersDir = shadersDir;

        public void Initializer(string nameEffect)
        {
            CustomVertex.PositionTextured[] vertex =
            {
                new CustomVertex.PositionTextured(-1, 1, 1, 0, 0),
                new CustomVertex.PositionTextured(1, 1, 1, 1, 0),
                new CustomVertex.PositionTextured(-1, -1, 1, 0, 1),
                new CustomVertex.PositionTextured(1, -1, 1, 1, 1)
            };

            FullScreenQuad = new VertexBuffer(typeof(CustomVertex.PositionTextured), 4, Device, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionTextured.Format, Pool.Default);
            FullScreenQuad.SetData(vertex, 0, LockFlags.None);

            RenderTarget2D = new Texture(Device, Device.PresentationParameters.BackBufferWidth, Device.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default);
            DepthStencil = Device.CreateDepthStencilSurface(Device.PresentationParameters.BackBufferWidth, Device.PresentationParameters.BackBufferHeight, DepthFormat.D24S8, MultiSampleType.None, 0, true);

            Effect = TGCShaders.Instance.LoadEffect(ShadersDir + nameEffect + ".fx");
            Effect.Technique = "OndasTechnique";
        }

        public void SetTime(float value) => Time = FastMath.Clamp(Time + value, 0, 10);

        public void PreRenderMeshes()
        {
            TexturesManager.Instance.clearAll();

            OldRenderTarget = Device.GetRenderTarget(0);
            OldDepthStencil = Device.DepthStencilSurface;
            Surf = RenderTarget2D.GetSurfaceLevel(0);

            Device.SetRenderTarget(0, Surf);
            Device.DepthStencilSurface = DepthStencil;
            Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1f, 0);

            Device.BeginScene();
        }

        public void Render()
        {
            Device.EndScene();
            Surf.Dispose();

            Device.SetRenderTarget(0, OldRenderTarget);
            Device.DepthStencilSurface = OldDepthStencil;

            Device.BeginScene();
            Device.VertexFormat = CustomVertex.PositionTextured.Format;

            if (RenderEffectTeleport)
                Effect.Technique = "OscurecerTechnique";
            else
            {
                Effect.Technique = "DefaultTechnique";
                Time = 0;
            }

            Device.SetStreamSource(0, FullScreenQuad, 0);
            Effect.SetValue("render_target2D", RenderTarget2D);
            Effect.SetValue("time", Time);

            Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1f, 0);

            Effect.Begin(FX.None);
            Effect.BeginPass(0);
            Device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
            Effect.EndPass();
            Effect.End();
        }

        public void Dispose()
        {
            FullScreenQuad.Dispose();
            if (Effect != null && !Effect.Disposed)
                Effect.Dispose();
            RenderTarget2D.Dispose();
            DepthStencil.Dispose();
            OldDepthStencil.Dispose();
            OldRenderTarget.Dispose();
        }
    }
}
