using UnityEngine;
using System.Collections;

// Currently not working the way it is
//

[System.Serializable]
public class ItemProgression : MonoBehaviour
{
	private int _current_exp;
	public int current_exp
	{
		get { return _current_exp; }
	}
	private int _projected_exp;
	public int projected_exp
	{
		get { return _projected_exp; }
	}

	private int _current_lvl;
	public int current_lvl
	{
		get { return _current_lvl; }
	}
	private int _projected_lvl;
	public int projected_lvl
	{
		get { return (_projected_exp / MAX_EXP_PER_LEVEL) + _current_lvl; }
	}
	private bool _leveled;
	public bool leveled
	{
		get { return _leveled; }
	}

	private int _tier_counter;

	// The 0th index is the default
	[SerializeField, Range (1, 100)]
	private int[] _level_timeline;
	public int[] level_timeline
	{
		get { return _level_timeline; }
	}

	// The 0th index is the default
	[SerializeField]
	private Sprite[] _item_textures_per_level;
	public Sprite[] item_textures_per_level
	{
		get { return item_textures_per_level; }
	}

	// The 0th index is the default
	[SerializeField]
	private Sprite[] _item_scroll_textures;
	public Sprite[] item_scroll_textures
	{
		get { return _item_scroll_textures; }
	}

	[SerializeField, Range (1, 100)]
	private int _item_max_level;
	public int item_max_craft_level
	{
		get { return _item_max_level; }
	}

	[SerializeField]
	private BaseItem _current_item;
	public BaseItem current_item
	{
		get { return _current_item; }
	}

	[SerializeField]
	public ItemProgression prev_item_progress;

	[SerializeField]
	private ItemProgression _next_item_progress;
	public ItemProgression next_item_progress
	{
		get { return _next_item_progress; }
	}

	public bool transformable
	{
		get { return _next_item_progress != null; }
	}

	public bool ready_to_transform
	{
		get { return (transformable) ? _current_lvl >= _item_max_level : false; }
	}

	public Sprite current_portrait
	{
		get { return _item_textures_per_level[_tier_counter]; }
	}

	public Sprite current_scroll_portrait
	{
		get { return _item_scroll_textures[_tier_counter]; }
	}

	public int MAX_EXP_PER_LEVEL
	{
		get { return 3000; }
	}

	public float normalized_current_exp
	{
		get { return ((float)_current_exp)/((float)MAX_EXP_PER_LEVEL); }
	}

	public float normalized_projected_exp
	{
		get { return ((float)_projected_exp)/((float)MAX_EXP_PER_LEVEL); }
	}

	void Awake()
	{
		_current_exp = _projected_exp = _tier_counter = 0;
		_projected_lvl = _current_lvl = 1;
		_item_max_level = 15;
		//prev_item_progress = null;
	}

	void Start()
	{
		string message = "The arrays need to be the same size.";
		DebugUtils.Assert(_level_timeline.Length == _item_textures_per_level.Length, message);
		DebugUtils.Assert(_level_timeline.Length == _item_scroll_textures.Length, message);
		DebugUtils.Assert(_item_textures_per_level.Length == _item_scroll_textures.Length, message);
	}
	
	public ItemProgression(int[] level_timeline, Sprite[] textures, Sprite[] scroll_textures, int max_level, BaseItem current)
	{
		DebugUtils.Assert(level_timeline.Length == textures.Length);
		DebugUtils.Assert(level_timeline.Length == scroll_textures.Length);
		DebugUtils.Assert(textures.Length == scroll_textures.Length);

		_level_timeline 		 = level_timeline;
		_item_textures_per_level = textures;
		_item_scroll_textures    = scroll_textures;
		_item_max_level          = max_level;
		_current_item			 = current;
	}

	public void AddExp(int amount)
	{
		_projected_exp = Mathf.Clamp(_projected_exp + amount, 0, int.MaxValue);
		_leveled = _projected_exp >= MAX_EXP_PER_LEVEL;
	}

	public void SaveChanges()
	{
		if(_leveled)
		{
			int level_gain = Mathf.FloorToInt(_projected_exp / MAX_EXP_PER_LEVEL);
			_projected_lvl = Mathf.Clamp(level_gain + _projected_lvl, 1, _item_max_level);
			_projected_exp = _projected_exp % MAX_EXP_PER_LEVEL;
		}

		_current_exp = _projected_exp;
		_current_lvl = _projected_lvl;
		_leveled = false;

		// check if we hit a level milestone yet
		for(int i = 0; i < _level_timeline.Length ; ++i)
		{
			if(_current_lvl >= _level_timeline[i])
			{
				_tier_counter = i;
			}
		}
	}

	public void UndoChanges()
	{
		_projected_exp = _current_exp;
		_projected_lvl = _current_lvl;
		_leveled = false;
	}
}
