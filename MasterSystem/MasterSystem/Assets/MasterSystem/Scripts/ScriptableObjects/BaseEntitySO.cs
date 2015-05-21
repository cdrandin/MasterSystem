using UnityEngine;
using System.Collections;

[System.Serializable]
public class BaseEntitySO 
{
	private const int MAX_INT = 10000;
	
	public string name;
	
	[Range(0,MAX_INT)]
	public int _max_hp;
	
	[Range(0,MAX_INT)]
	private int _hp;
	public int hp
	{
		get { return  _hp; }
		set { _hp = Mathf.Clamp(value, 0, _max_hp); }
	}
}
