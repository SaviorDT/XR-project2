using System;
using System.Collections.Generic;
using UnityEngine;

public enum TempoEventType
{
	roll,
	cut,
	put,
	send
}

public enum TempoBatchEventType
{
	start,
	getToolt,
	getOffToolt,
	getTool,
	getOffTool,
	tutor,
	player_input,
	end
}

public interface TempoBatchEventCallback
{
	public void OnStart(TempoEventType tempoEventType);
	public void OnEnd();
	public void OnGetToolt();
	public void OnGetOffToolt();
	public void OnGetTool();
	public void OnGetOffTool();
	public void OnTutorEvent();
	public void OnPlayerInput();
}

public class TempoBatch
{
	public GameObject tool, target;
	public TempoEventType tempoEventType;
	public TempoBatchEventCallback callback;
	public int repeatCount = 0;
	public List<KeyValuePair<double, TempoBatchEventType>> events;

	public TempoBatch(int repeatCount, TempoEventType tempoEventType, TempoBatchEventCallback callback, GameObject tool = null, GameObject target = null)
	{
		this.repeatCount = repeatCount;
		this.tempoEventType = tempoEventType;
		this.callback = callback;
		this.tool = tool;
		this.target = target;
	}

	public TempoBatch AddEvent(List<KeyValuePair<double, TempoBatchEventType>> events)
	{
		this.events = events;
		return this;
	}
	public void OnCallback(TempoBatchEventType eventType)
	{
		switch (eventType)
		{
			case TempoBatchEventType.start:
				callback.OnStart(tempoEventType);
				break;
			case TempoBatchEventType.getToolt:
				callback.OnGetToolt();
				break;
			case TempoBatchEventType.getOffToolt:
				callback.OnGetOffToolt();
				break;
			case TempoBatchEventType.getTool:
				callback.OnGetTool();
				break;
			case TempoBatchEventType.getOffTool:
				callback.OnGetOffTool();
				break;
			case TempoBatchEventType.tutor:
				callback.OnTutorEvent();
				break;
			case TempoBatchEventType.player_input:
				callback.OnPlayerInput();
				break;
			case TempoBatchEventType.end:
				callback.OnEnd();
				break;
		}
	}
}

public interface TempoTemplate
{
	double bpm { get; }
	int maxScore { get; }
	AudioClip music { get; }
	
	List<TempoBatch> events { get; }
	void SetOnEndCallback(Action callback);
}
