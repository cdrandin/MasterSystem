using UnityEngine;
using System.Collections;

public enum COMBAT_TYPE
{
	TURNED,
	TIMED
}

public class Applications
{
	public static COMBAT_TYPE type = COMBAT_TYPE.TIMED;
//	public static COMBAT_TYPE type = COMBAT_TYPE.TURNED;
}
