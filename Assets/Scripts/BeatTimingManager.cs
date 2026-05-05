using System;
using UnityEngine;

public enum BeatTimingResult
{
	TooEarly,
	Early,
	OnTime,
	Late,
	TooLate
}

public sealed class BeatTimingManager
{
	private readonly double _bpm;
	private readonly double[] _beats;
	private readonly TempoEventType[] _actions;
	private readonly Action<BeatTimingResult, double>[] _callbacks;
	private readonly double _toleranceSeconds;
	private readonly double _startTime;
	private int _nextIndex;

	public BeatTimingManager(
		double bpm,
		double[] beats,
		TempoEventType[] actions,
		Action<BeatTimingResult, double>[] callbacks,
		double toleranceSeconds)
	{
		if (bpm <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(bpm), "BPM must be positive.");
		}

		if (beats == null || actions == null || callbacks == null)
		{
			string name = beats == null ? nameof(beats) : actions == null ? nameof(actions) : nameof(callbacks);
			throw new ArgumentNullException(name);
		}

		if (beats.Length != actions.Length || beats.Length != callbacks.Length)
		{
			throw new ArgumentException("beats, actions, and callbacks must have the same length.");
		}

		if (toleranceSeconds < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(toleranceSeconds), "Tolerance must be non-negative.");
		}

		_bpm = bpm;
		_beats = beats;
		_actions = actions;
		_callbacks = callbacks;
		_toleranceSeconds = toleranceSeconds;
		_startTime = Time.timeAsDouble;
		_nextIndex = 0;
	}

	public bool IsFinished => _nextIndex >= _beats.Length;

	public void BeatInput(TempoEventType action)
	{
		if (IsFinished)
		{
			return;
		}

		if (action != _actions[_nextIndex])
		{
			return;
		}

		double now = Time.timeAsDouble;
		double targetTime = GetBeatTimeSeconds(_beats[_nextIndex]);
		double delta = now - targetTime;

		if (delta < -3 * _toleranceSeconds)
		{
			return;
		}

		if (delta > 2 * _toleranceSeconds)
		{
			Debug.LogWarning("BeatInput received too late. Ignoring. Delta: " + delta);
			return;
		}

		BeatTimingResult result;

		if (delta < -2 * _toleranceSeconds)
		{
			result = BeatTimingResult.TooEarly;
		}
		else if (delta < -_toleranceSeconds)
		{
			result = BeatTimingResult.Early;
		}
		else if (delta <= _toleranceSeconds)
		{
			result = BeatTimingResult.OnTime;
		}
		else
		{
			result = BeatTimingResult.Late;
		}

		_callbacks[_nextIndex]?.Invoke(result, delta);
		_nextIndex++;
	}

	public void Tick()
	{
		if (IsFinished)
		{
			return;
		}

		double now = Time.timeAsDouble;

		double targetTime = GetBeatTimeSeconds(_beats[_nextIndex]);
		double delta = now - targetTime;

		if (delta > 2 * _toleranceSeconds)
		{
			_callbacks[_nextIndex]?.Invoke(BeatTimingResult.TooLate, delta);
			_nextIndex++;
		}
	}

	private double GetBeatTimeSeconds(double beat)
	{
		double secondsPerBeat = 60.0 / _bpm;
		return _startTime + beat * secondsPerBeat;
	}
}
