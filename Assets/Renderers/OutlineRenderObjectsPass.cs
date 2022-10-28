using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace InTheShadow
{
    public class OutlineRenderObjectsPass : ScriptableRenderPass
    {
        private RTHandle _destination;

        private readonly List<ShaderTagId> _shaderTagIdList = new List<ShaderTagId>() {
            new ShaderTagId("UniversalForward") 
        };
        
        private FilteringSettings _filteringSettings;
        private RenderStateBlock _renderStateBlock;

        private readonly Material _overrideMaterial;
        
        public OutlineRenderObjectsPass(RTHandle destination, int layerMask, Material overrideMaterial)
        {
            _destination = destination;

            _overrideMaterial = overrideMaterial;
            
            _filteringSettings = new FilteringSettings(RenderQueueRange.opaque, layerMask);
            _renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            cmd.GetTemporaryRT(Shader.PropertyToID(_destination.name), cameraTextureDescriptor);
            
            ConfigureTarget(_destination);
            ConfigureClear(ClearFlag.All, Color.clear);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            SortingCriteria sortingCriteria = renderingData.cameraData.defaultOpaqueSortFlags;
            DrawingSettings drawingSettings = CreateDrawingSettings(
                _shaderTagIdList,
                ref renderingData,
                sortingCriteria
            );
            drawingSettings.overrideMaterial = _overrideMaterial;
            
            context.DrawRenderers(
                renderingData.cullResults,
                ref drawingSettings,
                ref _filteringSettings,
                ref _renderStateBlock
            );
        }
    }
}
