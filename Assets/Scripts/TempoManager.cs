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
	private readonly Action<BeatTimingResult>[] _callbacks;
	private readonly double _toleranceSeconds;
	private readonly double _startTime;
	private int _nextIndex;

	public BeatTimingManager(
		double bpm,
		double[] beats,
		Action<BeatTimingResult>[] callbacks,
		double toleranceSeconds)
	{
		if (bpm <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(bpm), "BPM must be positive.");
		}

		if (beats == null || callbacks == null)
		{
			throw new ArgumentNullException(beats == null ? nameof(beats) : nameof(callbacks));
		}

		if (beats.Length != callbacks.Length)
		{
			throw new ArgumentException("beats and callbacks must have the same length.");
		}

		if (toleranceSeconds < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(toleranceSeconds), "Tolerance must be non-negative.");
		}

		_bpm = bpm;
		_beats = beats;
		_callbacks = callbacks;
		_toleranceSeconds = toleranceSeconds;
		_startTime = Time.timeAsDouble;
		_nextIndex = 0;
	}

	public bool IsFinished => _nextIndex >= _beats.Length;

	public void BeatInput()
	{
		if (IsFinished)
		{
			return;
		}

		double now = Time.timeAsDouble;
		double targetTime = GetBeatTimeSeconds(_beats[_nextIndex]);
		double delta = now - targetTime;

		if (delta < -2 * _toleranceSeconds)
		{
			return;
		}

		if (delta > 2 * _toleranceSeconds)
		{
			throw new InvalidOperationException("BeatInput received too late; use Tick to handle missed beats.");
		}

		BeatTimingResult result;

		if (delta < -_toleranceSeconds)
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

		_callbacks[_nextIndex]?.Invoke(result);
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
			_callbacks[_nextIndex]?.Invoke(BeatTimingResult.TooLate);
			_nextIndex++;
		}
	}

	private double GetBeatTimeSeconds(double beat)
	{
		double secondsPerBeat = 60.0 / _bpm;
		return _startTime + beat * secondsPerBeat;
	}
}
