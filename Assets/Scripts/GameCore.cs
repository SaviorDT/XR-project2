using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class GameCore
{
	[SerializeField] private float _toleranceSeconds = 0.1f;
	[SerializeField] private int score = 0, max_score = 100;
	private AudioSource _musicSource;
	private TempoTemplate _tempoTemplate;
	private BeatTimingManager _beatTimingManager;
	private BackendTimingManager _backendTimingManager;
	private CancellationTokenSource _tickCts;

	public GameCore(TempoTemplate tempoTemplate)
	{
		_tempoTemplate = tempoTemplate;
		max_score = _tempoTemplate.maxScore;
	}

	public void Start()
	{
		InitializeTempo();
		PlayMusic();
		HookUserInput();
		StartTickLoop();
		StartBearTeacher();
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
				TempoBatch capturedBatch = tempoBatch;
				backendBeats.Add(beatTime);
				backendCallbacks.Add(() => OnBackendEvent(capturedBatch, eventType));
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

	private void PlayMusic()
	{
		AudioClip clip = _tempoTemplate?.music;
		if (clip == null)
		{
			Debug.LogWarning("TempoTemplate music is null.");
			return;
		}

		const string musicObjectName = "GameMusic";
		GameObject musicObject = GameObject.Find(musicObjectName);
		if (musicObject == null)
		{
			musicObject = new GameObject(musicObjectName);
		}

		_musicSource = musicObject.GetComponent<AudioSource>();
		if (_musicSource == null)
		{
			_musicSource = musicObject.AddComponent<AudioSource>();
		}

		_musicSource.clip = clip;
		_musicSource.Play();
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
		if (result != BeatTimingResult.TooLate)
		{
			bool hasReached1Star = score >= max_score * 0.75;
			double safeDeltaSeconds = Math.Max(Math.Abs(deltaSeconds), _toleranceSeconds / 4.136);
			score += (int)(_toleranceSeconds / safeDeltaSeconds * 1000);
			AnimationController.Instance.UpdateScorebar((double)score / max_score);
			if ((double) score / max_score >= 0.75 && !hasReached1Star) 
			{
				AnimationController.Instance.Play1StarEffect();
			}

			Debug.Log($"Beat '{eventType}': {result} ({deltaSeconds:0.000}s)");
		}
		
		switch (result)
		{
			case BeatTimingResult.OnTime:
				AnimationController.Instance.ShowPerfectInputEffect();
				break;
			case BeatTimingResult.Early:
			case BeatTimingResult.Late:
				AnimationController.Instance.ShowGoodInputEffect();
				break;
			case BeatTimingResult.TooLate:
				AnimationController.Instance.ShowMissInputEffect();
				break;
		}
	}
	private void StartBearTeacher()
	{
		GameObject bearTeacher = GameObject.Find("bear-teacher-hand-trynew02 (3)");
		if (bearTeacher != null)
		{
			Animator animator = bearTeacher.GetComponent<Animator>();
			if (animator == null)
			{
				Debug.LogWarning("bearTeacher is missing an Animator component.");
				return;
			}

			animator.SetTrigger("StartCooking");
		}
		else
		{
			Debug.LogWarning("No GameObject named 'BearTeacher' found in the scene.");
		}
	}

	private void OnBackendEvent(TempoBatch tempoBatch, TempoBatchEventType eventType)
	{
		tempoBatch.OnCallback(eventType);
	}
}
