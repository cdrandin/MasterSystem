using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Items 
{
	// Current premade items
	// Later, custome GUI to create objects

	private const int FAST_COOLDOWN = 3;
	private const int MEDIUM_COOLDOWN = 10;
	private const int LONG_COOLDOWN = 30;

	// 	BaseItem(string name, int usage_cost, int amt, int downtime_after_usage, string text, WEAPON_HANDLE hand, HARMFULNESS harm, ACTION_LIST action, ABILITY_LIST ability)

	#region Primary Weapons
	public static BaseItem CreatePrimaryWeapon0()
	{
		return new BaseItem("Vampire Blade", 0, 5, 1,"Deathblow: Gain 10 health if target hit dies", WEAPON_HANDLE.SINGLE_HANDED, HARMFULNESS.HARMFUL, ACTION_LIST.AFTER_ATTACK, ABILITY_LIST.DEATHBLOW);
	}

	public static BaseItem CreatePrimaryWeapon0_Timer()
	{
		return new BaseItem("Vampire Blade", 0, 3, 1,"Deathblow: Gain 10 health if target hit dies", WEAPON_HANDLE.SINGLE_HANDED, HARMFULNESS.HARMFUL, ACTION_LIST.AFTER_ATTACK, ABILITY_LIST.DEATHBLOW);
	}

	public static BaseItem CreatePrimaryWeapon0Ability0_Timer_UPDATE()
	{
		return new BaseItem("Light Sword/Slash", 0, 3, FAST_COOLDOWN,"None", WEAPON_HANDLE.SINGLE_HANDED, HARMFULNESS.HARMFUL, ACTION_LIST.ON_ATTACK, ABILITY_LIST.PASSIVE);
	}

	public static BaseItem CreatePrimaryWeapon0Ability1_Timer_UPDATE()
	{
		return new BaseItem("Light Sword/EnergySword", 15, 7, MEDIUM_COOLDOWN,"10% Chance to slay enemy outright", WEAPON_HANDLE.SINGLE_HANDED, HARMFULNESS.HARMFUL, ACTION_LIST.ON_ATTACK, ABILITY_LIST.DEATHBLOW);
	}

	// Warrior
	public static BaseItem CreateWarriorPrimaryWeapon0Ability0_Timer()
	{
		BaseItem item = new BaseItem("Slash", 0, 3, FAST_COOLDOWN,"3 MELEE DAMAGE, 5-SEC COOLDOWN, 0 SPIRIT COST", WEAPON_HANDLE.SINGLE_HANDED, HARMFULNESS.HARMFUL, ACTION_LIST.ON_ATTACK, ABILITY_LIST.PASSIVE);
		item.SetImage(Resources.Load("Textures/Abilities/ability_prim_attack1_FPO") as Texture2D);
		item.SetSoundEffectAudio(Resources.Load("Sounds/rip+struck") as AudioClip);
		return item;
	}
	public static BaseItem CreateWarriorPrimaryWeapon0Ability1_Timer()
	{
		BaseItem item = new BaseItem("Melt", 15, 8, LONG_COOLDOWN,"8 MELEE DAMAGE, 10% CHANCE TO SLAY ENEMY OUTRIGHT  30 SEC COOLDOWN, 15 SPIRIT COST", WEAPON_HANDLE.SINGLE_HANDED, HARMFULNESS.HARMFUL, ACTION_LIST.ON_ATTACK, ABILITY_LIST.SLAUGHTER);
		item.SetImage(Resources.Load("Textures/Abilities/ability_prim_attack2_FPO") as Texture2D);
		item.SetSoundEffectAudio(Resources.Load("Sounds/rip+struck") as AudioClip);
		item.item_fx = Resources.Load("CustomFX/FX_EnergySwordCompiled") as GameObject;
		item.ability_type = ABILITY_TYPE.MELEE;
		return item;
	}

	// Mystic
	public static BaseItem CreateMysticPrimaryWeapon0Ability0_Timer()
	{
		BaseItem item = new BaseItem("Energy Attack", 5, 5, FAST_COOLDOWN, "5 MYSTIC DAMAGE, 5-SEC COOLDOWN, 5 SPIRIT COST", WEAPON_HANDLE.SINGLE_HANDED, HARMFULNESS.HARMFUL, ACTION_LIST.ON_ATTACK, ABILITY_LIST.PASSIVE);
		item.SetImage(Resources.Load("Textures/Abilities/ability_staff_energy_FPO") as Texture2D);
//		item.SetSoundEffectAudio(Resources.Load("Sounds/rip+struck") as AudioClip);
//		item.item_fx = Resources.Load("CustomFX/FX_EnergySwordCompiled") as GameObject;
		item.ability_type = ABILITY_TYPE.MYSTIC;
		return item;
	}
	public static BaseItem CreateMysticPrimaryWeapon0Ability1_Timer()
	{
		BaseItem item = new BaseItem("Magic Missle", 20, 0, LONG_COOLDOWN,"2-4 MYSTIC DAMAGE/MISSILE, 3 MISSILES THAT STRIKE ENEMIES AT RANDOM, 20 SEC COOLDOWN, 20 SPIRIT COST", WEAPON_HANDLE.SINGLE_HANDED, HARMFULNESS.HARMFUL, ACTION_LIST.ON_ATTACK, ABILITY_LIST.MAGIC_MISSLE);
		item.SetImage(Resources.Load("Textures/Abilities/ability_staff_magicmissles_FPO") as Texture2D);
		item.ability_type = ABILITY_TYPE.MYSTIC;
		item.on_use_sound_effect_resource_path = "Sounds/MagicMissleTravel";

		return item;
	}

	// Ranger
	public static BaseItem CreateRangerPrimaryWeapon0Ability0_Timer()
	{
		BaseItem item = new BaseItem("Arrow", 0, 3, FAST_COOLDOWN, "3 RANGED DAMAGE, 5-SEC COOLDOWN, 0 SPIRIT COST", WEAPON_HANDLE.SINGLE_HANDED, HARMFULNESS.HARMFUL, ACTION_LIST.ON_ATTACK, ABILITY_LIST.PASSIVE);
		item.SetImage(Resources.Load("Textures/Abilities/ability_bow_arrow1_FPO") as Texture2D);
		item.ability_type = ABILITY_TYPE.RANGED;
		return item;
	}
	
	public static BaseItem CreateRangerPrimaryWeapon0Ability1_Timer()
	{
		BaseItem item = new BaseItem("Ice Arrow", 15, 6, LONG_COOLDOWN, "3-7 RANGED DAMAGE,25% CHANCE TO FREEZE ENEMY FOR 3 SEC, 35  SEC COOLDOWN, 15 SPIRIT COST", WEAPON_HANDLE.SINGLE_HANDED, HARMFULNESS.HARMFUL, ACTION_LIST.ON_ATTACK, ABILITY_LIST.FREEZE);
		item.SetImage(Resources.Load("Textures/Abilities/ability_bow_ice-arrow_FPO") as Texture2D);
		item.ability_type = ABILITY_TYPE.RANGED;
		return item;
	}

	// Minions
	public static BaseItem CreateBasicMinionPrimaryWeapon0Ability0_Timer()
	{
		BaseItem item = new BaseItem("Basic Sword", 0, 3, 5, "Basic minion attack", WEAPON_HANDLE.SINGLE_HANDED, HARMFULNESS.HARMFUL, ACTION_LIST.ON_ATTACK, ABILITY_LIST.PASSIVE);
		item.ability_type = ABILITY_TYPE.MYSTIC;
		item.item_fx = Resources.Load("CustomFX/CFXM_Slash+Text") as GameObject;
		item.on_use_sound_effect_resource_path = "Sounds/rip+struck";
		
		return item;
	}

	public static BaseItem CreateBasicBoss0PrimaryWeapon0Ability0_Timer()
	{
		BaseItem item = new BaseItem("Basic Sword", 0, 3, 5, "Boss0 attack", WEAPON_HANDLE.SINGLE_HANDED, HARMFULNESS.HARMFUL, ACTION_LIST.ON_ATTACK, ABILITY_LIST.PASSIVE);
		item.ability_type = ABILITY_TYPE.MYSTIC;
		item.item_fx = Resources.Load("CustomFX/CFXM_Slash+Text") as GameObject;
		item.on_use_sound_effect_resource_path = "Sounds/rip+struck";
		
		return item;
	}

	public static BaseItem CreatePrimaryWeapon1()
	{
		return new BaseItem("Sulfuras Hand of Ragnaros", 2, 10, 2, "Cleave: Deals 2 damage to the attacked near by units", WEAPON_HANDLE.DOUBLE_HANDED, HARMFULNESS.HARMFUL, ACTION_LIST.AFTER_ATTACK, ABILITY_LIST.SPREAD_2X);
	}

	public static BaseItem CreatePrimaryWeapon1_Timer()
	{
		return new BaseItem("Sulfuras Hand of Ragnaros", 0, 7, 5, "Cleave: Deals 2 damage to the attacked near by units", WEAPON_HANDLE.DOUBLE_HANDED, HARMFULNESS.HARMFUL, ACTION_LIST.AFTER_ATTACK, ABILITY_LIST.SPREAD_2X);
	}
	#endregion

	#region Secondary Weapons
	public static BaseItem CreateSecondaryWeapon0()
	{
		return new BaseItem("Dire Shield", 1, 5, 0, "Gain 5 armor", WEAPON_HANDLE.SINGLE_HANDED, HARMFULNESS.Utility, ACTION_LIST.ON_DEFEND, ABILITY_LIST.GAIN_ARMOR);
	}

	public static BaseItem CreateSecondaryWeapon0_Timer()
	{
		return new BaseItem("Dire Shield", 0, 5, 30, "Gain 5 armor. Recharge: 30 seconds", WEAPON_HANDLE.SINGLE_HANDED, HARMFULNESS.Utility, ACTION_LIST.ON_DEFEND, ABILITY_LIST.GAIN_ARMOR);
	}

	public static BaseItem CreateSecondaryWeapon0Ability0_Timer_UPDATE()
	{
		return new BaseItem("Mind Shield/Armor", 0, 5, MEDIUM_COOLDOWN,"None", WEAPON_HANDLE.SINGLE_HANDED, HARMFULNESS.Utility, ACTION_LIST.ON_DEFEND, ABILITY_LIST.PASSIVE);
	}

	public static BaseItem CreateSecondaryWeapon0Ability1_Timer_UPDATE()
	{
		return new BaseItem("Mind Shield/Shield Bash", 20, 9, MEDIUM_COOLDOWN,"None", WEAPON_HANDLE.SINGLE_HANDED, HARMFULNESS.HARMFUL, ACTION_LIST.ON_ATTACK, ABILITY_LIST.PASSIVE);
	}

	// Warrior
	public static BaseItem CreateWarriorSecondaryWeapon0Ability0_Timer()
	{
		BaseItem item = new BaseItem("Shield", 0, 5, MEDIUM_COOLDOWN, "+5 ARMOR, 20 SEC COOLDOWN, 0 SPIRIT COST", WEAPON_HANDLE.SINGLE_HANDED, HARMFULNESS.Utility, ACTION_LIST.ON_DEFEND, ABILITY_LIST.PASSIVE);
		item.SetImage(Resources.Load("Textures/Abilities/ability_sec_shield_FPO") as Texture2D);
		item.SetSoundEffectAudio(Resources.Load("Sounds/armor_ching") as AudioClip);
		item.ability_type = ABILITY_TYPE.UNKNOWN;
		return item;
	}
	public static BaseItem CreateWarriorSecondaryWeapon0Ability1_Timer()
	{
		BaseItem item = new BaseItem("MYSTIC SHIELD", 15, 0, LONG_COOLDOWN, "NEXT MYSTIC ATTACK IS NEGATED, 30 SEC COOLDOWN, 15 SPIRIT COST", WEAPON_HANDLE.SINGLE_HANDED, HARMFULNESS.Utility, ACTION_LIST.ON_DEFEND, ABILITY_LIST.NEGATE_MYSTIC);
//		item.item_fx = Resources.Load("CustomFX/FX_Shatter") as GameObject;
		item.SetImage(Resources.Load("Textures/Abilities/ability_shield_mysticshield_FPO") as Texture2D);
		item.ability_type = ABILITY_TYPE.UNKNOWN;
		return item;
	}

	// Mystic
	public static BaseItem CreateMysticSecondaryWeapon0Ability0_Timer()
	{
		BaseItem item = new BaseItem("Shatter", 30, 0, MEDIUM_COOLDOWN, "SHATTERS ANY FROZEN ENEMY DESTROYING IT, 20 SEC COOLDOWN, 30 SPIRIT COST", WEAPON_HANDLE.SINGLE_HANDED, HARMFULNESS.HARMFUL, ACTION_LIST.ON_ATTACK, ABILITY_LIST.SHATTER);
		item.SetImage(Resources.Load("Textures/Abilities/ability_amulet_shatter_FPO") as Texture2D);
		item.item_fx = Resources.Load("CustomFX/FX_Shatter") as GameObject;
		item.ability_type = ABILITY_TYPE.MYSTIC;
		return item;
	}
	public static BaseItem CreateMysticSecondaryWeapon0Ability1_Timer()
	{
		BaseItem item = new BaseItem("The Dark", 50, 15, LONG_COOLDOWN, "SUMMONS DARK SPIRITS THAT SLAY ENEMY, 50 SEC COOLDOWN, 50 SPIRIT COST", WEAPON_HANDLE.SINGLE_HANDED, HARMFULNESS.HARMFUL, ACTION_LIST.ON_ATTACK, ABILITY_LIST.PASSIVE);
		item.SetImage(Resources.Load("Textures/Abilities/ability_amulet_thedark_FPO") as Texture2D);
		item.ability_type = ABILITY_TYPE.MYSTIC;
		return item;
	}

	// Ranger
	public static BaseItem CreateRangerSecondaryWeapon0Ability0_Timer()
	{
		BaseItem item = new BaseItem("Shield", 0, 5, MEDIUM_COOLDOWN, "+5 ARMOR, 20 SEC COOLDOWN, 0 SPIRIT COST", WEAPON_HANDLE.SINGLE_HANDED, HARMFULNESS.Utility, ACTION_LIST.ON_DEFEND, ABILITY_LIST.PASSIVE);
		item.SetImage(Resources.Load("Textures/Abilities/ability_shield_shield2_FPO") as Texture2D);
		item.SetSoundEffectAudio(Resources.Load("Sounds/armor_ching") as AudioClip);
		item.ability_type = ABILITY_TYPE.UNKNOWN;
		return item;
	}

	public static BaseItem CreateRangerSecondaryWeapon0Ability1_Timer()
	{
		BaseItem item = new BaseItem("Heal", 30, 20, LONG_COOLDOWN, "HEALS ANY CHARACTER FOR 20 HP, 30 SEC COOLDOWN,30  SPIRIT COST", WEAPON_HANDLE.SINGLE_HANDED, HARMFULNESS.HELPFUL, ACTION_LIST.ON_HEAL, ABILITY_LIST.PASSIVE);
		item.item_fx = Resources.Load("CustomFX/FX_HealWard") as GameObject;
		item.SetSoundEffectAudio(Resources.Load("Sounds/healing_chime") as AudioClip);
		item.SetImage(Resources.Load("Textures/Abilities/ability_shield_heal_FPO") as Texture2D);
		item.ability_type = ABILITY_TYPE.MYSTIC;
		return item;
	}

	#endregion

	#region Soulshard
	public static BaseItem CreateSoulShard0()
	{
		return new BaseItem("Heal", 2, 10, 3, "Heal 10 points of damage. Recharge: 2 Turns", WEAPON_HANDLE.UNDEFINED, HARMFULNESS.HELPFUL, ACTION_LIST.ON_HEAL, ABILITY_LIST.GAIN_HEALTH);
	}

	public static BaseItem CreateSoulShard0_Timer()
	{
		return new BaseItem("Heal", 0, 10, 120, "Heal 10 points of damage. Recharge: 120 seconds", WEAPON_HANDLE.UNDEFINED, HARMFULNESS.HELPFUL, ACTION_LIST.ON_HEAL, ABILITY_LIST.GAIN_HEALTH);
	}

	public static BaseItem CreateSoulShard0Ability0_Timer_UPDATE()
	{
		return new BaseItem("Talisman/Heal", 50, 15, LONG_COOLDOWN,"None", WEAPON_HANDLE.SINGLE_HANDED, HARMFULNESS.HELPFUL, ACTION_LIST.ON_HEAL, ABILITY_LIST.PASSIVE);
	}

	// Warrior
	public static BaseItem CreateWarriorSoulShardWeapon0Ability0_Timer()
	{
		BaseItem item = new BaseItem("Heal", 30, 10, LONG_COOLDOWN, "+10 Health, 30 SEC COOLDOWN, 30 SPIRIT COST", WEAPON_HANDLE.SINGLE_HANDED, HARMFULNESS.HELPFUL, ACTION_LIST.ON_HEAL, ABILITY_LIST.PASSIVE);
		item.item_fx = Resources.Load("CustomFX/FX_HealWard") as GameObject;
		item.SetImage(Resources.Load("Textures/Abilities/ability_class_heal_FPO") as Texture2D);
		item.SetSoundEffectAudio(Resources.Load("Sounds/healing_chime") as AudioClip);
		item.ability_type = ABILITY_TYPE.MYSTIC;
		return item;
	}

	// Mystic
	public static BaseItem CreateMysticSoulShardWeapon0Ability0_Timer()
	{
		return null;
	}

	// Ranger
	public static BaseItem CreateRangerSoulShardWeapon0Ability0_Timer()
	{
		return null;
	}
	#endregion
}
