using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
// Just for now to determine between different animations
public enum ANIMATION_TYPE
{
	UNDEFINED = 0,
	SLASH
}
public class AnimationBehaviour : MonoBehaviour 
{
	private static AnimationBehaviour _instance;
	public static AnimationBehaviour instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = Camera.main.gameObject.AddComponent<AnimationBehaviour>();
				_instance.Setup();
			}

			return _instance;
		}
	}

	private struct AnimationObject
	{
		public GameObject animation;
		public float duration;
	}

	private Dictionary<ANIMATION_TYPE, GameObject> _animation_objects;
	private GameObject _current_animation_object;
	private bool _init;

	void FixedUpdate()
	{
	}

	public void PlayAnimation(ANIMATION_TYPE type, Vector3 animation_location)
	{
		_current_animation_object = _animation_objects[type];
		_current_animation_object.SetActive(true);
		_current_animation_object.transform.position = animation_location;
	}

	/// <summary>
	/// Setup the animatino objects. Meaning it pulls them from the resource folder and loads them into memory.
	/// Will need a better way to determine which animations should be loaded.
	/// Expected to be only called once.
	/// </summary>
	public void Setup()
	{
		if(_init)
			return;

		_init = true;
		_animation_objects = new Dictionary<ANIMATION_TYPE, GameObject>();
	
		ANIMATION_TYPE[] animation_list = QueryUniqueAnimations();

		if(animation_list.Length == 0)
		{
			Debug.LogWarning("No animations are set on the UnitGameObjects");
		}
		// Get the unique animations for this combat scenario
		// Change the tags to the enum, which will allow for easy fetching
		foreach(ANIMATION_TYPE type in animation_list)
		{
			_animation_objects.Add(type, QueryAnimationObject(type) as GameObject);
		}
	}

	/// <summary>
	/// Queries the unique animations. Will go through each unit existing in the combat scenario. 
	/// Get each unique animation required. Excluding duplicates.
	/// </summary>
	/// <returns>The unique animations.</returns>
	private ANIMATION_TYPE[] QueryUniqueAnimations()
	{
		// Get all units in this combat
		UnitGameobject[] _unit_objects = GameObject.FindObjectsOfType<UnitGameobject>();
		List<ANIMATION_TYPE> object_animations = new List<ANIMATION_TYPE>();

		foreach(UnitGameobject unit in _unit_objects)
		{
			// Skip undefined animation types
			if(unit.primary_item_animation == ANIMATION_TYPE.UNDEFINED)
			{ }
			else
			{
				// new animation
				if(!object_animations.Contains(unit.primary_item_animation))
				{
					object_animations.Add(unit.primary_item_animation);
				}
			}

			// Skip undefined animation types
			if(unit.secondary_item_animation == ANIMATION_TYPE.UNDEFINED)
			{}
			else
			{
				// new animation
				if(!object_animations.Contains(unit.secondary_item_animation))
				{
					object_animations.Add(unit.secondary_item_animation);
				}
			}

			// Skip undefined animation types
			if(unit.soulshard_animation == ANIMATION_TYPE.UNDEFINED)
			{}
			else
			{
				// new animation
				if(!object_animations.Contains(unit.soulshard_animation))
				{
					object_animations.Add(unit.soulshard_animation);
				}
			}
		}

		return object_animations.ToArray();
	}

	/// <summary>
	/// Queries the animation object depnding on what ANIMATION_TYPE was requested
	/// </summary>
	/// <returns>The animation object.</returns>
	/// <param name="type">Type.</param>
	private GameObject QueryAnimationObject(ANIMATION_TYPE type)
	{
		GameObject animation_obj = null;
		switch(type)
		{
		case ANIMATION_TYPE.SLASH:
			animation_obj = Instantiate(Resources.Load("SlashAnimation")) as GameObject;
			break;
		default:
			animation_obj = null;
			break;
		}

		return animation_obj;
	}

	/// <summary>
	/// Release the animation objects, freeing up memory.
	/// </summary>
	public void CleanUp()
	{
		if(!_init)
			return;

		_animation_objects.Clear();
		_init = false;
	}
}
*/