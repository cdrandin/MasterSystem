using UnityEngine;
using System.Collections;

public class UpdateItemPortrait : MonoBehaviour
{
	public MeshRenderer primary0_meshrender;
	public MeshRenderer primary1_meshrender;
	public MeshRenderer secondary0_meshrender;
	public MeshRenderer secondary1_meshrender;
	public MeshRenderer soulshard0_meshrender;

	private TimerBasedCombatSystem _combat_system;
	
	// Use this for initialization
	void Start () 
	{
		_combat_system = TimerBasedCombatSystem.instance;
//		StartCoroutine(DelayUpdate(1f/10f));
	}
	
	// Update is called once per frame
	void MyUpdate () 
	{
		if(_combat_system != null)
		{
			if(_combat_system.selected_unit != null)
			{
				if(_combat_system.selected_unit.primary_slot != null && _combat_system.selected_unit.primary_slot[0] != null)
				{
					primary0_meshrender.transform.parent.gameObject.SetActive(true);
					primary0_meshrender.material.mainTexture = _combat_system.selected_unit.primary_slot[0].item.item_image;
				}
				else
				{
					primary0_meshrender.transform.parent.gameObject.SetActive(false);
				}

				if(_combat_system.selected_unit.primary_slot != null && _combat_system.selected_unit.primary_slot[1] != null)
				{
					primary1_meshrender.transform.parent.gameObject.SetActive(true);
					primary1_meshrender.material.mainTexture = _combat_system.selected_unit.primary_slot[1].item.item_image;
				}
				else
				{
					primary1_meshrender.transform.parent.gameObject.SetActive(false);
				}


				if(_combat_system.selected_unit.secondary_slot != null && _combat_system.selected_unit.secondary_slot[0] != null)
				{
					secondary0_meshrender.transform.parent.gameObject.SetActive(true);
					secondary0_meshrender.material.mainTexture = _combat_system.selected_unit.secondary_slot[0].item.item_image;
				}
				else
				{
					secondary0_meshrender.transform.parent.gameObject.SetActive(false);
				}

				if(_combat_system.selected_unit.secondary_slot != null && _combat_system.selected_unit.secondary_slot[1] != null)
				{
					secondary1_meshrender.transform.parent.gameObject.SetActive(true);
					secondary1_meshrender.material.mainTexture = _combat_system.selected_unit.secondary_slot[1].item.item_image;
				}
				else
				{
					secondary1_meshrender.transform.parent.gameObject.SetActive(false);
				}


				if(_combat_system.selected_unit.soulshard_slot != null && _combat_system.selected_unit.soulshard_slot[0] != null)
				{
					soulshard0_meshrender.transform.parent.gameObject.SetActive(true);
					soulshard0_meshrender.material.mainTexture = _combat_system.selected_unit.soulshard_slot[0].item.item_image;
				}
				else
				{
					soulshard0_meshrender.transform.parent.gameObject.SetActive(false);
				}
			}
		}
	}

	public void ChangeItemPortrait()
	{
		MyUpdate();
	}

	IEnumerator DelayUpdate(float delay)
	{
		while(true)
		{
			MyUpdate();
			yield return new WaitForSeconds(delay);
		}
	}
}
