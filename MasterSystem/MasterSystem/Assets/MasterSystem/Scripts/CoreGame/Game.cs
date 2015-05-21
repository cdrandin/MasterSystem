using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

[RequireComponent (typeof(HandleEvents), typeof(HandleInputs))]
public class Game : MonoBehaviour 
{
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
	
	public GameObject player_obj;
	// Only care for the positions
	// Assignment on screen will be as such (left, middle, right) [0, 1, 2]
	public GameObject[] all_enemies_obj;
	
	public static UnitEntity player;
	public static UnitEntity[] all_enemies;
	
	private UnitGameobject _player_unit_obj;
	private UnitGameobject[] _all_enemies_unit_obj;
	
	
	public static bool m_init = false;
	
	//HACK: Attempting to get animation to work.
	private GameObject  bossGameObject;
	private UnitAnimation bossAnimation;

	private bool _ai_vote_end_turn;

	// NOTE: If you initialize any extra characters make sure to update there health rects after
	// TurnBasedGUILayout.InitLayout() is called.
	void Awake()
	{
		hero     = PlayerHeroSingleton.instance.main_hero;
		currency = CurrencySingleton.instance.currency;
		
		GetComponent<HandleEvents>().enabled = false;
		m_init = false;

		//GetComponent<Applications>().type = COMBAT_TYPE.TIMED;
		
		SetUpHero();
		
		SetUpEnemies(EncounterManagement.current_wave.enemies);
		if(EncounterManagement.current_wave.ContainBoss())
		{
			SetUpBoss(EncounterManagement.current_wave.boss);
		}
	}
	
	// Use this for initialization
	void Start () 
	{
		GetComponent<HandleEvents>().enabled = true;

		StartCoroutine(UpdateVisuals(0.25f));
		
		m_init = true;

		bool ai_on = true;
		if(ai_on)
		{
			int max_enemies = Mathf.Min(all_enemies.Length, 3);
			float prev_time = 0f;
			for(int i =0 ; i < max_enemies; ++i)
			{				
				UnitEntity unit = all_enemies[i];

				DelayAction.instance.DelayInf(()=> // ****
				{
					if(TurnBasedCombatSystem.instance.current_turn != OWNERSHIP.ENEMY) return;

					_ai_vote_end_turn = true;

					prev_time += Random.Range(0.75f, 1.25f);
					DelayAction.instance.Delay(()=>
					{
						if(TurnBasedCombatSystem.instance.current_turn != OWNERSHIP.ENEMY) return;
						Debug.Log(string.Format("Thinking... {0}", unit));
						System.Func<COMBAT_RETURN_STATUS> ai_action = AI_Timer.Decision(unit);
						TurnBasedCombatSystem.instance.current_combat_status = ai_action();
						if(TurnBasedCombatSystem.instance.current_combat_status != COMBAT_RETURN_STATUS.NO_AVAILABLE_MOVES)
						{
							_ai_vote_end_turn = false;
						}

						Debug.Log(string.Format("Combat return status: {0}", TurnBasedCombatSystem.instance.current_combat_status));
						TurnBasedCombatSystem.instance.UpdateCombatState();

						if(_ai_vote_end_turn)
						{
							DelayAction.instance.Delay(()=>
    	                    {
								TurnBasedCombatSystem.instance.NextTurn();
								_ai_vote_end_turn = true;
							},
							.5f);
						}
					}, 
					prev_time);
				}, 
				.25f,
				()=>
				{ 
					return (unit.IsDead || TurnBasedCombatSystem.instance.winner != OWNERSHIP.UNDEFINED); 
				}); // Not too great, checks every 1s if possible);
			}
		}
		else
			Debug.LogWarning("AI is disable");
		
		DelayAction.instance.DelayInf(()=> // ****
 		{
			OWNERSHIP winner = TurnBasedCombatSystem.instance.winner;
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
		()=> { return TurnBasedCombatSystem.instance.winner != OWNERSHIP.UNDEFINED; });
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(!m_init)
			return;

		if(Input.GetKeyDown(KeyCode.Space))
		{
			TurnBasedCombatSystem.instance.NextTurn();
		}

		if(Input.GetKeyDown(KeyCode.Alpha1))
		{
			System.Func<COMBAT_RETURN_STATUS> ai_action = AI_Timer.Decision(all_enemies[0]);
			TurnBasedCombatSystem.instance.current_combat_status = ai_action();
			Debug.Log(string.Format("Combat return status: {0}", TurnBasedCombatSystem.instance.current_combat_status));
//			TurnBasedCombatSystem.instance.UpdateCombatState();
		}
		
		if(Input.GetKeyDown(KeyCode.Alpha2))
		{
			System.Func<COMBAT_RETURN_STATUS> ai_action = AI_Timer.Decision(all_enemies[1]);
			TurnBasedCombatSystem.instance.current_combat_status = ai_action();
			Debug.Log(string.Format("Combat return status: {0}", TurnBasedCombatSystem.instance.current_combat_status));
//			TurnBasedCombatSystem.instance.UpdateCombatState();
		}
		
		if(Input.GetKeyDown(KeyCode.Alpha3))
		{
			System.Func<COMBAT_RETURN_STATUS> ai_action = AI_Timer.Decision(all_enemies[2]);
			TurnBasedCombatSystem.instance.current_combat_status = ai_action();
			Debug.Log(string.Format("Combat return status: {0}", TurnBasedCombatSystem.instance.current_combat_status));
//			TurnBasedCombatSystem.instance.UpdateCombatState();
		}
	}

	void LateUpdate()
	{
		TurnBasedCombatSystem.instance.UpdateCombatState();
	}

	IEnumerator UpdateVisuals(float delay)
	{
		while(true)
		{
			if(_player_unit_obj != null)
				_player_unit_obj.UpdateGUI();
			
			if(_all_enemies_unit_obj != null)
			{
				foreach(UnitGameobject unit_obj in _all_enemies_unit_obj)
				{
					unit_obj.UpdateGUI();
				}
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
		
		PrimaryItem   pi = new PrimaryItem(Items.CreatePrimaryWeapon0());
		pi.item.on_use_sound_effect_resource_path = "Sounds/rip+struck";
		
		SecondaryItem si = new SecondaryItem(Items.CreateSecondaryWeapon0());
		si.item.on_use_sound_effect_resource_path = "Sounds/armor_ching";
		
		SoulShard     ss = new SoulShard(Items.CreateSoulShard0());
		ss.item.on_use_sound_effect_resource_path = "Sounds/healing_chime";
		
		player           = new UnitEntity(be, pi, si, ss);
		
		// Find Gameobject for the unit
		_player_unit_obj = player_obj.GetComponent<UnitGameobject>();
		// Set the unit's UnitEntity component to this generated one
		_player_unit_obj._unit_entity = player;
		_player_unit_obj.Setup();
		
		// Set weak ref of the actual GameObject to the UnitEntity
		player.SetWeakRefUnitGameObject(_player_unit_obj);
		
		TurnBasedCombatSystem.instance.UnitsFrom(OWNERSHIP.PLAYER).Add(player);
	}

	void SetUpBoss(EnemyEntity boss_ee)
	{
		// fill up last element in the available slots, for the boss.
		int l = _all_enemies_unit_obj.Length;
		
		all_enemies_obj[2].SetActive(true);
		all_enemies[l - 1] = PrepareUnitEntitySO(boss_ee.unit_entity_so);
		all_enemies[l - 1].Reset();
		all_enemies[l -1].EquipToPrimary(new BaseItem("Boss slash", 0, 10, 3, "slash", WEAPON_HANDLE.SINGLE_HANDED, HARMFULNESS.HARMFUL, ACTION_LIST.ON_ATTACK, ABILITY_LIST.PASSIVE));
		all_enemies[l -1].primary.item.on_use_sound_effect_resource_path = boss_ee.unit_entity_so.primary.item.on_use_sound_effect_resource_path;

		_all_enemies_unit_obj[l - 1] = all_enemies_obj[2].GetComponent<UnitGameobject>();
		_all_enemies_unit_obj[l - 1]._unit_entity = all_enemies[l - 1];
		_all_enemies_unit_obj[l - 1].Setup();
		
		all_enemies[l - 1].SetWeakRefUnitGameObject(_all_enemies_unit_obj[l - 1]);
		
		all_enemies_obj[2].GetComponent<Renderer>().material.mainTexture = boss_ee.unit_entity_so.unit_image;
		
		TurnBasedCombatSystem.instance.UnitsFrom(OWNERSHIP.ENEMY).Add(all_enemies[l - 1]);
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
			all_enemies[i].Reset();
			all_enemies[i].EquipToPrimary(new BaseItem("Minion Slash", 0, 2, 1, "Slash", WEAPON_HANDLE.SINGLE_HANDED, HARMFULNESS.HARMFUL, ACTION_LIST.ON_ATTACK, ABILITY_LIST.PASSIVE));
			all_enemies[i].primary.item.on_use_sound_effect_resource_path = minions_only[i].unit_entity_so.primary.item.on_use_sound_effect_resource_path;
			
			_all_enemies_unit_obj[i] = all_enemies_obj[i].GetComponent<UnitGameobject>();
			_all_enemies_unit_obj[i]._unit_entity = all_enemies[i];
			_all_enemies_unit_obj[i].Setup();
			
			all_enemies[i].SetWeakRefUnitGameObject(_all_enemies_unit_obj[i]);
			
			_all_enemies_unit_obj[i].gameObject.GetComponent<Renderer>().material.mainTexture = minions_only[i].unit_entity_so.unit_image;
			
			TurnBasedCombatSystem.instance.UnitsFrom(OWNERSHIP.ENEMY).Add(all_enemies[i]);
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
		TurnBasedCombatSystem.Reset();
	}

	public void NextTurn()
	{
		// Player shouldn't be able to control enemies end turn event
		if(TurnBasedCombatSystem.instance.current_turn == OWNERSHIP.PLAYER)
		{
			TurnBasedCombatSystem.instance.NextTurn();
			player.ReleaseCharge();
		}
	}

	void OnGUI()
	{
		if(!m_init)
			return;

		// Display end turn button
		if(TurnBasedCombatSystem.instance.current_turn == OWNERSHIP.PLAYER)
		{
			GUI.skin.button.fontSize = (int)(Screen.height*(1.0f/10.0f) / 5.0f);
		}
		
		GUI.skin.button.fontSize = (int)(Screen.height*(1.0f/10.0f) / 5.0f);
		GUI.Label(new Rect(Screen.width/6.5f, 0.0f, Screen.width/**(1.0f/2.5f)*/, Screen.height*(1.0f/15.0f)), string.Format("\tCurrent turn: {0}", TurnBasedCombatSystem.instance.current_turn));
	}
}
