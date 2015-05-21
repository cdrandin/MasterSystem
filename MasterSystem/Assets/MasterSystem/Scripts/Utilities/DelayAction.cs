using UnityEngine;
using System.Collections;

public class DelayAction : MonoBehaviour
{
	private static DelayAction _instance;
	public static DelayAction instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = Camera.main.gameObject.AddComponent<DelayAction>();
			}

			return _instance;
		}
	}

	public Coroutine Delay(System.Action action, float delay)
	{
		return StartCoroutine(ToDelay(action, delay));
	}

	public Coroutine DelayInf(System.Action action, float delay, System.Func<bool> stop_condition)
	{
		return StartCoroutine(ToDelayInf(action, delay, stop_condition));
	}

	IEnumerator ToDelay(System.Action action, float delay)
	{
		yield return new WaitForSeconds(delay);
		action();
	}

	IEnumerator ToDelayInf(System.Action action, float delay, System.Func<bool> stop_condition)
	{
		while(true)
		{
			yield return new WaitForSeconds(delay);
			action();

			if(stop_condition())
			{
				Debug.Log("Stopping ToDelayInf");
				break;
			}
		}
	}


	// more specific delays

	void OnApplicationQuit()
	{
		StopAllCoroutines();
		Destroy(this);
	}
}
