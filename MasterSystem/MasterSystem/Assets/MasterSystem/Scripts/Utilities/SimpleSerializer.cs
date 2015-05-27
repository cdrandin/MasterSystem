using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

public class GetCreatePair <T> where T: class
{
	public T obj;
	public bool created;

	public GetCreatePair(T obj, bool v)
	{
		this.obj = obj;
		this.created = v;
	}
}

public class SimpleSerializer
{
	// http://answers.unity3d.com/questions/19649/unity-doesnt-like-my-where-constraints-in-my-c-scr.html#answer-19658
	/// <summary>
	/// Gets or creates the object when checking against the system.
	/// </summary>
	/// <returns>The object to the corresponding key or a new one if one did not exist.</returns>
	/// <param name="key">Key.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static T GetOrCreate<T>(string key) where T: class, new()
	{
		T o = SimpleSerializer.Load<T>(key);
		if(o == null)
		{
			o = new T();
			SimpleSerializer.Save<T>(key, o);
		}
		return o;
	}

	/// <summary>
	/// Gets or creates the object when checking against the system. 
	/// The returned object includes a boolean if the object was newly created or not.
	/// </summary>
	/// <returns>A KetValuePair containing the object to the corresponding key or a new one if one did not exist and if the object was newly created or not.</returns>
	/// <param name="key">Key.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static GetCreatePair<T> GetOrCreateWithStatus<T>(string key) where T: class, new()
	{
		T o = SimpleSerializer.Load<T>(key);
		bool new_obj = false;
		if(o == null)
		{
			o = new T();
			SimpleSerializer.Save<T>(key, o);
			new_obj = true;
		}

		return new GetCreatePair<T>(o, new_obj);
	}

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