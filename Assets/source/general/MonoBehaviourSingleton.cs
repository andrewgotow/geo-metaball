using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
	using UnityEditor;
#endif

[ExecuteInEditMode]
public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour {

	private static T _instance;
	public static T instance{
		get {
			if (_instance == null) {
				_instance = (T)Object.FindObjectOfType(typeof(T));
			}
			/*
			if (_instance == null) {
				var go = new GameObject($"{typeof(T).Name} - Instance");
				_instance = go.AddComponent<T>();

				#if UNITY_EDITOR
					EditorApplication.RepaintHierarchyWindow();
				#endif
			}
			*/

			return _instance;
		}
	}

}
