using UnityEditor;
using UnityEngine;

namespace InTheShadow.Editor
{
	// Custom Editor using SerializedProperties.
	// Automatic handling of multi-object editing, undo, and Prefab overrides.
	[CustomEditor(typeof(SnapshotUtility))]
	public class SnapshotUtilityEditor : UnityEditor.Editor
	{
		private SerializedProperty _projectorCameraProp;
		private SerializedProperty _shadowCasterGroupProp;
		private SerializedProperty _comparisonShaderProp;

		void OnEnable()
		{
			// Setup the SerializedProperties.
			_projectorCameraProp = serializedObject.FindProperty("projectorCamera");
			_shadowCasterGroupProp = serializedObject.FindProperty("shadowCasterGroup");
			_comparisonShaderProp = serializedObject.FindProperty("comparisonShader");
		}
		
		public override void OnInspectorGUI()
		{
			// Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
			serializedObject.Update ();
			
			GUILayout.Label ("Save", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField (_projectorCameraProp, new GUIContent ("Projector Camera"));
			EditorGUILayout.PropertyField (_shadowCasterGroupProp, new GUIContent ("Shadow Casters"));
			
			if (GUILayout.Button("Save"))
			{
				(target as SnapshotUtility)?.MakeSnapshot();
			}

			GUILayout.Space(10.0f);
			GUILayout.Label("Load", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField (_comparisonShaderProp, new GUIContent ("Comparison shader"));
			

			// Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
			serializedObject.ApplyModifiedProperties ();
		}
	}
}