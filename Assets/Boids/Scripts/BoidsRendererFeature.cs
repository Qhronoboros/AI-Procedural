// Source Git-Amend https://www.youtube.com/watch?v=v_WkGKn601M

using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Experimental.Rendering;

public class BoidsRendererFeature : ScriptableRendererFeature
{
    class BoidsPass : ScriptableRenderPass
    {
        ComputeShader computeShader;
        int kernel;

        GraphicsBuffer boidsBuffer;
        Vector3[] boidPositions;
        Vector3[] boidVelocities;

        int boidCount = 32;

        RTHandle boidsHandle;
        int width = 256, height = 256;

        public RTHandle Boids => boidsHandle;

        public void Setup(ComputeShader cs)
        {
            computeShader = cs;
            kernel = cs.FindKernel("CSMain");

            if (boidsHandle == null || boidsHandle.rt.width != width || boidsHandle.rt.height != height)
            {
                boidsHandle?.Release();

                var desc = new RenderTextureDescriptor(width, height, GraphicsFormat.R32_SFloat, 0)
                {
                    enableRandomWrite = true,
                    msaaSamples = 1,
                    sRGB = false,
                    useMipMap = false
                };

                boidsHandle = RTHandles.Alloc(desc, name: "_BoidsRT");
            }

            if (boidsBuffer == null || boidsBuffer.count != boidCount)
            {
                boidsBuffer?.Release();
                boidsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, boidCount, sizeof(float) * 6);
                boidPositions = new Vector3[boidCount];
                boidVelocities = new Vector3[boidCount];
            }
        }

        class PassData
        {
            public ComputeShader compute;
            public int kernel;
            public TextureHandle output;
            public BufferHandle boidHandle;
            public int boidCount;
        }

        public override void RecordRenderGraph(RenderGraph graph, ContextContainer context)
        {
            
        }
    }

    public override void Create()
    {
        
    }
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }
}
