using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace InTheShadow
{
	public class OutlineRenderObjectsPass : ScriptableRenderPass
	{
		private RenderTargetHandle _destination;

		private readonly Material _overrideMaterial;

		private readonly List<ShaderTagId> _shaderTagIdList = new List<ShaderTagId>() { new ShaderTagId("UniversalForward") };
		private FilteringSettings _filteringSettings;
		private RenderStateBlock _renderStateBlock;

		public OutlineRenderObjectsPass(RenderTargetHandle destination, int layerMask, Material overrideMaterial)
		{
			_destination = destination;

			_overrideMaterial = overrideMaterial;

			_filteringSettings = new FilteringSettings(RenderQueueRange.opaque, layerMask);
			_renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
		}

		public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
		{
			cmd.GetTemporaryRT(_destination.id, cameraTextureDescriptor);
			ConfigureTarget(_destination.Identifier());
			ConfigureClear(ClearFlag.All, Color.clear);
		}

		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			SortingCriteria sortingCriteria = renderingData.cameraData.defaultOpaqueSortFlags;
			DrawingSettings drawingSettings = CreateDrawingSettings(_shaderTagIdList, ref renderingData, sortingCriteria);
			drawingSettings.overrideMaterial = _overrideMaterial;

			context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref _filteringSettings, ref _renderStateBlock);
		}

		public override void FrameCleanup(CommandBuffer cmd)
		{
			cmd.ReleaseTemporaryRT(_destination.id);
		}
	}
}