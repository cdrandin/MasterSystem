using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof(Image))]
public class EnemyArrowAnimationUI : MonoBehaviour 
{
	public Image image;

	[Range(.5f,5f)]
	public float expiration = 2f;

	private CFX_AutoRotate script;

	// Use this for initialization
	void Start () {
		script = this.GetComponent<CFX_AutoRotate>();
	
//		image.rectTransform.RotateAround(Vector3.left, Vector3.forward, 90);
		image.rectTransform.rotation = Quaternion.Euler(new Vector3(0f,0f,90f));
		Hide();
	}

	void OnEnable()
	{
		Show();
	}

	IEnumerator Kill(float delay)
	{
		yield return new WaitForSeconds(delay);

		Hide();
		image.rectTransform.rotation = Quaternion.Euler(new Vector3(0f,0f,90f));

		yield return null;
	}

	public void SetPosition(Vector3 pos)
	{
//		Vector2 viewportPoint = Camera.main.WorldToViewportPoint(pos);
		image.transform.position = new Vector3(pos.x, pos.y+13f, Camera.main.transform.position.z + 10f);
	}

	public void Show()
	{
		image.enabled = true;
		if(script != null)
			script.enabled = true;
		StartCoroutine(Kill(expiration));
	}

	public void Hide()
	{
		this.enabled = false;
		image.enabled = false;
		if(script != null)
			script.enabled = false;
	}
}
