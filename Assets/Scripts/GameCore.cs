using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class GameCore
{
	[SerializeField] private float _toleranceSeconds = 0.1f;
	[SerializeField] private int score = 0;
	private TempoTemplate _tempoTemplate;
	private BeatTimingManager _beatTimingManager;
	private BackendTimingManager _backendTimingManager;
	private CancellationTokenSource _tickCts;

	public GameCore(TempoTemplate tempoTemplate)
	{
		_tempoTemplate = tempoTemplate;
	}

	public void Start()
	{
		InitializeTempo();
		HookUserInput();
		StartTickLoop();
	}

	private void InitializeTempo()
	{
		
		if (_tempoTemplate == null)
		{
			Debug.LogError("GameCore needs a TempoTemplate. Call Initialize before Start.");
			return;
		}

		var mainBeats = new System.Collections.Generic.List<double>();
		var mainActions = new System.Collections.Generic.List<TempoEventType>();
		var mainCallbacks = new System.Collections.Generic.List<Action<BeatTimingResult, double>>();
		var backendBeats = new System.Collections.Generic.List<double>();
		var backendCallbacks = new System.Collections.Generic.List<Action>();

		foreach (var tempoBatch in _tempoTemplate.events)
		{
			if (tempoBatch?.events == null)
			{
				continue;
			}

			foreach (var tempoEvent in tempoBatch.events)
			{
				double beatTime = tempoEvent.Key;
				TempoBatchEventType eventType = tempoEvent.Value;

				if (eventType == TempoBatchEventType.player_input)
				{
					TempoEventType capturedAction = tempoBatch.tempoEventType;
					mainBeats.Add(beatTime);
					mainActions.Add(capturedAction);
					mainCallbacks.Add((result, delta) => OnBeatResult(capturedAction, result, delta));
				}
				else
				{
					TempoBatch capturedBatch = tempoBatch;
					backendBeats.Add(beatTime);
					backendCallbacks.Add(() => OnBackendEvent(capturedBatch, eventType));
				}
			}
		}

		_beatTimingManager = new BeatTimingManager(
			_tempoTemplate.bpm,
			mainBeats.ToArray(),
			mainActions.ToArray(),
			mainCallbacks.ToArray(),
			_toleranceSeconds);

		_backendTimingManager = new BackendTimingManager(
			_tempoTemplate.bpm,
			backendBeats.ToArray(),
			backendCallbacks.ToArray());
	}

    private void HookUserInput()
	{
		DrumStickSwingDetector cutDetector = FindSingleDetector<DrumStickSwingDetector>("DrumStickSwingDetector");
		if (cutDetector == null)
		{
			Debug.LogWarning("No DrumStickSwingDetector found in the scene.");
		}
		else
		{
			cutDetector.SetCallback(() => OnInput(TempoEventType.cut));
		}

		PizzaThrowDetector pizzaThrowDetector = FindSingleDetector<PizzaThrowDetector>("PizzaThrowDetector");
		if (pizzaThrowDetector == null)
		{
			Debug.LogWarning("No PizzaThrowDetector found in the scene.");
		}
		else
		{
			pizzaThrowDetector.SetCallback(() => OnInput(TempoEventType.send));
		}

		RollingPinDetector rollingPinDetector = FindSingleDetector<RollingPinDetector>("RollingPinDetector");
		if (rollingPinDetector == null)
		{
			Debug.LogWarning("No RollingPinDetector found in the scene.");
		}
		else
		{
			rollingPinDetector.SetCallback(() => OnInput(TempoEventType.roll));
		}

		SprinkleDetector sprinkleDetector = FindSingleDetector<SprinkleDetector>("SprinkleDetector");
		if (sprinkleDetector == null)
		{
			Debug.LogWarning("No SprinkleDetector found in the scene.");
		}
		else
		{
			sprinkleDetector.SetCallback(() => OnInput(TempoEventType.put));
		}
	}

	private static T FindSingleDetector<T>(string detectorName) where T : UnityEngine.Object
	{
		T[] detectors = UnityEngine.Object.FindObjectsByType<T>(FindObjectsSortMode.None);
		if (detectors == null || detectors.Length == 0)
		{
			return null;
		}

		if (detectors.Length > 1)
		{
			Debug.LogWarning($"Multiple {detectorName} found in the scene. Using the first one.");
		}

		return detectors[0];
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
		_backendTimingManager?.Stop();
	}

	public void OnInput(TempoEventType action)
	{
		_beatTimingManager?.BeatInput(action);
	}

	private void OnBeatResult(TempoEventType eventType, BeatTimingResult result, double deltaSeconds)
	{
		double safeDeltaSeconds = Math.Max(Math.Abs(deltaSeconds), _toleranceSeconds / 4.136);
		score += (int)(_toleranceSeconds / safeDeltaSeconds * 1000);
		Debug.Log($"Beat '{eventType}': {result} ({deltaSeconds:0.000}s)");
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

	private void OnBackendEvent(TempoBatch tempoBatch, TempoBatchEventType eventType)
	{
		tempoBatch?.OnCallback(eventType);
		Debug.Log($"Backend event: {eventType}");
	}
}
