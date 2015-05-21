using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

//[ExecuteInEditMode]
[RequireComponent (typeof(HandleEvents), typeof(HandleInputs), typeof(GUICooldownTimer))]
public class Game_Timer : MonoBehaviour 
{
	public GameObject game_start_button;

	private HeroEntityPD hero;
	private Currency currency;

	public Text result_text;
	public Text deep_iron;
	public Text dream_shard;
	public Text ethereal_dust;
	public Text exp_text;

	public Image levelup_banner;
	public Image outcome_combat_image_object;
	public Sprite victory_banner;
	public Sprite defeat_banner;

	public Image spirit_bar_amount;
	public Text spirit_bar_amount_text;

	public GameObject player_obj;
	public GameObject ally_obj_left;
	public GameObject ally_obj_right;

	// Only care for the positions
	// Assignment on screen will be as such (left, middle, right) [0, 1, 2]
	public GameObject[] all_enemies_obj;

	public static UnitEntity player;
	public static UnitEntity ally_unit_left;
	public static UnitEntity ally_unit_right;

	public static UnitEntity[] all_enemies;

	private UnitGameobject _player_unit_obj;
	private UnitGameobject _unit_obj_left;
	private UnitGameobject _unit_obj_right;


	private UnitGameobject[] _all_enemies_unit_obj;

	[HideInInspector]
	public bool ai_on;

	public static bool m_init = false;

	//HACK: Attempting to get animation to work.
	private GameObject  bossGameObject;
	private UnitAnimation bossAnimation;

	private Coroutine[] _enemy_ai_coroutine;

	// NOTE: If you initialize any extra characters make sure to update there health rects after
	// TurnBasedGUILayout.InitLayout() is called.
	void Awake()
	{
		game_start_button.SetActive(true);
		hero     = PlayerHeroSingleton.instance.main_hero;
		currency = CurrencySingleton.instance.currency;

		GetComponent<HandleEvents>().enabled = false;

		//GetComponent<Applications>().type = COMBAT_TYPE.TIMED;

		SetUpHero();
		SetupAlly();
		SetUpEnemies(EncounterManagement.current_wave.enemies);
		if(EncounterManagement.current_wave.ContainBoss())
		{
			SetUpBoss(EncounterManagement.current_wave.boss);
		}

		TimerBasedCombatSystem.instance.AddUnitTo(ally_unit_left, OWNERSHIP.PLAYER);
		TimerBasedCombatSystem.instance.AddUnitTo(player, OWNERSHIP.PLAYER);
		TimerBasedCombatSystem.instance.AddUnitTo(ally_unit_right, OWNERSHIP.PLAYER);

		TimerBasedCombatSystem.instance.SetMaxPower(50);
	}

	void AICoroutine()
	{
		if(_enemy_ai_coroutine == null)
		{
			int max_enemies = Mathf.Min(all_enemies.Length, 3);
			float prev_time = 0f;
			float delay_time = .75f*2f;
			float arrow_ui_delay = delay_time;
			float time = Time.time;

			_enemy_ai_coroutine =  new Coroutine[max_enemies];

			for(int i =0 ; i < max_enemies; ++i)
			{
				prev_time += Random.Range(0.75f, 1.25f);
				
				UnitEntity unit = all_enemies[i];

				ArrowUI(unit, prev_time + delay_time);
				
				_enemy_ai_coroutine[i] = DelayAction.instance.DelayInf(()=>
				{
					if(!ai_on) return;

					System.Func<COMBAT_RETURN_STATUS> ai_action = AI_Timer.Decision(unit);
					TimerBasedCombatSystem.instance.current_combat_status = ai_action();
					Debug.Log(string.Format("Combat return status: {0}", TimerBasedCombatSystem.instance.current_combat_status));
					TimerBasedCombatSystem.instance.UpdateCombatState();
				}, 
				prev_time + delay_time + 1.5f,
				()=>
				{ 
					return (unit.IsDead || TimerBasedCombatSystem.instance.winner != OWNERSHIP.UNDEFINED); 
				}); // Not too great, checks every 1s if possible);
			}
		}
	}

	void ArrowUI(UnitEntity unit, float delay = 0f)
	{
		EnemyArrowAnimationUI arrow_ui = GameObject.FindObjectOfType<EnemyArrowAnimationUI>();
		DelayAction.instance.DelayInf(()=>
		{
			if(!ai_on) return;
			if(arrow_ui != null)
			{
				if(unit.available_action)
				{
					arrow_ui.Show();
					arrow_ui.SetPosition(unit.unit_game_object.gameObject.transform.position);
				}
			}
		}, delay,
		()=>
		{ 
			return (unit.IsDead || TimerBasedCombatSystem.instance.winner != OWNERSHIP.UNDEFINED); 
		});
	}

	// Use this for initialization
	void Start () 
	{
		TimerBasedCombatSystem.instance.PortraitSpinRight();

		GetComponent<HandleEvents>().enabled = true;
		
		StartCoroutine(UpdateVisuals(0.1f));
		
		m_init = true;

		ai_on = true;
		GameObject.FindObjectOfType<TimerCombatDebugUI>().UpdateEnemyAI();

		// Do AI stuff
		if(ai_on)
		{
			AICoroutine();
		}

		// Check for winner, if winner do stuff
		DelayAction.instance.DelayInf(()=>
		{
			OWNERSHIP winner = TimerBasedCombatSystem.instance.winner;
			if(winner == OWNERSHIP.UNDEFINED) return; // Only do stuff when there is a winner

			// Manage aftermath
			GetComponent<HandleInputs>().enabled = false; // kill input for combat buttons
			bool player_victory = (winner == OWNERSHIP.PLAYER);
			float exp = (player_victory) ? 1.5f * all_enemies.Length : 0f;
			int di    = (player_victory) ? Random.Range(1,100) : Random.Range(1,25);
			int ds    = (player_victory) ? Random.Range(1,100) : Random.Range(1,25);
			int ed    = (player_victory) ? Random.Range(1,100) : Random.Range(1,25);

			// Store reward for the encounter. Will be added up at the end.
			EncounterManagement.current_wave.SetReward(new Reward(exp, di, ds, ed));
			//EncounterManagement.NextWave();

			outcome_combat_image_object.gameObject.SetActive(true); // exit combat
			outcome_combat_image_object.sprite = (player_victory) ? victory_banner : defeat_banner;

			deep_iron.text     = di.ToString();
			dream_shard.text   = ds.ToString();
			ethereal_dust.text = ed.ToString();
			exp_text.text      = exp.ToString("N0");

			// Now safe to set next encounter
			EncounterManagement.NextWave(); // set to next wave, if any
			bool encounter_completed = EncounterManagement.at_end;

			// Check if we got next encounter
			// If we got another wave, put text, otherwise we are at the end, display loot or run
			result_text.text   = (encounter_completed) ? ((player_victory) ? "Loot" : "Retreat") : "Next wave";

			// We are done. Tally reward and exit scene
			if(encounter_completed)
			{
				Reward reward = EncounterManagement.total_reward;

				if(player_victory)
					EncounterManagement.SetCurrentEncounterAsCompleted();

				// Add up player's currency
				currency.AddTo(CURRENCY_TYPE.DEEP_IRON, (uint)reward.deep_iron);
				currency.AddTo(CURRENCY_TYPE.DREAM_SHARD, (uint)reward.dream_shard);
				currency.AddTo(CURRENCY_TYPE.ETHEREAL_DUST, (uint)reward.ethereal_dust);
				
				CurrencySingleton.Save();

				// Add up hero's exp
				bool level_up = hero.AddExperience(reward.experience);

				// If we leveled up ignore stats, they are getting over written now
				if(level_up)
				{
					player.base_entity.hp = hero.base_entity_pd.health;
				}
				// Update 
				else
				{
					if(player.base_entity.hp == 0)
						player.base_entity.hp = 1;
					hero.base_entity_pd.health = player.base_entity.hp;
				}

				PlayerHeroSingleton.Copy(hero); // worried about this
				PlayerHeroSingleton.Save();

				if(levelup_banner)
				{
					levelup_banner.enabled = level_up;
				}
			}
		},
		.5f,
		()=> { return TimerBasedCombatSystem.instance.winner != OWNERSHIP.UNDEFINED; });

		DelayAction.instance.DelayInf(()=> { 
									spirit_bar_amount.fillAmount = TimerBasedCombatSystem.instance.group_power_normalized; 
									spirit_bar_amount_text.text = string.Format("{0} / {1}", TimerBasedCombatSystem.instance.group_power, TimerBasedCombatSystem.instance.max_group_power); 
//									spirit_bar_amount.color = new Color(Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f)); 
									  },
									  1f/10f,
									  ()=> { return TimerBasedCombatSystem.instance.winner != OWNERSHIP.UNDEFINED; });	

		DelayAction.instance.DelayInf(()=>{ TimerBasedCombatSystem.instance.ModPower(1); }, 1f, ()=>{ return TimerBasedCombatSystem.instance.winner != OWNERSHIP.UNDEFINED; });
		GameObject.FindObjectOfType<UpdateItemPortrait>().ChangeItemPortrait();
	}

	public void UpdateSwipePortrait()
	{
		if(TimerBasedCombatSystem.instance.selected_unit.IsDead)
		{
//			Debug.Log(string.Format("Unit: {0}   DEAD: {1}", TimerBasedCombatSystem.instance.right_unit, TimerBasedCombatSystem.instance.right_unit.IsDead));
//			Debug.Log(string.Format("Unit: {0}   DEAD: {1}", TimerBasedCombatSystem.instance.left_unit, TimerBasedCombatSystem.instance.left_unit.IsDead));

			if(!TimerBasedCombatSystem.instance.right_unit.IsDead)
			{
				RightSwipePortrait();
			}
			else if(!TimerBasedCombatSystem.instance.left_unit.IsDead)
			{
				LeftSwipePortrait();
			}
		}
	}

	public void RightSwipePortrait()
	{
		if(TimerBasedCombatSystem.instance.right_unit.IsDead) return;
		Debug.Log("RIGHT");
		TimerBasedCombatSystem.instance.SwapRight();
		GameObject.FindObjectOfType<UpdateItemPortrait>().ChangeItemPortrait();

		Vector3 tempScale = TimerBasedCombatSystem.instance.right_unit.unit_game_object.gameObject.transform.localScale;
		TimerBasedCombatSystem.instance.right_unit.unit_game_object.gameObject.transform.localScale = TimerBasedCombatSystem.instance.selected_unit.unit_game_object.gameObject.transform.localScale;
		TimerBasedCombatSystem.instance.selected_unit.unit_game_object.gameObject.transform.localScale = tempScale;

		Vector3 tempPos = TimerBasedCombatSystem.instance.right_unit.unit_game_object.gameObject.transform.position;
		TimerBasedCombatSystem.instance.right_unit.unit_game_object.gameObject.transform.position = TimerBasedCombatSystem.instance.selected_unit.unit_game_object.gameObject.transform.position;
		TimerBasedCombatSystem.instance.selected_unit.unit_game_object.gameObject.transform.position = tempPos;

		_unit_obj_left.UpdateUnitGameObject();
		_unit_obj_right.UpdateUnitGameObject();
		_player_unit_obj.UpdateUnitGameObject();

		return;
		TimerBasedCombatSystem.instance.PortraitSpinRight();

		GameObject.FindObjectOfType<UpdateItemPortrait>().ChangeItemPortrait();

		_unit_obj_left.UpdateUnitGameObject();
		_unit_obj_right.UpdateUnitGameObject();
		_player_unit_obj.UpdateUnitGameObject();

		Debug.Log(string.Format("Left:  <{0} **DEAD: {1}**  Center:  ({2}) **DEAD: {3}**  Right:  {4} **DEAD: {5}**>", TimerBasedCombatSystem.instance.left_unit, TimerBasedCombatSystem.instance.left_unit.IsDead, TimerBasedCombatSystem.instance.selected_unit, TimerBasedCombatSystem.instance.selected_unit.IsDead, TimerBasedCombatSystem.instance.right_unit, TimerBasedCombatSystem.instance.right_unit.IsDead));
		

		// scale
		tempScale = _unit_obj_left.gameObject.transform.localScale;
		_unit_obj_left.gameObject.transform.localScale = _unit_obj_right.gameObject.transform.localScale;
		_unit_obj_right.gameObject.transform.localScale = tempScale;
		
		tempScale = _player_unit_obj.gameObject.transform.localScale;
		_player_unit_obj.gameObject.transform.localScale = _unit_obj_right.gameObject.transform.localScale;
		_unit_obj_right.gameObject.transform.localScale = tempScale;

//		// Swap positions
		tempPos = _unit_obj_left.gameObject.transform.position;
		if(!TimerBasedCombatSystem.instance.left_unit.IsDead && !TimerBasedCombatSystem.instance.right_unit.IsDead)
		{
			_unit_obj_left.gameObject.transform.position = _unit_obj_right.gameObject.transform.position;
			_unit_obj_right.gameObject.transform.position = tempPos;
		}
		
		tempPos = _player_unit_obj.gameObject.transform.position;
		if(!TimerBasedCombatSystem.instance.selected_unit.IsDead && !TimerBasedCombatSystem.instance.right_unit.IsDead)
		{
			_player_unit_obj.gameObject.transform.position = _unit_obj_right.gameObject.transform.position;
			_unit_obj_right.gameObject.transform.position = tempPos;
		}
	}

	public void LeftSwipePortrait()
	{
		if(TimerBasedCombatSystem.instance.left_unit.IsDead) return;
		Debug.Log("LEFT");
		
		TimerBasedCombatSystem.instance.SwapLeft();
		GameObject.FindObjectOfType<UpdateItemPortrait>().ChangeItemPortrait();

		Vector3 tempScale = TimerBasedCombatSystem.instance.left_unit.unit_game_object.gameObject.transform.localScale;
		TimerBasedCombatSystem.instance.left_unit.unit_game_object.gameObject.transform.localScale = TimerBasedCombatSystem.instance.selected_unit.unit_game_object.gameObject.transform.localScale;
		TimerBasedCombatSystem.instance.selected_unit.unit_game_object.gameObject.transform.localScale = tempScale;

		Vector3 tempPos = TimerBasedCombatSystem.instance.left_unit.unit_game_object.gameObject.transform.position;
		TimerBasedCombatSystem.instance.left_unit.unit_game_object.gameObject.transform.position = TimerBasedCombatSystem.instance.selected_unit.unit_game_object.gameObject.transform.position;
		TimerBasedCombatSystem.instance.selected_unit.unit_game_object.gameObject.transform.position = tempPos;

		_unit_obj_left.UpdateUnitGameObject();
		_unit_obj_right.UpdateUnitGameObject();
		_player_unit_obj.UpdateUnitGameObject();

		return;
		TimerBasedCombatSystem.instance.PortraitSpinLeft();

		GameObject.FindObjectOfType<UpdateItemPortrait>().ChangeItemPortrait();

		_unit_obj_left.UpdateUnitGameObject();
		_unit_obj_right.UpdateUnitGameObject();
		_player_unit_obj.UpdateUnitGameObject();

		Debug.Log(string.Format("Left:  <{0} **DEAD: {1}**  Center:  ({2}) **DEAD: {3}**  Right:  {4} **DEAD: {5}**>", TimerBasedCombatSystem.instance.left_unit, TimerBasedCombatSystem.instance.left_unit.IsDead, TimerBasedCombatSystem.instance.selected_unit, TimerBasedCombatSystem.instance.selected_unit.IsDead, TimerBasedCombatSystem.instance.right_unit, TimerBasedCombatSystem.instance.right_unit.IsDead));

//
//		// scale
		tempScale = _unit_obj_left.gameObject.transform.localScale;
		_unit_obj_left.gameObject.transform.localScale = _unit_obj_right.gameObject.transform.localScale;
		_unit_obj_right.gameObject.transform.localScale = tempScale;
		
		tempScale = _player_unit_obj.gameObject.transform.localScale;
		_player_unit_obj.gameObject.transform.localScale = _unit_obj_left.gameObject.transform.localScale;
		_unit_obj_left.gameObject.transform.localScale = tempScale;

//		// Swap positions
		tempPos = _unit_obj_left.gameObject.transform.position;
		if(!TimerBasedCombatSystem.instance.left_unit.IsDead && !TimerBasedCombatSystem.instance.right_unit.IsDead)
		{
			_unit_obj_left.gameObject.transform.position = _unit_obj_right.gameObject.transform.position;
			_unit_obj_right.gameObject.transform.position = tempPos;
		}

		tempPos = _player_unit_obj.gameObject.transform.position;
		if(!TimerBasedCombatSystem.instance.left_unit.IsDead && !TimerBasedCombatSystem.instance.selected_unit.IsDead)
		{
			_player_unit_obj.gameObject.transform.position = _unit_obj_left.gameObject.transform.position;
			_unit_obj_left.gameObject.transform.position = tempPos;
		}
	}

	void Update()
	{
//		if(Input.GetKeyDown(KeyCode.Alpha1))
//		{
//			System.Func<COMBAT_RETURN_STATUS> ai_action = AI_Timer.Decision(all_enemies[0]);
//			TimerBasedCombatSystem.instance.current_combat_status = ai_action();
//			Debug.Log(string.Format("Combat return status: {0}", TimerBasedCombatSystem.instance.current_combat_status));
//			TimerBasedCombatSystem.instance.UpdateCombatState();
//		}
//
//		if(Input.GetKeyDown(KeyCode.Alpha2))
//		{
//			System.Func<COMBAT_RETURN_STATUS> ai_action = AI_Timer.Decision(all_enemies[1]);
//			TimerBasedCombatSystem.instance.current_combat_status = ai_action();
//			Debug.Log(string.Format("Combat return status: {0}", TimerBasedCombatSystem.instance.current_combat_status));
//			TimerBasedCombatSystem.instance.UpdateCombatState();
//		}
//
//		if(Input.GetKeyDown(KeyCode.Alpha3))
//		{
//			System.Func<COMBAT_RETURN_STATUS> ai_action = AI_Timer.Decision(all_enemies[2]);
//			TimerBasedCombatSystem.instance.current_combat_status = ai_action();
//			Debug.Log(string.Format("Combat return status: {0}", TimerBasedCombatSystem.instance.current_combat_status));
//			TimerBasedCombatSystem.instance.UpdateCombatState();
//		}
//		Debug.LogError(string.Format("Current unit: {0}", TimerBasedCombatSystem.instance.selected_unit.base_entity.name));
	}

	IEnumerator UpdateVisuals(float delay)
	{
		while(true)
		{
			if(ai_on && _enemy_ai_coroutine == null)
				AICoroutine();

			foreach(UnitEntity ue in TimerBasedCombatSystem.instance.AllUnitsFrom(OWNERSHIP.PLAYER))
			{
				ue.unit_game_object.UpdateGUI();
			}

			foreach(UnitEntity ue in TimerBasedCombatSystem.instance.AllUnitsFrom(OWNERSHIP.ENEMY))
			{
				ue.unit_game_object.UpdateGUI();
				ue.unit_game_object.UpdateHP();
			}

			yield return new WaitForSeconds(delay);
		}
	}

	void SetUpHero()
	{
		// Player
//		BaseEntity    be = new BaseEntity(hero.base_entity_pd.max_health, 0, 0, hero.base_entity_pd.name);
		BaseEntity    be = new BaseEntity(hero.base_entity_pd);
//		be.DamageUnit(hero.base_entity_pd.max_health - hero.base_entity_pd.health);

		PrimaryItem   pi = new PrimaryItem(Items.CreatePrimaryWeapon0_Timer());
		pi.item.on_use_sound_effect_resource_path = "Sounds/rip+struck";

		SecondaryItem si = new SecondaryItem(Items.CreateSecondaryWeapon0_Timer());
		si.item.on_use_sound_effect_resource_path = "Sounds/armor_ching";

		SoulShard     ss = new SoulShard(Items.CreateSoulShard0_Timer());
		ss.item.on_use_sound_effect_resource_path = "Sounds/healing_chime";

		player           = new UnitEntity(be, pi, si, ss);

		// primary
		player.primary_slot = new PrimaryItem[2];
		Debug.Log(player);	

		player.primary_slot[0] = new PrimaryItem(Items.CreateRangerPrimaryWeapon0Ability0_Timer());
//		player.primary_slot[0].item.SetImage(Resources.Load("Textures/Abilities/ability_prim_attack1_FPO") as Texture2D);

//		player.primary_slot[0].item.on_use_animation_effect
		
		player.primary_slot[1] = new PrimaryItem(Items.CreateRangerPrimaryWeapon0Ability1_Timer());
//		player.primary_slot[1].item.item_fx = Resources.Load("CustomFX/FX_EnergySwordCompiled") as GameObject;
//		player.primary_slot[1].item.SetImage(Resources.Load("Textures/Abilities/ability_prim_attack2_FPO") as Texture2D);

		// secondary
		player.secondary_slot = new SecondaryItem[2];
		player.secondary_slot[0] = new SecondaryItem(Items.CreateRangerSecondaryWeapon0Ability0_Timer());
//		player.secondary_slot[0].item.SetImage(Resources.Load("Textures/Abilities/ability_sec_shield_FPO") as Texture2D);

		player.secondary_slot[1] = new SecondaryItem(Items.CreateRangerSecondaryWeapon0Ability1_Timer());
//		player.secondary_slot[1].item.item_fx = Resources.Load("CustomFX/FX_Shatter") as GameObject;
//		player.secondary_slot[1].item.SetImage(Resources.Load("Textures/Abilities/ability_shield_mysticshield_FPO") as Texture2D);

		player.soulshard_slot = new SoulShard[1];
		player.soulshard_slot[0] = null;
//		player.soulshard_slot[0].item.item_fx = Resources.Load("CustomFX/FX_HealWard") as GameObject;
//		player.soulshard_slot[0].item.SetImage(Resources.Load("Textures/Abilities/ability_class_heal_FPO") as Texture2D);
//		
		// Find Gameobject for the unit
		_player_unit_obj = player_obj.GetComponent<UnitGameobject>();
		// Set the unit's UnitEntity component to this generated one
		_player_unit_obj._unit_entity = player;
		_player_unit_obj.Setup();
		
		// Set weak ref of the actual GameObject to the UnitEntity
		player.SetWeakRefUnitGameObject(_player_unit_obj);

//		TimerBasedCombatSystem.instance.UnitsFrom(OWNERSHIP.PLAYER).Add(player);
	}

	void SetupAlly()
	{
		// Left ally
		if(ally_obj_left != null)
		{
	//		BaseEntity    be = new BaseEntity(hero.base_entity_pd.max_health, 0, 0, hero.base_entity_pd.name);
			BaseEntity    be = new BaseEntity(15,5,2,"Mystic"); // new BaseEntity(hero.base_entity_pd);
	//		be.DamageUnit(hero.base_entity_pd.max_health - hero.base_entity_pd.health);
			
			PrimaryItem   pi = new PrimaryItem(Items.CreatePrimaryWeapon0_Timer());
			pi.item.on_use_sound_effect_resource_path = "Sounds/rip+struck";
			
			SecondaryItem si = new SecondaryItem(Items.CreateSecondaryWeapon0_Timer());
			si.item.on_use_sound_effect_resource_path = "Sounds/armor_ching";
			
			SoulShard     ss = new SoulShard(Items.CreateSoulShard0_Timer());
			ss.item.on_use_sound_effect_resource_path = "Sounds/healing_chime";
			
			ally_unit_left           = new UnitEntity(be, pi, si, ss);

			ally_unit_left.primary_slot = new PrimaryItem[2];
			ally_unit_left.primary_slot[0] = new PrimaryItem(Items.CreateMysticPrimaryWeapon0Ability0_Timer());
			ally_unit_left.primary_slot[1] = new PrimaryItem(Items.CreateMysticPrimaryWeapon0Ability1_Timer());
			
			ally_unit_left.secondary_slot = new SecondaryItem[2];
			ally_unit_left.secondary_slot[0] = new SecondaryItem(Items.CreateMysticSecondaryWeapon0Ability0_Timer());
			ally_unit_left.secondary_slot[1] = new SecondaryItem(Items.CreateMysticSecondaryWeapon0Ability1_Timer());
			
			ally_unit_left.soulshard_slot = new SoulShard[1];
			
			// Find Gameobject for the unit
			_unit_obj_left = ally_obj_left.GetComponent<UnitGameobject>();
			// Set the unit's UnitEntity component to this generated one
			_unit_obj_left._unit_entity = ally_unit_left;
			_unit_obj_left.Setup();

			// Set weak ref of the actual GameObject to the UnitEntity
			ally_unit_left.SetWeakRefUnitGameObject(_unit_obj_left);
			
//			TimerBasedCombatSystem.instance.UnitsFrom(OWNERSHIP.PLAYER).Add(ally_unit_left);
		}
		else
		{
			Debug.LogError("Left ally missing");
		}

		// Right ally
		if(ally_obj_right != null)
		{
//			BaseEntity    be = new BaseEntity(hero.base_entity_pd.max_health, 0, 0, hero.base_entity_pd.name);
			BaseEntity    be = new BaseEntity(15,5,2,"Solider");
	//		be.DamageUnit(hero.base_entity_pd.max_health - hero.base_entity_pd.health);
			
			PrimaryItem   pi = new PrimaryItem(Items.CreatePrimaryWeapon0_Timer());
			pi.item.on_use_sound_effect_resource_path = "Sounds/rip+struck";
			
			SecondaryItem si = new SecondaryItem(Items.CreateSecondaryWeapon0_Timer());
			si.item.on_use_sound_effect_resource_path = "Sounds/armor_ching";
			
			SoulShard     ss = new SoulShard(Items.CreateSoulShard0_Timer());
			ss.item.on_use_sound_effect_resource_path = "Sounds/healing_chime";
			
			ally_unit_right           = new UnitEntity(be, pi, si, ss);
			
			ally_unit_right.primary_slot = new PrimaryItem[2];
			ally_unit_right.primary_slot[0] = new PrimaryItem(Items.CreateWarriorPrimaryWeapon0Ability0_Timer());
			ally_unit_right.primary_slot[1] = new PrimaryItem(Items.CreateWarriorPrimaryWeapon0Ability1_Timer());
			
			ally_unit_right.secondary_slot = new SecondaryItem[2];
			ally_unit_right.secondary_slot[0] = new SecondaryItem(Items.CreateWarriorSecondaryWeapon0Ability0_Timer());
			ally_unit_right.secondary_slot[1] = new SecondaryItem(Items.CreateWarriorSecondaryWeapon0Ability1_Timer());
			
			ally_unit_right.soulshard_slot = new SoulShard[1];
			ally_unit_right.soulshard_slot[0] = new SoulShard(Items.CreateWarriorSoulShardWeapon0Ability0_Timer());
			
			// Find Gameobject for the unit
			_unit_obj_right = ally_obj_right.GetComponent<UnitGameobject>();
			// Set the unit's UnitEntity component to this generated one
			_unit_obj_right._unit_entity = ally_unit_right;
			_unit_obj_right.Setup();
			
			// Set weak ref of the actual GameObject to the UnitEntity
			ally_unit_right.SetWeakRefUnitGameObject(_unit_obj_right);
			
//			TimerBasedCombatSystem.instance.UnitsFrom(OWNERSHIP.PLAYER).Add(ally_unit_right);
		}
		else
		{
			Debug.LogError("Left ally missing");
		}
	}

	void SetUpBoss(EnemyEntity boss_ee)
	{
		// fill up last element in the available slots, for the boss.
		int l = _all_enemies_unit_obj.Length;

		all_enemies_obj[2].SetActive(true);
		all_enemies[l - 1] = PrepareUnitEntitySO(boss_ee.unit_entity_so);
		all_enemies[l - 1].primary_slot = new PrimaryItem[1];
		all_enemies[l - 1].primary_slot[0] = new PrimaryItem(all_enemies[l - 1].primary.item);
		all_enemies[l - 1].primary_slot[0].item.ability_type = ABILITY_TYPE.MYSTIC;

		all_enemies[l - 1].Reset();

		_all_enemies_unit_obj[l - 1] = all_enemies_obj[2].GetComponent<UnitGameobject>();
		_all_enemies_unit_obj[l - 1]._unit_entity = all_enemies[l - 1];
		_all_enemies_unit_obj[l - 1].Setup();

		all_enemies[l - 1].SetWeakRefUnitGameObject(_all_enemies_unit_obj[l - 1]);

		all_enemies_obj[2].GetComponent<Renderer>().material.mainTexture = boss_ee.unit_entity_so.unit_image;

		TimerBasedCombatSystem.instance.AddUnitTo(all_enemies[l - 1], OWNERSHIP.ENEMY);
	}

	// Max 3 minions, which takes up a boss location
	void SetUpEnemies(EnemyEntity[] enemies_ee)
	{
		EnemyEntity[] minions_only = enemies_ee.Where(ue => ue.type == ENEMY_TYPE.MINION).ToArray();
		
		all_enemies 	      = new UnitEntity[enemies_ee.Length];
		_all_enemies_unit_obj = new UnitGameobject[enemies_ee.Length];

		for(int i =0 ; i < minions_only.Length; ++i)
		{
			all_enemies_obj[i].SetActive(true);
			all_enemies[i] = PrepareUnitEntitySO(minions_only[i].unit_entity_so);

			all_enemies[i].primary_slot = new PrimaryItem[1];
			// CreateBasicMinionPrimaryWeapon0Ability0_Timer

			all_enemies[i].primary_slot[0] = new PrimaryItem(Items.CreateBasicMinionPrimaryWeapon0Ability0_Timer());

//			all_enemies[i].primary_slot[0] = new PrimaryItem(all_enemies[i].primary.item);
//			all_enemies[i].primary_slot[0].item.ability_type = ABILITY_TYPE.MYSTIC;
//			all_enemies[i].primary_slot[0].item.item_fx = Resources.Load("CustomFX/CFXM_Slash+Text") as GameObject;
//			all_enemies[i].primary_slot[0].item.on_use_sound_effect_resource_path = "Sounds/rip+struck";
			all_enemies[i].Reset();

			_all_enemies_unit_obj[i] = all_enemies_obj[i].GetComponent<UnitGameobject>();
			_all_enemies_unit_obj[i]._unit_entity = all_enemies[i];
			_all_enemies_unit_obj[i].Setup();

			all_enemies[i].SetWeakRefUnitGameObject(_all_enemies_unit_obj[i]);

			_all_enemies_unit_obj[i].gameObject.GetComponent<Renderer>().material.mainTexture = minions_only[i].unit_entity_so.unit_image;

			TimerBasedCombatSystem.instance.AddUnitTo(all_enemies[i], OWNERSHIP.ENEMY);
		}
	}

	private UnitEntity PrepareUnitEntitySO(UnitEntitySO unit)
	{
		BaseEntity be    = new BaseEntity(unit.base_entity_so);
		PrimaryItem pi   = (unit.primary.item != null) ? new PrimaryItem(new BaseItem(unit.primary.item)) : null;
		SecondaryItem si = (unit.secondary.item != null) ? new SecondaryItem(new BaseItem(unit.secondary.item)) : null;
		SoulShard ss     = (unit.soul.item != null) ? new SoulShard(new BaseItem(unit.soul.item)) : null;
		return new UnitEntity(be, pi, si, ss);
	}

	public void PrepareEndCombatTransition()
	{
		ResetCombat();
		if(!EncounterManagement.at_end)
		{
			Application.LoadLevel(Application.loadedLevel);
		}
		else
		{
			SceneManager.LoadPreviousLevel();
		}
	}

	void ResetCombat()
	{
		StopAllCoroutines();
		TimerBasedCombatSystem.Reset();
	}
}
