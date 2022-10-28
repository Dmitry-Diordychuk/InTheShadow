using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace InTheShadow
{
	public class OutlineFeature : ScriptableRendererFeature
	{
		[SerializeField] private string renderTextureName;
		[SerializeField] private RenderSettings renderSettings;

		[Serializable]
		public class RenderSettings
		{
			public Material overrideMaterial = null;
			public LayerMask layerMask = 0;
		}

		[SerializeField] private string blurTextureName;
		[SerializeField] private BlurSettings blurSettings;

		[Serializable]
		public class BlurSettings
		{
			public Material blurMaterial;
			public int downSample = 1;
			public int passesCount = 1;
		}

		[SerializeField] private Material outlineMaterial;
		[SerializeField] private RenderPassEvent renderPassEvent;

		private RTHandle _renderTexture;
		private RTHandle _blurTexture;

		private OutlineRenderObjectsPass _renderPass;
		private BlurPass _blurPass;
		private OutlinePass _outlinePass;

		public override void Create()
		{
			_renderTexture = RTHandles.Alloc(renderTextureName, renderTextureName);
			_blurTexture = RTHandles.Alloc(blurTextureName, blurTextureName);

			_renderPass = new OutlineRenderObjectsPass(
				_renderTexture,
				renderSettings.layerMask,
				renderSettings.overrideMaterial
			);

			_blurPass = new BlurPass(
				_renderTexture,
				_blurTexture,
				blurSettings.blurMaterial,
				blurSettings.downSample,
				blurSettings.passesCount
			);

			_outlinePass = new OutlinePass(outlineMaterial);

			_renderPass.renderPassEvent = renderPassEvent;
			_blurPass.renderPassEvent = renderPassEvent;
			_outlinePass.renderPassEvent = renderPassEvent;
		}

		public override void AddRenderPasses(
			ScriptableRenderer renderer,
			ref RenderingData renderingData
		)
		{
			renderer.EnqueuePass(_renderPass);
			renderer.EnqueuePass(_blurPass);
			renderer.EnqueuePass(_outlinePass);
		}
	}
}