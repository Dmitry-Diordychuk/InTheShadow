using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InTheShadow
{
    public class SnapshotUtility : MonoBehaviour
    {
        [Serializable]
        public struct SnapshotData
        {
            public int width;
            public int height;
            public TextureFormat textureFormat;
            public byte[] rawTextureData;
            public List<Quaternion> rotations;
        }
        
        public ShadowProjectorCamera projectorCamera;
        public ShadowCasterGroup shadowCasterGroup;

        public ComputeShader comparisonShader;
        
        private RenderTexture _cameraRT;
        private ComputeBuffer _comparisonShaderBuffer;
        private float[] _comparisonResult;
        private int _kernel;
        private uint _threadsX;
        private uint _threadsY;
        

        private void Start()
        {
            if (!projectorCamera)
            {
                Debug.LogWarning("Projection camera render texture isn't set!", this);
            }

            if (!shadowCasterGroup)
            {
                Debug.LogWarning("Shadows casters missing!", this);
            }
            
            _kernel = comparisonShader.FindKernel ("CSMain");
            comparisonShader.GetKernelThreadGroupSizes(_kernel, out _threadsX, out _threadsY, out _);
            
            _cameraRT = projectorCamera.GetRenderTarget();

            _comparisonShaderBuffer = new ComputeBuffer(_cameraRT.width * _cameraRT.height, sizeof(float));
            comparisonShader.SetBuffer(_kernel, "Result", _comparisonShaderBuffer);
            comparisonShader.SetInt("Size", _cameraRT.width);
            
            comparisonShader.SetTexture(_kernel, "Camera", _cameraRT);
            
            _comparisonResult = new float[_cameraRT.width * _cameraRT.height];
        }

        public float CompareSnapshotWithProjection(Texture2D snapshot)
        {
            comparisonShader.SetTexture(_kernel, "Sample", snapshot);

            comparisonShader.Dispatch(
                _kernel, 
                (int)(_cameraRT.width / _threadsX), 
                (int)(_cameraRT.height / _threadsY), 
                1);
                
                
            _comparisonShaderBuffer.GetData(_comparisonResult);

            float sum = 0.0f;
            foreach (var value in _comparisonResult)
            {
                sum += value;
            }

            return 1.0f - sum / (_cameraRT.width * _cameraRT.height);
        }

        public (List<Texture2D>, List<List<Quaternion>>) LoadSnapshots(string relatedPathToFile)
        {
            List<Texture2D> snapshots = new List<Texture2D>();
            List<List<Quaternion>> rotations = new List<List<Quaternion>>();

            int i = 1;
            string filePathWithNumber = relatedPathToFile + "_" + i;
            while (File.Exists(filePathWithNumber))
            {

                using Stream stream = new FileStream(filePathWithNumber, FileMode.Open, FileAccess.Read, FileShare.Read);
                ISerializer binarySerialize = new BinaryJsonSerializer();
                SnapshotData snapshotData = binarySerialize.Deserialize<SnapshotData>(stream);
				
                snapshots.Add(new Texture2D(snapshotData.width, snapshotData.height, snapshotData.textureFormat, false));
                snapshots.Last().LoadRawTextureData(snapshotData.rawTextureData);
                snapshots.Last().Apply();
				
                rotations.Add(new List<Quaternion>());
                rotations.Last().AddRange(snapshotData.rotations);

                i++;
                filePathWithNumber = relatedPathToFile + "_" + i;
            }
			
            return (snapshots, rotations);
        }
        
        public void MakeSnapshot()
        {
            Texture2D snapshot = GetShadowSnapshot(projectorCamera.GetRenderTarget());

            string filename = $"{SceneManager.GetActiveScene().name}_snapshot";
            string path = "Assets/Resources/Snapshots";

            List<Quaternion> rotations = shadowCasterGroup.GetAllRotations();

            SaveSnapshotAsRawData(snapshot, rotations, path, filename);
        }
        
        private static Texture2D GetShadowSnapshot(RenderTexture renderTexture)
        {
            RenderTexture.active = renderTexture;
            Texture2D snapshot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
            snapshot.ReadPixels(new Rect(0, 0, snapshot.width, snapshot.height), 0, 0);
            return snapshot;
        }
        
        private static void SaveSnapshotToPNG(Texture2D snapshot, string filePath)
        {
            byte[] bytes = snapshot.EncodeToPNG();
            File.WriteAllBytes(filePath + ".png", bytes);
        }
        
        private static void SaveSnapshotAsRawData(Texture2D snapshot, List<Quaternion> rotations, string relatedPath, string filename)
        {
            SnapshotData snapshotData;
            snapshotData.width = snapshot.width;
            snapshotData.height = snapshot.height;
            snapshotData.textureFormat = snapshot.format;
            snapshotData.rawTextureData = snapshot.GetRawTextureData();
            snapshotData.rotations = rotations;

            string filePathWithNumber = GenerateFileNumber(Path.Combine(relatedPath, filename));
            using Stream stream = new FileStream(filePathWithNumber, FileMode.CreateNew, FileAccess.Write, FileShare.None);

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

        private void OnDestroy()
        {
            _comparisonShaderBuffer.Release();
        }
    }
}
