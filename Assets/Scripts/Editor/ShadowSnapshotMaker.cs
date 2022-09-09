using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InTheShadow.Editor
{
    public class ShadowSnapshotMaker : EditorWindow
    {
        private RenderTexture _renderTexture;

        [MenuItem("Window/Custom/ShadowSnapshotMaker")]
        private static void ShowWindow()
        {
            GetWindow(typeof(ShadowSnapshotMaker));
        }
        
        private readonly List<string> _errors = new();
        private void OnGUI()
        {
            GUILayout.Label("Shadow Snapshot Maker", EditorStyles.boldLabel);
            _renderTexture =
                (RenderTexture)EditorGUILayout.ObjectField("Render Texture", _renderTexture, typeof(RenderTexture),
                    false);

            ActualizeErrorsList();
            using (new GUIEnabledScope(_errors.Count == 0))
            {
                if (GUILayout.Button("Save"))
                {
                    MakeSnapshot(_renderTexture);
                }
            }
            
            if (_errors.Count > 0)
            {
                EditorGUILayout.HelpBox(string.Join('\n', _errors), MessageType.Error);
            }
        }

        private readonly ref struct GUIEnabledScope
        {
            private readonly bool _oldEnabled;

            public GUIEnabledScope(bool newEnabled)
            {
                _oldEnabled = GUI.enabled;
                GUI.enabled = newEnabled;
            }

            public void Dispose()
            {
                GUI.enabled = _oldEnabled;
            }
        }
        private void ActualizeErrorsList()
        {
            _errors.Clear();
            if (!_renderTexture) _errors.Add("There must be render texture");
        }
        
        private void MakeSnapshot(RenderTexture renderTexture)
        {
            RenderTexture.active = renderTexture;
            
            Texture2D snapshot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
            
            snapshot.ReadPixels(new Rect(0, 0, snapshot.width, snapshot.height), 0, 0);

            byte[] bytes = snapshot.EncodeToPNG();
            System.IO.File.WriteAllBytes($"Assets/Resources/Snapshots/{SceneManager.GetActiveScene().name}_snapshot.png", bytes);
        }
    }
}
