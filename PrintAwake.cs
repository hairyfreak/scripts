// The PrintAwake script is placed on a GameObject.  The Awake function is
// called when the GameObject is started at runtime.  The script is also
// called by the Editor.  An example is when the scene is changed to a
// different scene in the Project window.
// The Update() function is called, for example, when the GameObject transform
// position is changed in the Editor.

// findme ref: from https://docs.unity3d.com/ScriptReference/ExecuteInEditMode.html

// finde notes: runs very frequently and difficult to control  better to use an empty game object with custom button.
// kept here only as a reminder


using UnityEngine;

[ExecuteInEditMode]
public class PrintAwake : MonoBehaviour
{
	/*
	void Awake()
	{
		Debug.Log("Editor causes this Awake");
	}

	void Update()
	{
		Debug.Log("Editor causes this Update");
	}

	void OnGUI()
	{
		Debug.Log("Editor causes this GUI " + Event.current);

	}
	*/
}