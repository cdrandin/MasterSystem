using UnityEngine;
using System.Collections;

// Ability needs to know the amount in terms of how much to apply the ability affect

public enum ABILITY_LIST
{
	PASSIVE = 0,  			// Nothing
	DEATHBLOW,     			// When killed, do something
	SLAUGHTER,			    // % change to instantly kill target
	FREEZE,					// % chance to freeze target
	SHATTER,			    // install kill target if target has been affected by ATTRIBUTE.FROZEN
	SPREAD_2X,        		// Spread to units to the left and right of focus unit
	SPREAD_ROW,   			// Sread to all units on the focused unit's side
	SPREAD_ALL,   			// Spread to all units
	GAIN_ARMOR,    			// + armor stat
	LOSE_ARMOR,    			// - armor stat
	GAIN_HEALTH,   			// + health
	LOSE_HEALTH,   			// - health
	GAIN_POWER,	   			// + power
	LOSE_POWER,    			// - power
	SELF_DPT,      			// Self deal damage-per-turn
	OTHER_DPT,    			// Other deal damage-per-turn
	SELF_DAMAGE_PER_HIT,	// Self deal damage per hit
	OTHER_DAMAGE_PER_HIT,	// Other deal damage per hit
	NEGATE_MYSTIC,		    // Immune to mystic effect
	NEGATE_PHYSICAL,		// Immune to physical effect
	NEGATE_HARMFUL,
	NEGATE_HELPFUL,
	RANDOM,
	RANDOM_MULTIPLE,
	MAGIC_MISSLE
}

public class Ability
{
	static System.Action<UnitEntity , UnitEntity> DeathblowAbility()
	{
		return delegate(UnitEntity self, UnitEntity target) 
		{
			Debug.Log(string.Format("DEATHBLOW"));
		};
	}

	static System.Action<UnitEntity , UnitEntity> SlaughterAbility()
	{
		return delegate(UnitEntity self, UnitEntity target) 
		{
			bool success = Random.Range(0f,1f) <= .5f; // .1f;
			Debug.Log(string.Format("SLAUGHTER"));
			
			if(success)
			{
				Camera.main.GetComponent<AudioSource>().PlayOneShot(Resources.Load("Sounds/SlaughterSFX") as AudioClip);
				target.base_entity.DamageUnit(target.base_entity.max_hp);
				Debug.Log(string.Format("SLAUGHTER Successful!"));
			}
		};
	}

	static System.Action<UnitEntity , UnitEntity> FreezeAbility()
	{
		return delegate(UnitEntity self, UnitEntity target) 
		{
			bool success = Random.Range(0f,1f) <= 1f; // .25f;
			Debug.Log(string.Format("FREEZE"));
			
			if(success)
			{
				Camera.main.GetComponent<AudioSource>().PlayOneShot(Resources.Load("Sounds/FreezeSFX") as AudioClip);
				target.ApplyBuffable(new Buffable(ATTRIBUTES.FROZEN, 3f, 3f));
				Debug.Log(string.Format("FREEZE Successful!"));
			}
		};
	}

	static System.Action<UnitEntity , UnitEntity> ShatterAbility()
	{
		return delegate(UnitEntity self, UnitEntity target) 
		{
			Debug.Log(string.Format("Shatter!"));
			
			if(target.HasBuffable(ATTRIBUTES.FROZEN))
			{
				target.base_entity.DamageUnit(target.base_entity.max_hp);
				Debug.Log(string.Format("Shatter Successful!"));
			}
		};
	}

	static System.Action<UnitEntity , UnitEntity> MysticShieldAbility()
	{
		return delegate(UnitEntity self, UnitEntity target) 
		{
			Debug.Log(string.Format("Mystic Shield!"));
			
			if(!target.HasBuffable(ATTRIBUTES.NEGATE_MYSTIC))
			{
				target.ApplyBuffable(new Buffable(ATTRIBUTES.NEGATE_MYSTIC, 10f, 10f));
				GameObject obj = PoolingSystem.Instantiate(Resources.Load<GameObject>("Combat/Effects/DivineShieldEffect")) as GameObject;
				obj.name = string.Format("{0}-Mystic Shield", target);
				obj.transform.parent = target.unit_game_object.transform;

				Camera.main.GetComponent<AudioSource>().PlayOneShot(Resources.Load("Sounds/DivineShieldSFX") as AudioClip);

				DelayAction.instance.DelayInf(()=>
				{
				},
				.5f,
				()=>
				{
					if(!target.HasBuffable(ATTRIBUTES.NEGATE_MYSTIC))
					{
						PoolingSystem.Destroy(obj);
						return true;
					}
					
					return false;
				});
			}
		};
	}

	static System.Action<UnitEntity , UnitEntity> MagicMissleAbility()
	{
		return delegate(UnitEntity self, UnitEntity target) 
		{
			int num_missles = 3;
			int min_missle_damage = 2;
			int max_missle_damage = 4;
			int[] missle_damage = new int[num_missles];
			for(int i=0;i<num_missles;++i)
			{
				missle_damage[i] = Random.Range(min_missle_damage, max_missle_damage + 1);
			}
			
			TimerBasedCombatSystem.instance.current_attacking = self;

			UnitEntity[] enemies = TimerBasedCombatSystem.instance.LivingUnitsFrom(TimerBasedCombatSystem.instance.WhoOwnsMe(target)).ToArray();
			int[] random_i = new int[num_missles];

			for (int i = 0; i < random_i.Length; i++)
			{
				random_i[i] = Random.Range(0, enemies.Length);
			}

			GameObject missle_controller = GameObject.Instantiate(Resources.Load<GameObject>("Combat/Effects/magic_missle_shooter")) as GameObject;
			missle_controller.transform.position = self.unit_game_object.transform.position;
			missle_controller.SetActive(true);

			Transform[] pos = new Transform[num_missles];
			System.Action[] events = new System.Action[num_missles];

			for (int i = 0; i < pos.Length; i++)
			{
				int copy_i = i;
				pos[copy_i] = enemies[random_i[i]].unit_game_object.transform;

				events[i] = ()=> // on hits
				{ 
					Camera.main.GetComponent<AudioSource>().PlayOneShot(Resources.Load("Sounds/MagicMissleImpact") as AudioClip);

					CombatTextAnimator.instance.PlayText(string.Format("-{0}", missle_damage[copy_i]), Color.magenta, pos[copy_i].position, .25f);
					enemies[random_i[copy_i]].base_entity.DamageUnit(missle_damage[copy_i]); 
					TimerBasedCombatSystem.instance.UpdateCombatState();
				};
			}

			missle_controller.GetComponent<MagicMissleAnimation>().Setup(num_missles, pos, events);
			missle_controller.GetComponent<MagicMissleAnimation>().Play();
			self.current_item.PlaySoundEffect();

			Debug.Log(string.Format("Magic Missles"));
		};
	}


	static System.Action<UnitEntity , UnitEntity> Spread_2XAbility()
	{
		return delegate(UnitEntity self, UnitEntity target) 
		{
			Debug.Log(string.Format("Spread_2X"));
		};
	}

	static System.Action<UnitEntity , UnitEntity> Other_Damage_Per_HitAbility()
	{
		return delegate(UnitEntity self, UnitEntity target) 
		{
			Debug.Log(string.Format("Other_Damage_Per_Hit"));
		};
	}

	static System.Action<UnitEntity , UnitEntity> PassiveAbility()
	{
		return delegate(UnitEntity self, UnitEntity target) 
		{
			Debug.Log(string.Format("Passive"));
		};
	}


	public static System.Action<UnitEntity , UnitEntity> GetAbility(ABILITY_LIST ability)
	{
		System.Action<UnitEntity , UnitEntity> action = (UnitEntity self, UnitEntity target) => Debug.Log("No ability");

		switch(ability)
		{
		case ABILITY_LIST.DEATHBLOW:
			action = DeathblowAbility();
			break;
		case ABILITY_LIST.SLAUGHTER:
			action = SlaughterAbility();
			break;
		case ABILITY_LIST.FREEZE:
			action = FreezeAbility();
			break;
		case ABILITY_LIST.SHATTER:
			action = ShatterAbility();
			break;
		case ABILITY_LIST.NEGATE_MYSTIC:
			action = MysticShieldAbility();
			break;
		case ABILITY_LIST.SPREAD_2X:
			action = Spread_2XAbility();
			break;
		case ABILITY_LIST.OTHER_DAMAGE_PER_HIT:
			action = Other_Damage_Per_HitAbility();
			break;
		case ABILITY_LIST.MAGIC_MISSLE:
			action = MagicMissleAbility();
			break;
		}

		return action;
	}
}
