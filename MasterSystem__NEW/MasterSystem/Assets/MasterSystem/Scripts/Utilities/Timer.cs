using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour {

	TextMesh textMesh;

	// Use this for initialization
	void Start () {
		textMesh = gameObject.GetComponent<TextMesh>();
		int count = 25;
		StartCoroutine("Countdown", count);
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public IEnumerator Countdown(int start){


		while (start != 0){
			print (start);
			textMesh.text = start.ToString();
			--start;

			yield return new WaitForSeconds(1.0f);
		}
		textMesh.text = "";
	}
}
