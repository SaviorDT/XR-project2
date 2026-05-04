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

public class TempoBatch
{
	public GameObject tool, target;
	public TempoEventType tempoEventType;
	public int repeatCount = 0, counter = 0, countert = 0;
	public List<KeyValuePair<double, TempoBatchEventType>> events;

	public TempoBatch(int repeatCount, TempoEventType tempoEventType, GameObject tool = null, GameObject target = null)
	{
		this.repeatCount = repeatCount;
		this.tempoEventType = tempoEventType;
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
				OnStart();
				break;
			case TempoBatchEventType.getToolt:
				OnGetToolt();
				break;
			case TempoBatchEventType.getOffToolt:
				OnGetOffToolt();
				break;
			case TempoBatchEventType.getTool:
				OnGetTool();
				break;
			case TempoBatchEventType.getOffTool:
				OnGetOffTool();
				break;
			case TempoBatchEventType.tutor:
				OnTutorEvent();
				break;
			case TempoBatchEventType.player_input:
				OnPlayerInput();
				break;
			case TempoBatchEventType.end:
				OnEnd();
				break;
		}
	}

	private void OnStart() {Debug.Log("Start event triggered");}
	private void OnEnd() {Debug.Log("End event triggered");}
	private void OnGetToolt() {Debug.Log("GetToolt event triggered");}
	private void OnGetOffToolt() {Debug.Log("GetOffToolt event triggered");}
	private void OnGetTool() {Debug.Log("GetTool event triggered");}
	private void OnGetOffTool() {Debug.Log("GetOffTool event triggered");}
	private void OnTutorEvent() {Debug.Log("Tutor event triggered");}
	private void OnPlayerInput() {Debug.Log("Player input event triggered");}
}

public interface TempoTemplate
{
	double bpm { get; }
	List<TempoBatch> events { get; }
}
