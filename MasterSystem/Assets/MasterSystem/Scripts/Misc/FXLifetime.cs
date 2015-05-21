using UnityEngine;
using System.Collections;

public class FXLifetime : MonoBehaviour 
{
	public float lifetime = 2.0f;
	public FXDelaySet[] delaySets;

	void Start () 
	{
		this.StartCoroutine (Lifetime());

		foreach(FXDelaySet delaySet in delaySets)
		{
			this.StartCoroutine (DelayAppearance(delaySet.delay, delaySet.enableObject));
		}
	}
	
	private IEnumerator Lifetime()
	{
		yield return new WaitForSeconds (this.lifetime);

		if (this.gameObject != null)
			Destroy (this.gameObject);
	}

	private IEnumerator DelayAppearance(float delay, GameObject enableObject)
	{
		yield return new WaitForSeconds (delay);
		
		if (enableObject != null)
		{
			enableObject.SetActive(true);
		}
	}
}

[System.Serializable]
public class FXDelaySet
{
	public float delay = 0.0f;
	public GameObject enableObject = null;
}
