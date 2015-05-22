using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent (typeof(MasterSystem))]
public class MasterSystemGUI : MonoBehaviour 
{
	public Text attr_lvl_text;
	public Text attr_exp_text;
	public Image attr_exp_bar;

	private float delay = 1/10;

	private MasterSystem _ms;
	private int _lvl_max_amount = 100;
	private int _exp_max_amount = 3000;

	// Use this for initialization
	void Start () {
		_ms = this.GetComponent<MasterSystem>();
		StartCoroutine(MyDelyedUpdate(delay));
	}

	IEnumerator MyDelyedUpdate(float time)
	{
		while(true)
		{
			MyUpdate();

			yield return new WaitForSeconds(time);
		}
	}

	void MyUpdate()
	{
		if(_ms.focused_attr != null)
		{
			attr_lvl_text.text = string.Format("{0}/{1}", _ms.focused_attr.current_lvl, _lvl_max_amount);  // " " for max lvl
			attr_exp_text.text = string.Format("+ {0}/{1}", _ms.focused_attr.amount.ToString("0.##"), _exp_max_amount.ToString("0.##")); // not sure how to get the max lvl from server or if we care to do so
			attr_exp_bar.fillAmount = (float)_ms.focused_attr.amount / (float)_exp_max_amount;
		}
	}
}
