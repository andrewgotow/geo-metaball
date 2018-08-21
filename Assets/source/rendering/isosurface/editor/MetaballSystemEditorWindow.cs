using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Rendering.Isosurface {

public class MetaballSystemEditorWindow : EditorWindow {
	[MenuItem ("Window/Metaball Settings")]
    public static void ShowWindow () {
        EditorWindow.GetWindow(typeof(MetaballSystemEditorWindow));
    }
    
    void OnGUI () {
		SerializedObject system = new SerializedObject( MetaballSystem.instance );
   		SerializedProperty trianglePixelSize = system.FindProperty("settings.trianglePixelSize");
   		SerializedProperty depthSlices = system.FindProperty("settings.depthSlices");
   		SerializedProperty clipDistance = system.FindProperty("settings.clipDistance");

		GUILayout.Label("Metaball Settings", EditorStyles.boldLabel);
		EditorGUILayout.PropertyField(trianglePixelSize, new GUIContent("Triangle Pixel Size"));
		EditorGUILayout.PropertyField(depthSlices, new GUIContent("Depth Slice Count"));
		EditorGUILayout.PropertyField(clipDistance, new GUIContent("Far Clip Distance"));

		system.ApplyModifiedProperties();
    }
}

}