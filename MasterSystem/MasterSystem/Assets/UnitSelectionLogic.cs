using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// Pretty shitty way, but this is good for now.
// The correct way would be to query the server for the status of the player's group and get the info and display
// too much backend work for that for this master system prototype

public class UnitSelectionLogic : MonoBehaviour {

	// index: 0
	public Image top;
	public string top_unit_id;

	// index: 1
	public Image mid;
	public string mid_unit_id;

	// index: 2
	public Image bot;
	public string bot_unit_id;

	public string focused_id
	{
		get { return _unit_ids[_unit_ordering[1]]; }
	}

	private Sprite[] _unit_sprites;
	private string[] _unit_ids;
	private int[] _unit_ordering;

	private MasterSystem _ms;

	void Awake()
	{
		_ms = this.GetComponent<MasterSystem>();
		_unit_sprites = new Sprite[3]{top.sprite, mid.sprite, bot.sprite};
		_unit_ids = new string[3]{top_unit_id, mid_unit_id, bot_unit_id};
		_unit_ordering = new int[3]{0,1,2};
		SetId();
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		SetId();
	}

	private void Swap<T>(T[] data, int a, int b) {
		T temp = data[a];
		data[a] = data[b];
		data[b] = temp;
	}

	private void Set(int index)
	{
		if(index == 0)
		{
			top.sprite = _unit_sprites[_unit_ordering[index]];
			top_unit_id = _unit_ids[_unit_ordering[index]];
		}
		else if(index == 1)
		{
			mid.sprite = _unit_sprites[_unit_ordering[index]];
			mid_unit_id = _unit_ids[_unit_ordering[index]];
		}
		else if(index == 2)
		{
			bot.sprite = _unit_sprites[_unit_ordering[index]];
			bot_unit_id = _unit_ids[_unit_ordering[index]];
		}
	}

	public void MoveUp()
	{
		Swap <int>(_unit_ordering, 0, 1);
		Swap <int>(_unit_ordering, 1, 2);
			
		Set(0);
		Set(1);
		Set(2);
	}

	public void MoveDown()
	{
		Swap <int>(_unit_ordering, 0, 2);
		Swap <int>(_unit_ordering, 1, 2);
		
		Set(0);
		Set(1);
		Set(2);
	}

	public void SetId()
	{
		_ms.SetFocusUnitId(focused_id);
	}
}
