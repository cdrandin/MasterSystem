using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

public class SimpleSerializer
{
	/// <summary>
	/// Save the specified key and data. DATA WILL OVERRIDE EXISTING.
	/// </summary>
	/// <param name="key">Key.</param>
	/// <param name="data">Data.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static void Save<T>(string key, T data) where T: class
	{
		PlayerPrefs.SetString(key, GetSerialized(data));
		PlayerPrefs.Save();
	}

	/// <summary>
	/// Load the specified Key. If key doesn't not exist null is returned.
	/// </summary>
	/// <param name="Key">Key.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static T Load<T>(string Key) where T: class
	{
		if(PlayerPrefs.HasKey(Key))
		{
			return GetObject<T>(PlayerPrefs.GetString(Key));
		}

		return null;
	}

	static string GetSerialized<T>(T data) where T: class
	{
		using (var stream = new MemoryStream())
		{
			var formatter = new BinaryFormatter();
			formatter.Serialize(stream, data);
			stream.Flush();
			stream.Position = 0;
			return Convert.ToBase64String(stream.ToArray());
		}
	}

	static T GetObject<T>(string code) where T: class
	{
		byte[] b = Convert.FromBase64String(code);
		using (var stream = new MemoryStream(b))
		{
			var formatter = new BinaryFormatter();
			stream.Seek(0, SeekOrigin.Begin);
			return (T)formatter.Deserialize(stream);
		}
	}
}