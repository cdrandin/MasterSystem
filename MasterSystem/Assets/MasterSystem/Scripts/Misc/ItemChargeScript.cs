using UnityEngine;
using System.Collections;

public class ItemChargeScript : MonoBehaviour 
{
	public Transform primary_item_transform;
	public Transform secondary_item_transform;
	public Transform soulshard_item_transform;

	public Transform charges_transform;
	private SpriteRenderer[] charges;
	private int _current;

	// Use this for initialization
	void Start () 
	{
		charges = charges_transform.GetComponentsInChildren<SpriteRenderer>();
		ResetCharges();
	}

	// No bound check, not very realiable in terms of relating to an abilities charge limit
	// max 3
	public void AddCharge(ITEM_STATE state)
	{
		// Determine pivot
		switch(state)
		{
		case ITEM_STATE.PRIMARY:
			charges_transform.position = primary_item_transform.position;
			break;
		case ITEM_STATE.SECONDARY:
			charges_transform.position = secondary_item_transform.position;
			break;
		case ITEM_STATE.SOULSHARD:
			charges_transform.position = soulshard_item_transform.position;
			break;
		}

		try
		{
			charges[_current++].enabled = true;
		}
		catch(System.Exception e)
		{
			Debug.Log(e);
		}
	}

	public void ResetCharges()
	{
		_current = 0;
		foreach(SpriteRenderer sprite in charges)
		{
			sprite.enabled = false;
		}
	}
}
