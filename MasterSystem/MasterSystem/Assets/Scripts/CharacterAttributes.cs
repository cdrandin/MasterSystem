using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Attribute
{
	public int current_exp_amount;
	public int exp_amount;
	public int current_lvl;
	public ATTRIBUTE_TYPE attr_type; // don't like this, but so far easy way to distunigsh stats on both server/client

	public Attribute()
	{
		this.exp_amount = 0;
		this.current_lvl = 1;
		this.attr_type = ATTRIBUTE_TYPE.NONE;
	}

	public Attribute(ATTRIBUTE_TYPE type)
	{
		this.exp_amount = 0;
		this.current_lvl = 1;
		this.attr_type = type;
	}

	public override string ToString ()
	{
		return string.Format ("[Attribute]: Type: {0} LvL: {1}  Exp: {2}", this.attr_type, this.current_lvl, this.current_exp_amount);
	}
}

// Update Attribute objects
// This deals with pinging the server the give an updated values of the player's attributes
[System.Serializable]
public class ServerSideUpdateAttribute
{
	public Attribute str;
	public Attribute will;
	public Attribute dex;

	public override string ToString ()
	{
		return string.Format ("[ServerSideUpdateAttribute]: Str: {0},  Dex: {1},  Will: {2}", this.str, this.dex, this.will);
	}
}


[System.Serializable]
public class ServerSideAttribute
{
	public Attribute attr;
	public int exp_amount;
}


// Server side persistant data class
// Really bad way of doing it since classes may end up becoming very bloated and requires writing and read all of it everytime
// will do for now
[System.Serializable]
public class ServerSidePersistantDataMultipleCharacterAttribute
{
	private static ServerSidePersistantDataMultipleCharacterAttribute _instance;
	public static ServerSidePersistantDataMultipleCharacterAttribute instance
	{
		get	
		{
			if(_instance == null)
			{
				_instance = new ServerSidePersistantDataMultipleCharacterAttribute();
			}

			return _instance;	
		}
	}

	private static string server_side_character_list_attributes_id = "server_side_character_list";

	public List<ServerSiderPersistantDataCharacter> characters;
	public ServerSiderPersistantDataCharacter GetCharacter(string id)
	{
		foreach(ServerSiderPersistantDataCharacter character in characters)
		{
			if(character.id == id)
			{
				return character;
			}
		}

		return null;
	}

	private ServerSidePersistantDataMultipleCharacterAttribute()
	{
		GetCreatePair<List<ServerSiderPersistantDataCharacter>> pair = SimpleSerializer.GetOrCreateWithStatus<List<ServerSiderPersistantDataCharacter>>(server_side_character_list_attributes_id);

		// new, set 3 characters up
		if(pair.created)
		{
			characters = new List<ServerSiderPersistantDataCharacter>();
			characters.Add(new ServerSiderPersistantDataCharacter("Ranger"));
			characters.Add(new ServerSiderPersistantDataCharacter("Warrior"));
			characters.Add(new ServerSiderPersistantDataCharacter("Mystic"));
			Save(); // don't forget to save
		}
		else
		{
			characters = pair.obj;
		}
	}

	public void Save()
	{
		SimpleSerializer.Save<List<ServerSiderPersistantDataCharacter>>(server_side_character_list_attributes_id, characters);
	}
}

// This class compasses a single unit. Which can contain all stat info, etc, etc
// Right now it has a list of attributes
[System.Serializable]
public class ServerSiderPersistantDataCharacter
{
	public string id;
	public ServerSiderPersistantDataCharacterAttribute character_attribute;

	public ServerSiderPersistantDataCharacter()
	{
		character_attribute = new ServerSiderPersistantDataCharacterAttribute();
	}

	public ServerSiderPersistantDataCharacter(string id)
	{
		this.id = id;
		character_attribute = new ServerSiderPersistantDataCharacterAttribute();
	}

	public override string ToString ()
	{
		return string.Format ("[ServerSiderPersistantDataCharacter]: id: {0}  {1}", this.id, character_attribute);
	}
}

// Similiar to ServerSideUpdateAttribute, but keeping it seperate for strictness
[System.Serializable]
public class ServerSiderPersistantDataCharacterAttribute
{
	public Attribute str;
	public Attribute will;
	public Attribute dex;
		
	public ServerSiderPersistantDataCharacterAttribute()
	{
		this.str = new Attribute(ATTRIBUTE_TYPE.STR);
		this.dex = new Attribute(ATTRIBUTE_TYPE.DEX);
		this.will = new Attribute(ATTRIBUTE_TYPE.WILL);
	}

	public override string ToString ()
	{
		return string.Format ("[ServerSiderPersistantDataCharacterAttribute]: Str: {0},  Dex: {1},  Will: {2}", this.str, this.dex, this.will);
	}
}
