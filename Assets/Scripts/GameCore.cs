using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class GameCore : MonoBehaviour
{
	[SerializeField] private float _toleranceSeconds = 0.1f;
	private TempoTemplate _tempoTemplate;
	private BeatTimingManager _beatTimingManager;
	private TutorTimingManager _tutorTimingManager;
	private CancellationTokenSource _tickCts;

	public void Initialize(TempoTemplate tempoTemplate)
	{
		_tempoTemplate = tempoTemplate;
	}

	private void Start()
	{
		if (_tempoTemplate == null)
		{
			Debug.LogError("GameCore needs a TempoTemplate. Call Initialize before Start.");
			return;
		}

		var mainBeats = new System.Collections.Generic.List<double>();
		var mainActions = new System.Collections.Generic.List<TempoEventType>();
		var mainCallbacks = new System.Collections.Generic.List<Action<BeatTimingResult>>();
		var tutorBeats = new System.Collections.Generic.List<double>();
		var tutorEvents = new System.Collections.Generic.List<TempoEventType>();
		var tutorCallbacks = new System.Collections.Generic.List<Action<TempoEventType>>();

		foreach (var tempoEvent in _tempoTemplate.events)
		{
			double beatTime = tempoEvent.Key;
			TempoEventType eventType = tempoEvent.Value;

			if (eventType == TempoEventType.roll || eventType == TempoEventType.cut || eventType == TempoEventType.put || eventType == TempoEventType.send)
			{
				mainBeats.Add(beatTime);
				mainActions.Add(eventType);
				TempoEventType capturedType = eventType;
				mainCallbacks.Add(result => OnBeatResult(capturedType, result));
			}
			else
			{
				tutorBeats.Add(beatTime);
				tutorEvents.Add(eventType);
				tutorCallbacks.Add(evt => OnTutorEvent(evt));
			}
		}

		_beatTimingManager = new BeatTimingManager(
			_tempoTemplate.bpm,
			mainBeats.ToArray(),
			mainActions.ToArray(),
			mainCallbacks.ToArray(),
			_toleranceSeconds);

		_tutorTimingManager = new TutorTimingManager(
			_tempoTemplate.bpm,
			tutorBeats.ToArray(),
			tutorEvents.ToArray(),
			tutorCallbacks.ToArray());

		StartTickLoop();
	}

	private void StartTickLoop()
	{
		_tickCts?.Cancel();
		_tickCts = new CancellationTokenSource();
		_ = TickLoopAsync(_tickCts.Token);
	}

	private async Task TickLoopAsync(CancellationToken cancellationToken)
	{
		while (!cancellationToken.IsCancellationRequested)
		{
			_beatTimingManager?.Tick();
			await Task.Delay(10, cancellationToken);
		}
	}

    ~GameCore()
    {
        OnDestroy();
    }

	private void OnDestroy()
	{
		_tickCts?.Cancel();
		_tutorTimingManager?.Stop();
	}

	public void OnInput(TempoEventType action)
	{
		_beatTimingManager?.BeatInput(action);
	}

	private void OnBeatResult(TempoEventType eventType, BeatTimingResult result)
	{
		Debug.Log($"Beat '{eventType}': {result}");
	}

	private void OnRoll()
	{
		Debug.Log("TempoEventType: roll");
	}

	private void OnCut()
	{
		Debug.Log("TempoEventType: cut");
	}

	private void OnPut()
	{
		Debug.Log("TempoEventType: put");
	}

	private void OnSend()
	{
		Debug.Log("TempoEventType: send");
	}

	private void OnRollt()
	{
		Debug.Log("TempoEventType: rollt");
	}

	private void OnCutt()
	{
		Debug.Log("TempoEventType: cutt");
	}

	private void OnPutt()
	{
		Debug.Log("TempoEventType: putt");
	}

	private void OnSendt()
	{
		Debug.Log("TempoEventType: sendt");
	}

	private void OnTutorEvent(TempoEventType eventType)
	{
		Debug.Log($"Tutor event: {eventType}");
	}
}
