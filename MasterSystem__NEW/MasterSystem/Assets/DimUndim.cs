using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DimUndim : MonoBehaviour {

	public void Dim(Image img)
	{
		img.color = Color.gray;
	}

	public void UnDim(Image img)
	{
		img.color = Color.white;
	}
}
