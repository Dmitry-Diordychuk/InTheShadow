using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;

namespace InTheShadow
{
    public class BlurPass : ScriptableRenderPass
    {
        private readonly int _tmpBlurRTId1 = Shader.PropertyToID("_TempBlurTexture1");
        private readonly int _tmpBlurRTId2 = Shader.PropertyToID("_TempBlurTexture2");

        private RenderTargetIdentifier _tmpBlurRT1;
        private RenderTargetIdentifier _tmpBlurRT2;

        private readonly RenderTargetIdentifier _source;
        private RenderTargetHandle _destination;

        private readonly int _passesCount;
        private readonly int _downSample;
        private readonly Material _blurMaterial;


        public BlurPass(RenderTargetIdentifier source, RenderTargetHandle destination, Material blurMaterial, int downSample, int passesCount)
        {
            _source = source;
            _destination = destination;

            _blurMaterial = blurMaterial;
            _downSample = downSample;
            _passesCount = passesCount;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            var width = Mathf.Max(1, cameraTextureDescriptor.width >> _downSample);
            var height = Mathf.Max(1, cameraTextureDescriptor.height >> _downSample);
            var blurTextureDesc = new RenderTextureDescriptor(width, height, RenderTextureFormat.ARGB32, 0, 0);

            _tmpBlurRT1 = new RenderTargetIdentifier(_tmpBlurRTId1);
            _tmpBlurRT2 = new RenderTargetIdentifier(_tmpBlurRTId2);

            cmd.GetTemporaryRT(_tmpBlurRTId1, blurTextureDesc, FilterMode.Bilinear);
            cmd.GetTemporaryRT(_tmpBlurRTId2, blurTextureDesc, FilterMode.Bilinear);
            cmd.GetTemporaryRT(_destination.id, blurTextureDesc, FilterMode.Bilinear);

            ConfigureTarget(_destination.Identifier());
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get("BlurPass");

            if (_passesCount > 0)
            {
                cmd.Blit(_source, _tmpBlurRT1, _blurMaterial, 0);
                for (int i = 0; i < _passesCount - 1; i++)
                {
                    cmd.Blit(_tmpBlurRT1, _tmpBlurRT2, _blurMaterial, 0);
                    (_tmpBlurRT1, _tmpBlurRT2) = (_tmpBlurRT2, _tmpBlurRT1);
                }
                cmd.Blit(_tmpBlurRT1, _destination.Identifier());
            }
            else
                cmd.Blit(_source, _destination.Identifier());

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
        
        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(_destination.id);
            cmd.ReleaseTemporaryRT(_tmpBlurRTId1);
            cmd.ReleaseTemporaryRT(_tmpBlurRTId2);
        }
    }
}
