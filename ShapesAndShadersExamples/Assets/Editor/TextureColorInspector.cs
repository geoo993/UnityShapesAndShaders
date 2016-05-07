using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(TextureColorBasedOnWorldPosition))]

public class TextureColorInspector : Editor {

	private TextureColorBasedOnWorldPosition creator;

	private void OnEnable () {
		creator = target as TextureColorBasedOnWorldPosition;
		Undo.undoRedoPerformed += RefreshCreator;
	}

	private void OnDisable () {
		Undo.undoRedoPerformed -= RefreshCreator;
	}

	private void RefreshCreator () {
		if (Application.isPlaying) {
			creator.FillTexture();
		}
	}


	public override void OnInspectorGUI () {
		EditorGUI.BeginChangeCheck();
		DrawDefaultInspector();
		if (EditorGUI.EndChangeCheck()) {
			RefreshCreator();
		}
	}


}
