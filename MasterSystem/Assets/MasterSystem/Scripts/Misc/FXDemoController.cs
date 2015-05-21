using UnityEngine;
using System.Collections;

public class FXDemoController : MonoBehaviour 
{
	public enum e_FXType {HEALWARD, THEDARK, SHATTER, MAGICMISSILE, MAGICSHIELD, ENERGYSWORDCOMPILED};

	public GameObject FX_HealWard;
	public GameObject FX_TheDark;
	public GameObject FX_Shatter;
	public GameObject FX_MagicMissileCompiled;
	public GameObject FX_MagicShield;
	public GameObject FX_EnergySwordCompiled;

	public GameObject characterPortrait;

	public void SpawnPrefab(string FXName)
	{
		switch(FXName)
		{
			case "EnergySword":
				if(this.FX_EnergySwordCompiled != null)
				{
					GameObject go = (GameObject)Instantiate(this.FX_EnergySwordCompiled, characterPortrait.transform.position, this.FX_EnergySwordCompiled.transform.rotation);
				}
				break;

			case "HealWard":
				if(this.FX_HealWard != null)
				{
					GameObject go = (GameObject)Instantiate(this.FX_HealWard, characterPortrait.transform.position, this.FX_HealWard.transform.rotation);
				}
				break;

			case "MagicMissile":
				if(this.FX_MagicMissileCompiled != null)
				{
					GameObject go = (GameObject)Instantiate(this.FX_MagicMissileCompiled, characterPortrait.transform.position, this.FX_MagicMissileCompiled.transform.rotation);
				}
				break;

			case "MagicShield":
				if(this.FX_MagicShield != null)
				{
					this.FX_MagicShield.SetActive(!this.FX_MagicShield.activeInHierarchy);
				}
				break;

			
			case "Shatter":
				if(this.FX_Shatter != null)
				{
					GameObject go = (GameObject)Instantiate(this.FX_Shatter, characterPortrait.transform.position, this.FX_Shatter.transform.rotation);
				}
				break;

			case "TheDark":
				if(this.FX_TheDark != null)
				{
					GameObject go = (GameObject)Instantiate(this.FX_TheDark, characterPortrait.transform.position, this.FX_TheDark.transform.rotation);
				}
				break;

			
		}
	}
}
