using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TitleScreenScript : MonoBehaviour 
{
	private string remember = "Remember";
	public Button turn;
	public Button time;

	void Awake()
	{
		SetOrientation();
	}

	void Start()
	{
		if(!PlayerPrefs.HasKey(remember))
		{
			PlayerPrefs.DeleteAll();
			PlayerPrefs.SetInt(remember, 1);

			CurrencySingleton.instance.currency.AddTo(CURRENCY_TYPE.DEEP_IRON, 4000000);
			CurrencySingleton.instance.currency.AddTo(CURRENCY_TYPE.DREAM_SHARD, 4000000);
			CurrencySingleton.instance.currency.AddTo(CURRENCY_TYPE.ETHEREAL_DUST, 4000000);
		}

		turn.onClick.AddListener(()=>
		{ 
			Applications.type = COMBAT_TYPE.TURNED; 
			Setup();
		});

		time.onClick.AddListener(()=>
		{ 
			Applications.type = COMBAT_TYPE.TIMED; 
			Setup();
		});
	}

	void SetOrientation()
	{
		Screen.autorotateToLandscapeLeft      = false;
		Screen.autorotateToLandscapeRight     = false;
		Screen.autorotateToPortrait           = true;
		Screen.autorotateToPortraitUpsideDown = true;
		Screen.orientation 					  = ScreenOrientation.AutoRotation;
	}

	void Setup()
	{
		Camera.main.GetComponent<ButtonNextLevel>().NextLevelButton("1_Home_Menu"); 
		//PlayerPrefs.DeleteAll();
	}
}
