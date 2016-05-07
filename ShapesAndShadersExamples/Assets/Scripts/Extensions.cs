using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//It is common to create a class to contain all of your
//extension methods. This class must be static.
public static class ExtensionMethods
{
	//Even though they are used like normal methods, extension
	//methods must be declared static. Notice that the first
	//parameter has the 'this' keyword followed by a Transform
	//variable. This variable denotes which class the extension
	//method becomes a part of.
	public static void ResetTransformation(this Transform trans)
	{
		trans.position = Vector3.zero;
		trans.localRotation = Quaternion.identity;
		trans.localScale = new Vector3(1, 1, 1);
	}

	public static Color RandomColor() {
		return new Color (Random.value, Random.value, Random.value, 1.0f);
	}

	public static void MaterialColorToRandom( this Material MaterialToChange)
	{
		MaterialToChange.color = new Color (Random.value, Random.value, Random.value, 1.0f);

	}
	public static void shuffle(List<int> arr)
	{
		// Knuth shuffle algorithm :: courtesy of Wikipedia :)
		for (int t = 0; t < arr.Count; t++ )
		{
			int tmp = arr[t];
			int r = Random.Range(t, arr.Count);
			arr[t] = arr[r];
			arr[r] = tmp;
		}
	}

}