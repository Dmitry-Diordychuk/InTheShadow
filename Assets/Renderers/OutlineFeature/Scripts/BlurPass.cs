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
        private RTHandle _tmpBlurRT1;
        private RTHandle _tmpBlurRT2;
        
        private readonly RTHandle _source;
        private RTHandle _destination;
        
        private readonly int _passesCount;
        private readonly int _downSample;
        private readonly Material _blurMaterial;
        
        public BlurPass(RTHandle source, RTHandle destination, Material blurMaterial, int downSample, int passesCount)
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

            _tmpBlurRT1 = RTHandles.Alloc(blurTextureDesc, FilterMode.Bilinear, name: "_TempBlurTexture1");
            _tmpBlurRT2 = RTHandles.Alloc(blurTextureDesc, FilterMode.Bilinear, name: "_TempBlurTexture2");

            cmd.GetTemporaryRT(Shader.PropertyToID(_destination.name), blurTextureDesc);
            ConfigureTarget(_destination);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get("BlurPass");
            
            if (_passesCount > 0)
            {
                cmd.Blit(_source.nameID, _tmpBlurRT1.nameID, _blurMaterial, 0);
                for (int i = 0; i < _passesCount - 1; i++)
                {
                    cmd.Blit(_tmpBlurRT1.nameID, _tmpBlurRT2.nameID, _blurMaterial, 0);
                    (_tmpBlurRT1, _tmpBlurRT2) = (_tmpBlurRT2, _tmpBlurRT1);
                }
                cmd.Blit(_tmpBlurRT1.nameID, _destination.nameID);
            }
            else
                cmd.Blit(_source.nameID, _destination.nameID);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
