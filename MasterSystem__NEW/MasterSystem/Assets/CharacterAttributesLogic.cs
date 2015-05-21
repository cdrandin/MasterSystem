using UnityEngine;
using System.Collections;

[System.Serializable]
public enum ATTRIBUTE_TYPE
{
	STR = 0,
	DEX = 1,
	WILL = 2
}

public class CharacterAttributesLogic
{
	private static int max_level = 100;
	private static int max_attr_amount = 3000;

//	private static int[] valid_exp_amounts = new int[]{100, 1000};

	public static Response AddExp(Request request)
	{
		ServerSideAttribute server_attr = XMLUtil.Deserialize<ServerSideAttribute> (request.payload);
		Attribute attr = server_attr.attr;

		if(attr.current_lvl < max_level)
		{
			// check if exp amount is valid
//			bool valid = false;
//			foreach(int amount in valid_exp_amounts)
//			{
//				if(attr.amount ==  amount)
//				{
//					valid = true;
//					break;
//				}
//			}
//
//			if(valid)
//			{
//				attr.amount += server_attr.amount;
//			}
//			else
//			{
//				attr.amount = 0;
//			}			
			attr.amount += server_attr.amount;

			// lvl up
			if(attr.amount >= max_attr_amount)
			{
				int lvl_gain = attr.amount/max_attr_amount;
				int exp_leftover = attr.amount%max_attr_amount;
				attr.current_lvl = Mathf.Clamp(attr.current_lvl + lvl_gain, 0, max_level);
				attr.amount = 0;
				attr.amount += exp_leftover;
			}
		}
		
		Response response = new Response ();
		response.payload = XMLUtil.Serialize<Attribute>(attr);
		response.error = false;
		return response;
	}
}
