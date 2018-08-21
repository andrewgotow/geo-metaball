using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectUtility {

	public static void SafeDestroy(Object obj)
	{
		if (Application.isEditor)
			Object.DestroyImmediate(obj);
		else
			Object.Destroy(obj);
	}

}
