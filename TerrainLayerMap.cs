using UnityEngine;
using System.Collections;


// custom class to hold terrain data in a way that can be serialized for export to JSON

// findme ref: from http://answers.unity3d.com/questions/53038/visializing-a-multidimensional-array.html


[System.Serializable]
public class TerrainLayerMap{
	public int[] vals;

	/*
	[System.Serializable]
	public struct rowData {
		public int[] cell;
	}

	// initialize empty array of cells
	// public rowData[] cells = new rowData[] {};*/
}
