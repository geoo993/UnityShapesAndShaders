using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(TextureGenerator))]

public class TextureCreatorInspector : Editor {

	private TextureGenerator creator;

	private void OnEnable () {
		creator = target as TextureGenerator;
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

//	public override void OnInspectorGUI () {
//
//		EditorGUI.BeginChangeCheck();
//
//		DrawDefaultInspector();
//
//		if (EditorGUI.EndChangeCheck() && Application.isPlaying) {
//			(target as TextureGenerator).FillTexture();
//		}
//	}

	public override void OnInspectorGUI () {
		EditorGUI.BeginChangeCheck();
		DrawDefaultInspector();
		if (EditorGUI.EndChangeCheck()) {
			RefreshCreator();
		}
	}


}
