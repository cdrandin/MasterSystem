using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HeroMenuData : MonoBehaviour 
{
	public Text hero_name_text;
	public Text level_value_text;
	public Text health_value_text;
	public Text armour_value_text;

	public Text deep_iron_amount_text;
	public Text dream_shard_amount_text;
	public Text ethereal_dust_amount_text;

	public Text exp_amount_text;

	public GameObject helper_text_object;

	void OnEnable()
	{
		UpdateHeroStats();
		UpdateCurrency();
	}

	public void UpdateHeroStats()
	{
		HeroEntityPD hero = PlayerHeroSingleton.instance.main_hero;

		if(hero != null)
		{
			if(hero_name_text != null)
			{
				hero_name_text.text = hero.base_entity_pd.name.ToUpper();
			}
			
			if(level_value_text != null)
			{
				level_value_text.text = hero.base_entity_pd.level.ToString();
			}
			
			if(health_value_text != null)
			{
				health_value_text.text = hero.base_entity_pd.health.ToString();
			}
			
			if(armour_value_text != null)
			{
				armour_value_text.text = hero.base_entity_pd.armor_rating.ToString();
			}

			if(exp_amount_text != null)
			{
				exp_amount_text.text = string.Format("{0} (current) / {1} (max)", hero.experience, hero.max_experience);
			}

			if(hero.base_entity_pd.health <= hero.base_entity_pd.max_health/2f)
			{
				helper_text_object.SetActive(true);
			}
			else
			{
				helper_text_object.SetActive(false);
			}
		} // end hero
	}

	public void UpdateCurrency()
	{
		Currency currency = CurrencySingleton.instance.currency;

		if(currency != null)
		{
			if(deep_iron_amount_text != null)
			{
				deep_iron_amount_text.text = currency.deep_iron_amount.ToString("N0");
			}
			
			if(dream_shard_amount_text != null)
			{
				dream_shard_amount_text.text = currency.dream_shard_amount.ToString("N0");
			}
			
			if(ethereal_dust_amount_text != null)
			{
				ethereal_dust_amount_text.text = currency.ethereal_dust_amount.ToString("N0");
			}
		} // end currency
	}

	public void GainFullHealth()
	{
		PlayerHeroSingleton.instance.main_hero.base_entity_pd.health = PlayerHeroSingleton.instance.main_hero.base_entity_pd.max_health;
		UpdateHeroStats();
		PlayerHeroSingleton.Save();
	}

	public void UpdateAllData()
	{
		UpdateHeroStats();
		UpdateCurrency();
	}

	public void ResetHero()
	{
		PlayerHeroSingleton.Reset();
		PlayerHeroSingleton.Save();
		UpdateHeroStats();
	}

	public void ResetCurrency()
	{
		CurrencySingleton.Reset();
		CurrencySingleton.Save();
		UpdateCurrency();
	}
}
