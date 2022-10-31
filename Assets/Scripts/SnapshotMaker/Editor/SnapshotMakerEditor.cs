using UnityEditor;
using UnityEngine;

namespace InTheShadow.Editor
{
	// Custom Editor using SerializedProperties.
	// Automatic handling of multi-object editing, undo, and Prefab overrides.
	[CustomEditor(typeof(SnapshotMaker))]
	public class SnapshotMakerEditor : UnityEditor.Editor
	{
		private SerializedProperty _projectorCameraProp;
		private SerializedProperty _shadowCasterGroupProp;

		void OnEnable()
		{
			// Setup the SerializedProperties.
			_projectorCameraProp = serializedObject.FindProperty ("projectorCamera");
			_shadowCasterGroupProp = serializedObject.FindProperty ("shadowCasterGroup");
		}
		
		public override void OnInspectorGUI()
		{
			// Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
			serializedObject.Update ();
			
			EditorGUILayout.PropertyField (_projectorCameraProp, new GUIContent ("Projector Camera"));
			EditorGUILayout.PropertyField (_shadowCasterGroupProp, new GUIContent ("Shadow Casters"));
			
			if (GUILayout.Button("Save"))
			{
				(target as SnapshotMaker)?.MakeSnapshot();
			}
			
			// Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
			serializedObject.ApplyModifiedProperties ();
		}
	}
}