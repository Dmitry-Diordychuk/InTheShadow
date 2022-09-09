using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InTheShadow
{
	public static class ShadowSnapshotUtility
	{
		public static Texture2D GetShadowSnapshot(RenderTexture renderTexture)
		{
			RenderTexture.active = renderTexture;
			Texture2D snapshot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
			snapshot.ReadPixels(new Rect(0, 0, snapshot.width, snapshot.height), 0, 0);
			return snapshot;
		}

		public static void SaveSnapshotToPNG(Texture2D snapshot, string relatedPath, string filename)
		{
			byte[] bytes = snapshot.EncodeToPNG();
			File.WriteAllBytes(Path.Combine(relatedPath, filename), bytes);
		}

		public static void SaveSnapshotAsRawData(Texture2D snapshot, string relatedPath, string filename)
		{
			byte[] bytes = snapshot.GetRawTextureData();
			File.WriteAllBytes(Path.Combine(relatedPath, filename), bytes);
		}
	}
}