﻿using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace InTheShadow.CustomPostProcessing
{
    public class BlackAndWhiteRenderPassFeature : ScriptableRendererFeature
    {
        class BlackAndWhiteRenderPass : ScriptableRenderPass
        {
            private RTHandle _source;

            private readonly Material _material;
            private readonly RTHandle _tempRTHandler;

            public BlackAndWhiteRenderPass(Material material)
            {
                this._material = material;
                _tempRTHandler = RTHandles.Alloc("_TemporaryColorTexture", name: "_TemporaryColorTexture");
            }

            public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
            {
                _source = renderingData.cameraData.renderer.cameraColorTargetHandle;
                cmd.GetTemporaryRT(Shader.PropertyToID(_tempRTHandler.name), renderingData.cameraData.cameraTargetDescriptor);
            }
            
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                CommandBuffer commandBuffer = CommandBufferPool.Get("CustomBlitRenderPass");
                
                Blit(commandBuffer, _source, _tempRTHandler, _material);
                Blit(commandBuffer, _tempRTHandler, _source);

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

        BlackAndWhiteRenderPass _blackAndWhitePass;

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


