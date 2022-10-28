using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace InTheShadow
{
    public class OutlinePass : ScriptableRenderPass
    {
        private readonly Material _material;

        public OutlinePass(Material material)
        {
            _material = material;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get("Outline");

            using (new ProfilingScope(cmd, new ProfilingSampler("Outline")))
            {
                var mesh = RenderingUtils.fullscreenMesh;
                cmd.DrawMesh(mesh, Matrix4x4.identity, _material, 0, 0);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
