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
            Texture2D snapshot = ShadowSnapshotUtility.GetShadowSnapshot(renderTexture);
            
            ShadowSnapshotUtility.SaveSnapshotToPNG(snapshot, 
                "Assets/Resources/Snapshots", 
                $"{SceneManager.GetActiveScene().name}_snapshot.png");
            
            ShadowSnapshotUtility.SaveSnapshotAsRawData(snapshot,
                "Assets/Resources/Snapshots", 
                $"{SceneManager.GetActiveScene().name}_snapshot");
        }
    }
}
