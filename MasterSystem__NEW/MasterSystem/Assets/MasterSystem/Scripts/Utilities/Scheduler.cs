using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Timers;

public class Scheduler : MonoBehaviour
{
	private float _time_interval; // how time will progress

	#region TimeCallBack class
	// Event info, which provides the action to perform and what time the event should happen
	public class TimeCallBack
	{
		private Vector2 m_scheduled_event_time;
		private System.Action  m_scheduled_action;
		private bool    m_looping;
		private bool    m_triggered;
		private string  m_event_name;
		
		public bool m_event_active
		{
			get { return m_triggered; }
		}
		
		public string description
		{
			get { return string.Format("Event: {0}  Time: {1}  Looping:{2}", m_event_name, m_scheduled_event_time, m_looping); }
		}
		
		public TimeCallBack(string name, System.Action action, bool looping)
		{
			m_event_name       = name;
			m_scheduled_action = action;
			m_looping          = looping;
		}
		
		public void SetEventTime(float time)
		{
			m_scheduled_event_time = new Vector2(time, float.NegativeInfinity);
		}
		
		public void SetEventTimeSpan(Vector2 timespan)
		{
			m_scheduled_event_time = timespan;
		}
		
		public void SetLooping(bool looping)
		{
			m_looping = looping;
		}
		
		// Check if event should run yet given the time span
		public bool Query(Vector2 previous_future_timespan)
		{
			float start = m_scheduled_event_time.x;
			bool trash  = false;
			
			// Is it time to run event?
			if(start >= previous_future_timespan.x && start <= previous_future_timespan.y)
			{
				m_scheduled_action();
				m_triggered = true;
				trash       = !m_looping; // should it be trashed?
			}
			
			return trash;
		}
		
		/// <summary>
		/// Queries to current time with a difference of the _time_interval
		/// </summary>
		/// <returns><c>true</c>, if to current time was queryed, <c>false</c> otherwise.</returns>
		public bool QueryToCurrentTime()
		{
			float start = m_scheduled_event_time.x;
			bool trash  = false;

			// Check if it is time to run
			if(Time.time >= start)
			{
				m_scheduled_action();
				m_triggered = true;
				trash       = !m_looping;
			}
			
			return trash;
		}
		
		public void PostponeEvent(float delay)
		{
			m_triggered               = false;
			m_scheduled_event_time.x += delay;
			m_scheduled_event_time.y += delay;
		}
	}
	#endregion

	private Dictionary<int, TimeCallBack> m_events_overview;

	/// <summary>
	/// Schedules an event given a Vector 2 time span value. Make sure to save the key to unschedule it.
	/// Target will be the object wishing to schedule an event.
	/// Looping will signify if you want this action to continue happening every 24 hours, according to the last time span entered.
	/// </summary>
	/// <returns>The event key.</returns>
	/// <param name="target">Target.</param>
	/// <param name="event_name">Event_name.</param>
	/// <param name="timespan">Timespan.</param>
	/// <param name="action">Action.</param>
	/// <param name="looping">If set to <c>true</c> looping.</param>
	public int ScheduleEventTimespan(GameObject target, string event_name, Vector2 timespan, System.Action action, bool looping = true)
	{
		TimeCallBack tcb = new TimeCallBack(event_name, action, looping);
		tcb.SetEventTimeSpan(timespan);
		
		int key = (target.GetInstanceID().ToString() + event_name).GetHashCode();
		
		if(m_events_overview.ContainsKey(key))
		{
			Debug.LogError(string.Format("Something is wrong. The key: {0} was already used. It isn't unique, change method", key));
			Debug.LogError(string.Format("Event: {0} not scheduled", event_name));
		}
		else
		{
			m_events_overview.Add(key, tcb);
		}
		
		return key;
	}

	/// <summary>
	/// Schedules an event given a time value. Make sure to save the key to unschedule it.
	/// Target will be the object wishing to schedule an event.
	/// Looping will signify if you want this action to continue happening every 24 hours, according to the last time entered.
	/// </summary>
	/// <returns>The event key.</returns>
	/// <param name="target">Target.</param>
	/// <param name="event_name">Event_name.</param>
	/// <param name="timespan">Timespan.</param>
	/// <param name="action">Action.</param>
	/// <param name="looping">If set to <c>true</c> looping.</param>
	public int ScheduleEventTime(GameObject target, string event_name, float time, System.Action action, bool looping = true)
	{
		TimeCallBack tcb = new TimeCallBack(event_name, action, looping);
		tcb.SetEventTime(time);

		int key = (GetInstanceID().ToString() + event_name).GetHashCode();
		
		if(m_events_overview.ContainsKey(key))
		{
			Debug.LogError(string.Format("Something is wrong. The key: {0} was already used. It isn't unique, change method", key));
			Debug.LogError(string.Format("Event: {0} not scheduled", event_name));
		}
		else
		{
			m_events_overview.Add(key, tcb);
		}
		
		return key;
	}

	/// <summary>
	/// Unschedules the event..
	/// </summary>
	/// <returns><c>true</c>, if event was unscheduled, <c>false</c> otherwise.</returns>
	/// <param name="key">Key.</param>
	public bool UnscheduleEvent(int key)
	{
		return m_events_overview.Remove(key);
	}

	// Determine if scheduled events should run yet
	void HandleEvents()
	{
		// Cache keys, so there are no syncing errors with the dictionary
		List<int> keys = new List<int>(m_events_overview.Keys);
		
		foreach(int key in keys)
		{
			// Run event, if it is time. Determine if it should be trashed
			//bool trash = m_events_overview[key].Query(new Vector2(Time.time, Time.time + 5.0f));
			bool discard = m_events_overview[key].QueryToCurrentTime();
			
			//Debug.Log(trash);
			
			// Trash item, event running
			if(discard)
			{
				UnscheduleEvent(key);
			}
			// Keep item
			else
			{
				// If event has been triggered, don't do it again
				if(m_events_overview[key].m_event_active)
				{
					m_events_overview[key].PostponeEvent(5.0f);
					Debug.Log("Starting again later!");
				}
			}
		}
	}

	private static Scheduler _instance;
	private static GameObject _instance_obj;
	public static Scheduler instance 
	{
		get
		{
			if(_instance == null)
			{
				_instance = FindObjectOfType<Scheduler>();
				if(instance == null)
				{
					_instance_obj = new GameObject("Scheduler");
					_instance      = _instance_obj.AddComponent<Scheduler>();
				}
			}
			return _instance;
		}
	}

	void Awake()
	{
		m_events_overview = new Dictionary<int, TimeCallBack>();
	}

	void Start()
	{
		_time_interval = 0.25f;

		InvokeRepeating("HandleEvents", 0.0f, _time_interval);
	}
}