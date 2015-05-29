using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public enum ATTRIBUTE_TYPE
{
	STR = 0,
	DEX = 1,
	WILL = 2,
	NONE = 3
}

public class CharacterAttributesLogic
{
	private static int max_level = 100;
	private static int max_attr_amount = 3000;

	// public just so I can modify directly "illegally"
	public static string server_side_character_attributes_id = "server_side_character_attributes";

//	public static string server_side_character_list_attributes_id = "server_side_character_list_attributes";

//	private static int[] valid_exp_amounts = new int[]{100, 1000};

	private static void SetAttributeToServerSideAttributes(ATTRIBUTE_TYPE type, Attribute attr)
	{
		ServerSideUpdateAttribute ssua = GetServerSideAttributes();

		switch(type)
		{
		case ATTRIBUTE_TYPE.STR:
			ssua.str = attr;
			break;
		case ATTRIBUTE_TYPE.DEX:
			ssua.dex = attr;
			break;
		case ATTRIBUTE_TYPE.WILL:
			ssua.will = attr;
			break;
		}

		SimpleSerializer.Save<ServerSideUpdateAttribute>(server_side_character_attributes_id, ssua);
	}

	public static Attribute GetServerSideAttribute(ATTRIBUTE_TYPE type)
	{
		ServerSideUpdateAttribute ssua = GetServerSideAttributes();
		Attribute attr = null;

		switch(type)
		{
		case ATTRIBUTE_TYPE.STR:
			attr = ssua.str;
			break;
		case ATTRIBUTE_TYPE.DEX:
			attr = ssua.dex;
			break;
		case ATTRIBUTE_TYPE.WILL:
			attr = ssua.will;
			break;
		}

		return attr;
	}

	public static ServerSideUpdateAttribute GetServerSideAttributes()
	{
//		ServerSideUpdateAttribute ssua = SimpleSerializer.Load<ServerSideUpdateAttribute>(server_side_character_attributes_id);
		GetCreatePair<ServerSideUpdateAttribute> ssua_pair = SimpleSerializer.GetOrCreateWithStatus<ServerSideUpdateAttribute>(server_side_character_attributes_id);

		if(ssua_pair.created)
		{
			Debug.Log("Creating new server side attributes object");

//			ssua = new ServerSideUpdateAttribute();
			ssua_pair.obj.str = new Attribute(ATTRIBUTE_TYPE.STR);
			ssua_pair.obj.dex = new Attribute(ATTRIBUTE_TYPE.DEX);
			ssua_pair.obj.will = new Attribute(ATTRIBUTE_TYPE.WILL);
//			SimpleSerializer.Save<ServerSideUpdateAttribute>(server_side_character_attributes_id, ssua);
		}

		return ssua_pair.obj;
	}

	public static Response AddAttributeExp(Request request)
	{
		ServerSideAttribute server_attr = XMLUtil.Deserialize<ServerSideAttribute> (request.payload);
		Attribute attr = server_attr.attr;
//		Attribute attr = ServerSidePersistantDataMultipleCharacterAttribute.instance.GetCharacter(server_attr.character_id).GetAttribute(server_attr.attr.attr_type);

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

			// spend attr exp pts
			attr.current_exp_amount += server_attr.exp_amount;
			TrainerInfoLogic.AddToServerSidePlayerAttrExp(-server_attr.exp_amount);

			// lvl up
			if(attr.exp_amount >= max_attr_amount)
			{
				int lvl_gain = attr.exp_amount/max_attr_amount;
				int exp_leftover = attr.exp_amount%max_attr_amount;
				attr.current_lvl = Mathf.Clamp(attr.current_lvl + lvl_gain, 0, max_level);
				attr.exp_amount = 0;
				attr.current_exp_amount += exp_leftover;
			}

			SetAttributeToServerSideAttributes(attr.attr_type, attr);
		}
		
		Response response = new Response ();
		response.payload = XMLUtil.Serialize<Attribute>(attr);
		response.error = false;
		return response;
	}


	public static Response UpdateAttributes(Request request)
	{
		Debug.LogError(request.payload);
		
		ServerSideUpdateAttribute ssua = GetServerSideAttributes();
		Debug.Log(string.Format("UpdateAttributes: {0}", ssua));

		// Just send an update of the characters attributes
		Response response = new Response ();
		response.payload = XMLUtil.Serialize<ServerSideUpdateAttribute>(ssua);
		response.error = false;
		return response;
	}

	public static Response SubmitAttrExpLevel(Request request)
	{
		Response response = new Response ();
		response.payload = "XMLUtil.Serialize<Attribute>(attr)";
		response.error = false;
		return response;
	}
}
