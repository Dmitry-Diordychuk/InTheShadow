using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace InTheShadow.CustomPostProcessing
{
    public class BlackAndWhiteRenderPassFeature : ScriptableRendererFeature
    {
        private class BlackAndWhiteRenderPass : ScriptableRenderPass
        {
            private RenderTargetIdentifier _source;

            private readonly Material _material;
            private RenderTargetHandle _tempRTHandler;

            public BlackAndWhiteRenderPass(Material material)
            {
                this._material = material;
                _tempRTHandler.Init("_TemporaryColorTexture");
            }

            public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
            {
                _source = renderingData.cameraData.renderer.cameraColorTarget;
                cmd.GetTemporaryRT(_tempRTHandler.id, renderingData.cameraData.cameraTargetDescriptor);
            }
            
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                CommandBuffer commandBuffer = CommandBufferPool.Get("CustomBlitRenderPass");
                
                Blit(commandBuffer, _source, _tempRTHandler.Identifier(), _material);
                Blit(commandBuffer, _tempRTHandler.Identifier(), _source);

                context.ExecuteCommandBuffer(commandBuffer);
                CommandBufferPool.Release(commandBuffer);
            }
        }

        [Serializable]
        public class Settings
        {
            public Material material = null;
        }

        public Settings settings = new Settings();

        private BlackAndWhiteRenderPass _blackAndWhitePass;

        public override void Create()
        {
            _blackAndWhitePass = new BlackAndWhiteRenderPass(settings.material)
            {
                renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing
            };
        }
        
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(_blackAndWhitePass);
        }
    }
}


