using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class CreateItemWindow : EditorWindow
{
	private static int _id;
	private static string _name = "Default Item name";
	private static int _cost;
	private static int _amount;
	private static int _cooldown;
	private static string _text = "Neque porro quisquam est qui dolorem ipsum quia dolor sit amet, consectetur";
	private static WEAPON_HANDLE _hand;
	private static HARMFULNESS _harmfulness;
	private static ACTION_LIST _action;
	private static ABILITY_LIST _ability;

	private static AudioClip _on_use_sound_effect;
//	private static string _on_use_sound_effect_resource_path;

	private static Animator _on_use_animation_effect;

	private static Texture2D _item_texture;

	private static Vector2 scroll_pos;
	private static bool fold_out;
	private static bool fold_out_open;

	private static string _item_path = "Assets/Resources/Items/";

	private static BaseItem _current_item;

	// Item progression stuff
//	private static int _size;
//
//	private static int[] _level_timeline;
//	private static bool _lt_foldout;
//
//	private static Texture2D[] _item_textures_per_level;
//	private static bool _it_foldout;
//
//	private static Texture2D[] _item_scroll_textures;
//	private static bool _is_foldout;
//
//	private static int _item_max_level;
//	private static BaseItem _next_item;

	// Add menu item named "My Window" to the Window menu
	[MenuItem("Window/Item Creator")]
	public static void ShowWindow()
	{
		_current_item = null;
		_id = BaseItemDataBaseInstance.instance.main_data.current_id;

//		_level_timeline = new int[0];
//		_item_textures_per_level = _item_scroll_textures = new Texture2D[0];
//		_size = 0;
//		_item_max_level = 1;

		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindow(typeof(CreateItemWindow));
	}

	public static void ShowWindowWithProperties(BaseItem item)
	{
		SetUpProperties(item);
		
		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindow(typeof(CreateItemWindow));
	}
	
	static void SetUpProperties(BaseItem item)
	{
		_current_item= item;
		_item_texture= _current_item.item_image;
		_id          = _current_item.id;
		_name        = _current_item.name;
		_cost        = _current_item.cost;
		_text        = _current_item.text;
		_amount      = _current_item.amount;
		_cooldown    = _current_item.cooldown;
		_hand        = _current_item.hand;
		_harmfulness = _current_item.harmfulness;
		_action      = _current_item.action_list;
		_ability     = _current_item.ability_list;
		_on_use_sound_effect = _current_item.on_use_sound_effect;
		_on_use_animation_effect = _current_item.on_use_animation_effect;

//		if(_current_item.item_progression != null)
//		{
//			_level_timeline = (_current_item.item_progression.level_timeline != null)?_current_item.item_progression.level_timeline:new int[0];
//			_size = _level_timeline.Length;
//	
//			_item_textures_per_level = (_current_item.item_progression.item_textures_per_level != null)?_current_item.item_progression.item_textures_per_level:new Texture2D[0];
//			_item_scroll_textures = (_current_item.item_progression.item_scroll_textures != null)?_current_item.item_progression.item_scroll_textures:new Texture2D[0];
//
//			_item_max_level = _current_item.item_progression.item_max_craft_level;
//			_next_item = _current_item.item_progression.next_item;
//		}
//		else
//		{
//			_level_timeline = new int[0];
//			_size = 0;
//			_item_textures_per_level = _item_scroll_textures = new Texture2D[0];
//		}


//		_on_use_sound_effect_resource_path = _current_item.on_use_sound_effect_resource_path;
	}


	void OnEnable()
	{
		_action      = ACTION_LIST.ON_ATTACK;
		_cost = 0; // For timer, stops turn based cooldown
	}

	void OnGUI()
	{
		GUILayout.Label ("Item creation", EditorStyles.boldLabel);

		scroll_pos   = GUILayout.BeginScrollView(scroll_pos,false, true);
		EditorGUILayout.IntField("Item id", _id);
		_item_texture= EditorGUILayout.ObjectField("Item image",_item_texture, typeof(Texture2D), false) as Texture2D;
		_name        = EditorGUILayout.TextField ("Item name", _name);
		
		if(Applications.type == COMBAT_TYPE.TURNED)
		{
			_cost    = EditorGUILayout.IntField("Item usage cost", _cost);
			EditorGUILayout.HelpBox("How much power this item will use up.", MessageType.Info);
		}

		EditorGUILayout.LabelField("Item description");
		_text        = EditorGUILayout.TextArea(_text, GUILayout.Width(position.width - 22f), GUILayout.Height(50));
		EditorStyles.textArea.wordWrap  = true;
		EditorStyles.textField.wordWrap = true;

		_amount      = EditorGUILayout.IntField("Item amount on usage", _amount);
		EditorGUILayout.HelpBox("This can be amount of damage, healing, etc.", MessageType.Info);

		_cooldown    = EditorGUILayout.IntField("Item cooldown", _cooldown);
		string word = (Applications.type == COMBAT_TYPE.TURNED) ? "turn" : "second";
		EditorGUILayout.HelpBox(string.Format("This is a per {0} basis.", word), MessageType.Info);

		_hand        = WEAPON_HANDLE.SINGLE_HANDED;
		_harmfulness = (HARMFULNESS)EditorGUILayout.EnumPopup("Item harmfulness", _harmfulness);
		EditorGUILayout.HelpBox("Harmfulness, meaning if the weapon is capable of harming, helping, or providing utility to another unit.", MessageType.Info);

		_action      = (ACTION_LIST)EditorGUILayout.EnumPopup("Item action", _action);
		EditorGUILayout.HelpBox("Action, meaning how the item usage will play out with its effect.", MessageType.Info);

		_ability     = (ABILITY_LIST)EditorGUILayout.EnumPopup("Item ability", _ability);
		EditorGUILayout.HelpBox("Ability, Item effect as to what it is capable of doing after or on usage.", MessageType.Info);

//		PresentItemProgression();

		fold_out = EditorGUILayout.Foldout(fold_out, "Optional item effects");
		if(fold_out)
		{
			_on_use_sound_effect     = EditorGUILayout.ObjectField("Item on usage sound effect", _on_use_sound_effect, typeof(AudioClip), false) as AudioClip;
			_on_use_animation_effect = EditorGUILayout.ObjectField("Item on usage animation effect", _on_use_animation_effect, typeof(Animator), false) as Animator;

			if(!fold_out_open)
			{
				scroll_pos += Vector2.up*50;
				fold_out_open = true;
			}
		}
		else
		{
			fold_out_open = false;
		}
			
		GUILayout.EndScrollView();

		EditorGUILayout.Space();

		GUILayout.BeginHorizontal();

		int num_of_buttons = 3;
		if(GUILayout.Button("Create new", GUILayout.Width(position.width/num_of_buttons)))
		{
			if(EditorUtility.DisplayDialog("Create item", "Creating new item, are you sure?",
			                               "Yes", "No"))
			{
				BaseItem item = ScriptableObjectUtility.CreateAssetAt<BaseItem>(_item_path + string.Format("{0}.asset", _name), false);
				item.Init(_name, _cost, _amount, _cooldown, _text, _hand, _harmfulness, _action, _ability);
				item.SetImage(_item_texture);
				item.on_use_sound_effect_resource_path = AssetDatabase.GetAssetPath(_on_use_sound_effect).Replace("Assets/Resources/", "");
				int fileExtPos = item.on_use_sound_effect_resource_path.LastIndexOf(".");
				if (fileExtPos >= 0 )
					item.on_use_sound_effect_resource_path= item.on_use_sound_effect_resource_path.Substring(0, fileExtPos);

				// Add in item progression stuff
//				item.SetItemProgresion(_level_timeline, _item_textures_per_level, _item_scroll_textures, _item_max_level, _next_item);
//				item.SetItemProgresion(new ItemProgression(_level_timeline, _item_textures_per_level, _item_scroll_textures, _item_max_level, _next_item));

				//item.SetSoundEffectAudio(_on_use_sound_effect);
				//BaseItemDataBaseInstance.instance.AddToDatabase(item);
				SaveItem(item);
				this.Close();
			}
		}

		if(_name.Length > 0)
		{
			if(GUILayout.Button("Update", GUILayout.Width(position.width/num_of_buttons)))
			{
				BaseItem item = AssetDatabase.LoadAssetAtPath(_item_path + string.Format("{0}.asset", _name), typeof(BaseItem)) as BaseItem;
				item.Init(_name, _cost, _amount, _cooldown, _text, _hand, _harmfulness, _action, _ability);
				item.SetImage(_item_texture);
				item.on_use_sound_effect_resource_path = AssetDatabase.GetAssetPath(_on_use_sound_effect).Replace("Assets/Resources/", "");
				int fileExtPos = item.on_use_sound_effect_resource_path.LastIndexOf(".");
				if (fileExtPos >= 0 )
					item.on_use_sound_effect_resource_path= item.on_use_sound_effect_resource_path.Substring(0, fileExtPos);

				// Add in item progression stuff
//				item.SetItemProgresion(_level_timeline, _item_textures_per_level, _item_scroll_textures, _item_max_level, _next_item);
//				item.SetItemProgresion(new ItemProgression(_level_timeline, _item_textures_per_level, _item_scroll_textures, _item_max_level, _next_item));

				//item.SetSoundEffectAudio(_on_use_sound_effect);
				//BaseItemDataBaseInstance.instance.ReplaceFromDatabaseAt(pos, item);
				SaveItem(item);
				this.Close();
			}
		}

		if(GUILayout.Button("Reset", GUILayout.Width(position.width/num_of_buttons)))
		{
			ResetValues();
		}

		GUILayout.EndHorizontal();

		this.Repaint();
	}

	private void PresentItemProgression()
	{
//		EditorGUILayout.Space();
//		EditorGUILayout.PrefixLabel("Item Progression");
//		EditorGUILayout.HelpBox("Provide an array of levels in which the item will change appearance", MessageType.Info);
//
//		_size = EditorGUILayout.IntField("Size", _size);
//		if(_level_timeline.Length != _size)
//		{
//			System.Array.Resize(ref _level_timeline, _size);
//			System.Array.Resize(ref _item_textures_per_level, _size);
//			System.Array.Resize(ref _item_scroll_textures, _size);
//		}
//		_lt_foldout = EditorGUILayout.Foldout(_lt_foldout," Level timeline values");
//		if(_lt_foldout)
//		{
//			for(int i=0;i<_size;++i)
//			{
//				_level_timeline[i] = EditorGUILayout.IntField(string.Format("Element{0}", i), _level_timeline[i]);
//			}
//		}
//
//		EditorGUILayout.Space();
//		EditorGUILayout.HelpBox("Provide corresponding textures", MessageType.Info);
//		_it_foldout = EditorGUILayout.Foldout(_it_foldout," Level timeline textures");
//		if(_it_foldout)
//		{
//			for(int i=0;i<_size;++i)
//			{
//				_item_textures_per_level[i] = EditorGUILayout.ObjectField(string.Format("Element{0}", i), _item_textures_per_level[i], typeof(Texture2D), false) as Texture2D;
//			}
//		}
//
//		// _item_scroll_textures
//		EditorGUILayout.Space();
//		EditorGUILayout.HelpBox("Provide corresponding textures", MessageType.Info);
//		_is_foldout = EditorGUILayout.Foldout(_is_foldout," Item scroll textures");
//		if(_is_foldout)
//		{
//			for(int i=0;i<_size;++i)
//			{
//				_item_scroll_textures[i] = EditorGUILayout.ObjectField(string.Format("Element{0}", i), _item_scroll_textures[i], typeof(Texture2D), false) as Texture2D;
//			}
//		}
//
//
//		EditorGUILayout.Space();
//		_item_max_level = EditorGUILayout.IntSlider("Item max level", _item_max_level, 1, 100);
//		_next_item = EditorGUILayout.ObjectField("Next item", _next_item, typeof(BaseItem), false) as BaseItem;
//
//		EditorGUILayout.Space();
	}

	private void SaveItem(BaseItem item)
	{
		ProjectApocalypseCreate.SaveAsset(item);
//		AssetDatabase.Refresh ();
//		EditorUtility.SetDirty(item);
//		AssetDatabase.SaveAssets();
	}
	
	private void ResetValues()
	{
		_id          = BaseItemDataBaseInstance.instance.main_data.current_id;

		_item_texture= null;
		_name        = null;
		_cost        = 0;
		_text        = null;
		_amount      = 0;
		_cooldown    = 0;
		_hand        = WEAPON_HANDLE.UNDEFINED;
		_harmfulness = HARMFULNESS.UNDEFINED;
		_action      = ACTION_LIST.ON_ATTACK;
		_ability     = ABILITY_LIST.PASSIVE;
	}

	void OnDestroy()
	{
		_current_item = null;
	}
}
