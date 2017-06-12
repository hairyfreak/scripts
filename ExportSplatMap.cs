using UnityEngine;
using System.Collections;
using SimpleJSON;
using Unity.IO.Compression;

// findme ref: adapted from https://docs.unity3d.com/ScriptReference/TerrainData.GetAlphamaps.html

public class ExportSplatMap : MonoBehaviour
{
	public Terrain terrain;
	public float noiseScale = 1;

	public ArrayList treeArray, detailArray;	// empty arrays to load terrain info in for export

	[System.Serializable]
	public class Row
	{
		public int[] rowData;

	}

	[System.Serializable]
	public class MapTable
	{
		public Row[] rows;
	}
		

	public string submitURL = "localhost:8888/shub1Env-test/decode.php";		// the receiving page on the server

//	public 	test mytest = new test(10);		// create new class and make sue of the consutctor line to pass paramater  NB doesn't seem to be wokring



	// findme ref: based on https://docs.unity3d.com/ScriptReference/WWWForm.html

	IEnumerator UploadJSON(string type, string format, string json, string terrainID) {

		// create form with named key-val pairs for $_post

		// findme todo: add routines for handling different data types
		WWWForm form = new WWWForm();
		form.AddField("type", "trees");
		form.AddField("format", "json");
		form.AddField("data", json);
		form.AddField("terrainID",terrainID);	//ID of the terrain in the database

		// use for heightmap/splatmap/zip		form.AddBinaryData("fileUpload", bytes, "screenShot.png", "image/png");


		WWW w = new WWW(submitURL, form);
		yield return w;
		if (!string.IsNullOrEmpty(w.error)) {
			print(w.error);
		}
		else {
			print("Finished Uploading Data " + json);
		}

	}



	// wrap a json string of arrays in [] to make 1 json array
	private string wrapJson(string json) {
		json = "[" + json + "]";
		return json;
	}

	public void uploadTrees()
	{
		//		print ("splat layers " + terrain.terrainData.alphamapLayers);
		treeArray = new ArrayList(terrain.terrainData.treeInstances);
		TreeInstance[] trees = terrain.terrainData.treeInstances;


		print ("treearray count " + trees.Length );
		//	print ("tree instance count " + t.terrainData.treeInstanceCount);


		string jsontrees = "";										// initialise tree data string


		// findme note: c# equivalent of for X as n. need the counter anwyay, not that relevant 
		/*
		foreach (TreeInstance tree in trees) {
			string jsontree = JsonUtility.ToJson (tree);
			if (i < (treeArray.Count - 1)) {
				jsontrees = jsontrees + ",";						
			}

		}
		*/


		for (int i=0; i < trees.Length; i++)
		{
			
			trees[i].position = Vector3.Scale(trees[i].position, terrain.terrainData.size);			// convert the tree position to world units
			
			string jsontree = JsonUtility.ToJson (trees[i]);										// encode as json
			jsontrees = jsontrees + jsontree;														// build json string
			if (i < (trees.Length - 1)) {															// add seperating comma
				jsontrees = jsontrees + ",";														// add a comma after any entry that isn't the last entry
			}
		}
		



		print("json trees " + wrapJson(jsontrees) );


		//string jsonTerrainSize = JsonUtility (terrain.terrainData.size);
		//print (jsonTerrainSize);

		

		// create form object with data to upload.

		StartCoroutine(UploadJSON("trees","json", wrapJson(jsontrees), terrain.name));

		System.IO.File.WriteAllText ("/Users/hairyfreak/Desktop/trees.txt", wrapJson(jsontrees));


	}




	public void uploadSplat()
	{
		

		//  GetDetailLayer(int xBase, int yBase, int width, int height, int layer);
		// in otherwords get detail layer from 0,0  layer id (see editor list order)
		var map = terrain.terrainData.GetDetailLayer(0, 0, terrain.terrainData.detailWidth, terrain.terrainData.detailHeight, 0);

		//string[] rows = new string[10]; 	// make new array of strings as long as the map is tall

		Row[] rows = new Row[terrain.terrainData.detailHeight];
		string csvString = "x;y;val\n";

		Texture2D texDetail = new Texture2D (terrain.terrainData.detailHeight, terrain.terrainData.detailWidth);		// 2D texture to store terrain data for convesion to png

		for (int y = 0; y < terrain.terrainData.detailHeight; y++) {			// iterate across rows detail map to the max dimensions of the detail map


			int[] cells = new int[terrain.terrainData.detailWidth];				// make new array of floats as long as the map is wide (one created for every row)
																					// terrain splat data is in range 0-1

			for (int x = 0; x < terrain.terrainData.detailWidth; x++) {			// iterate across columns to max dimensions of detail map
				cells[x] = map[x,y];											// add pixel value to array

				Color color = new Color(map[x,y],map[x,y],map[x,y]);
				texDetail.SetPixel(x, y, color);
//				csvString = csvString + x + ";" + y + ";" + map[x,y] + "\n";	// seems to crash

			}

			Row row = new Row();

			row.rowData = cells;

			rows[y] = row;

		}

		texDetail.Apply();	// applies setpixel changes to texture on GPU.  findme note: not sure if needed when not rendering the texture?

		byte[] byteDump = texDetail.EncodeToPNG();			// encode texture to PNG for sending
		System.IO.File.WriteAllBytes ("/Users/hairyfreak/Desktop/splatDumpNew.png", byteDump);



	//	print ("csv string " + csvString);
			
		string mapJSON = JsonHelper.ToJson<Row>(rows);			// generate final JSON string.
		print (mapJSON);

		MapTable ImageMap = new MapTable();
		ImageMap.rows = rows;

		string mapJSON2 = JsonUtility.ToJson(ImageMap, true);			// generate final JSON string with json utility to prettify
		print (mapJSON2);

		System.IO.File.WriteAllText ("/Users/hairyfreak/Desktop/splatDump.txt", mapJSON);
	//	System.IO.FileStream zipFile = System.IO.File.Create ("/Users/hairyfreak/Desktop/splatDump.zip");
		/*	
			//System.IO.File.OpenWrite ("/Users/hairyfreak/Desktop/splatDump.zip");
			System.IO.MemoryStream ms = new System.IO.MemoryStream();  
			

			using (GZipStream mapGzip = new GZipStream (zipFile, CompressionMode.Compress)) {
				// mapJSON.CopyTo (mapGzip);  findme fail: stream.copyto seems not to be fully supported.
			}
		*/

		//GZipStream gzs = new GZipStream (zipFile, CompressionMode.Compress);


		byte[] bytes = System.Text.Encoding.UTF8.GetBytes (mapJSON);

		using (System.IO.FileStream zipFile = System.IO.File.Create("/Users/hairyfreak/Desktop/splatDump.gz"))
		{
			using (GZipStream compressionStream = new GZipStream(zipFile, CompressionMode.Compress))
			{
				compressionStream.Write(bytes, 0, bytes.Length);
			}
		}


	//	string mapJSON = JsonHelper.ToJson<string>(rows);			// generate final JSON string.


	//	string jsonLayerMap = JsonHelper.ToJson<int>(map);
	//	print ("json layer map" + jsonLayerMap);

	// in otherwords get detail layer from 0,0  layer id, fill with an array of intensity values
		terrain.terrainData.SetDetailLayer(0, 0, 0, map);			// assign modified map back to terrain.




	//	float[,,] maps = t.terrainData.GetAlphamaps(0, 0, t.terrainData.alphamapWidth, t.terrainData.alphamapHeight);

		// iterate through row of terrain splat map 
		/*
		for (int y = 0; y < t.terrainData.alphamapHeight; y++)
		{
			print (y);
			// iterate through columns of entire terrain
			for (int x = 0; x < t.terrainData.alphamapWidth; x++)
			{
				print (x);
				float a0 = maps[x, y, 0];
				float a1 = maps[x, y, 1];

				a0 += Random.value * noiseScale;
				a1 += Random.value * noiseScale;

				float total = a0 + a1;

				maps[x, y, 0] = a0 / total;
				maps[x, y, 1] = a1 / total;
			}
		}

		t.terrainData.SetAlphamaps(0, 0, maps); */
	}
}