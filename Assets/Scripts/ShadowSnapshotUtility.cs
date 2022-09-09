using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InTheShadow
{
	public static class ShadowSnapshotUtility
	{
		[Serializable]
		private struct RenderTextureFormat
		{
			public TextureFormat textureFormat;
			public int width;
			public int height;
		}
		
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

		public static Texture2D LoadSnapshotFromRawData(string relatedPathToFile)
		{
			byte[] bytes = File.ReadAllBytes(relatedPathToFile);
			int size = (int)Math.Sqrt(bytes.Length / 3); // RGB
			Texture2D snapshot = new Texture2D(size, size, TextureFormat.RGB24, false);
			snapshot.LoadRawTextureData(bytes);
			snapshot.Apply();
			return snapshot;
		}

		public static float CompareSnapshots(Texture2D a, Texture2D b)
		{
			if (a.width != b.width || a.height != b.height)
			{
				return -1;
			}

			Color[] aPixels = a.GetPixels();
			Color[] bPixels = b.GetPixels();

			int wrongPixelsCounter = 0;
			for (int i = 0; i < aPixels.Length; i++)
			{
				if (aPixels[i] != bPixels[i])
				{
					wrongPixelsCounter++;
				}
			}

			int totalPixels = a.width * a.height;
			return 1 - (float)wrongPixelsCounter / totalPixels;
		}
	}
}