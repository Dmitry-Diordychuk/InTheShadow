using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace InTheShadow
{
	public static class SnapshotUtility
	{
		[Serializable]
		private struct RenderTextureFormat
		{
			public TextureFormat textureFormat;
			public int width;
			public int height;
		}
		
		[Serializable]
		public struct SnapshotData
		{
			public int width;
			public int height;
			public TextureFormat textureFormat;
			public byte[] rawTextureData;
			public List<Quaternion> rotations;
		}
		
		public static Texture2D GetShadowSnapshot(RenderTexture renderTexture)
		{
			RenderTexture.active = renderTexture;
			Texture2D snapshot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
			snapshot.ReadPixels(new Rect(0, 0, snapshot.width, snapshot.height), 0, 0);
			return snapshot;
		}

		public static void SaveSnapshotToPNG(Texture2D snapshot, string filePath)
		{
			byte[] bytes = snapshot.EncodeToPNG();
			File.WriteAllBytes(filePath + ".png", bytes);
		}

		public static void SaveSnapshotAsRawData(Texture2D snapshot, List<Quaternion> rotations, string relatedPath, string filename)
		{
			SnapshotData snapshotData;
			snapshotData.width = snapshot.width;
			snapshotData.height = snapshot.height;
			snapshotData.textureFormat = snapshot.format;
			snapshotData.rawTextureData = snapshot.GetRawTextureData();
			snapshotData.rotations = rotations;

			string filePathWithNumber = GenerateFileNumber(Path.Combine(relatedPath, filename));
			using Stream stream = new FileStream(filePathWithNumber, FileMode.CreateNew, FileAccess.Write, FileShare.None);
			//IFormatter formatter = GetBinaryFormatter();
			//formatter.Serialize(stream, snapshotData);

			ISerializer binarySerialize = new BinaryJsonSerializer();
			binarySerialize.Serialize(stream, snapshotData);

			SaveSnapshotToPNG(snapshot, filePathWithNumber);
		}

		private static string GenerateFileNumber(string filePath)
		{
			int i = 1;
			string filePathWithNumber = filePath + "_" + i;
			while (File.Exists(filePathWithNumber))
			{
				i++;
				filePathWithNumber = filePath + "_" + i;
			}

			return filePathWithNumber;
		}

		public static (List<Texture2D>, List<Quaternion>) LoadSnapshotFromRawData(string relatedPathToFile)
		{
			List<Texture2D> snapshots = new List<Texture2D>();
			List<Quaternion> rotations = new List<Quaternion>();

			int i = 1;
			string filePathWithNumber = relatedPathToFile + "_" + i;
			while (File.Exists(filePathWithNumber))
			{
				using Stream stream = new FileStream(filePathWithNumber, FileMode.Open, FileAccess.Read, FileShare.Read);
				ISerializer binarySerialize = new BinaryJsonSerializer();
				// IFormatter formatter = GetBinaryFormatter();
				SnapshotData snapshotData = binarySerialize.Deserialize<SnapshotData>(stream);
				
				snapshots.Add(new Texture2D(snapshotData.width, snapshotData.height, snapshotData.textureFormat, false));
				snapshots.Last().LoadRawTextureData(snapshotData.rawTextureData);
				rotations.AddRange(snapshotData.rotations);
				snapshots.Last().Apply();

				i++;
				filePathWithNumber = relatedPathToFile + "_" + i;
			}
			
			return (snapshots, rotations);
		}
		
		private static BinaryFormatter GetBinaryFormatter()
		{
			BinaryFormatter formatter = new BinaryFormatter();
			SurrogateSelector selector = new SurrogateSelector();
			
			QuaternionSerializationSurrogate quaternionSurrogate = new QuaternionSerializationSurrogate();
			
			selector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All),
				quaternionSurrogate);

			formatter.SurrogateSelector = selector;

			return formatter;
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