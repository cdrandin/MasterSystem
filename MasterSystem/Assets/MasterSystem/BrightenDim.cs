using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BrightenDim : MonoBehaviour 
{
	private Image _img;

	public void Brighten()
	{
		_img.color = Color.white;
	}

	public void Dim()
	{
		_img.color = Color.gray;
	}

	// Use this for initialization
	void Start () {
		_img = this.GetComponent<Image>();
		Dim();
	}
}
