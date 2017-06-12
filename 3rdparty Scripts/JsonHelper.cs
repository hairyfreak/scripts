using UnityEngine;
using System.Collections;


//findme ref: from https://stackoverflow.com/questions/38390274/unity-c-how-to-convert-an-array-of-a-class-into-json




public class JsonHelper
{

	public static T[] FromJson<T>(string json)
	{
		Wrapper<T> wrapper = UnityEngine.JsonUtility.FromJson<Wrapper<T>>(json);
		return wrapper.Items;
	}

	public static string ToJson<T>(T[] array)
	{
		Wrapper<T> wrapper = new Wrapper<T>();
		wrapper.Items = array;
		return UnityEngine.JsonUtility.ToJson(wrapper);
	}

	//findme patch: edited the serializable as notation has changed during unity versions
	[System.Serializable]
	private class Wrapper<T>
	{
		public T[] Items;
	}

}



/*

//findme ref: from http://answers.unity3d.com/questions/1290561/how-do-i-go-about-deserializing-a-json-array.html
// & https://docs.unity3d.com/ScriptReference/JsonUtility.FromJson.html

public class JsonHelper
{
	public static T[] getJsonArray<T>(string json)
	{
		string newJson = "{ \"array\": " + json + "}";
		Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
		return wrapper.array;
	}

	[System.Serializable]
	private class Wrapper<T>
	{
		public T[] array;
	}
}

*/