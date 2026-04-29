using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public sealed class TutorTimingManager
{
	private readonly double _bpm;
	private readonly double[] _beats;
	private readonly TempoEventType[] _events;
	private readonly Action<TempoEventType>[] _callbacks;
	private readonly double _startTime;
	private readonly SynchronizationContext _context;
	private readonly CancellationTokenSource _cts;
	private int _nextIndex;

	public TutorTimingManager(
		double bpm,
		double[] beats,
		TempoEventType[] events,
		Action<TempoEventType>[] callbacks)
	{
		if (bpm <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(bpm), "BPM must be positive.");
		}

		if (beats == null || events == null || callbacks == null)
		{
			string name = beats == null ? nameof(beats) : events == null ? nameof(events) : nameof(callbacks);
			throw new ArgumentNullException(name);
		}

		if (beats.Length != events.Length || beats.Length != callbacks.Length)
		{
			throw new ArgumentException("beats, events, and callbacks must have the same length.");
		}

		_bpm = bpm;
		_beats = beats;
		_events = events;
		_callbacks = callbacks;
		_startTime = Time.timeAsDouble;
		_context = SynchronizationContext.Current;
		_cts = new CancellationTokenSource();
		_nextIndex = 0;
		_ = RunAsync(_cts.Token);
	}

	public bool IsFinished => _nextIndex >= _beats.Length;

	~TutorTimingManager()
	{
		if (!_cts.IsCancellationRequested)
		{
			_cts.Cancel();
		}
	}

	public void Stop()
	{
		if (!_cts.IsCancellationRequested)
		{
			_cts.Cancel();
		}
	}

	public async Task RunAsync(CancellationToken cancellationToken)
	{
		while (!IsFinished)
		{
			double targetTime = GetBeatTimeSeconds(_beats[_nextIndex]);
			double delaySeconds = targetTime - Time.timeAsDouble;

			if (delaySeconds > 0)
			{
				await Task.Delay(TimeSpan.FromSeconds(delaySeconds), cancellationToken);
			}

			int index = _nextIndex;
			TempoEventType evt = _events[index];
			Action<TempoEventType> callback = _callbacks[index];

			if (_context != null)
			{
				_context.Post(_ => callback?.Invoke(evt), null);
			}
			else
			{
				callback?.Invoke(evt);
			}

			_nextIndex++;
		}
	}

	private double GetBeatTimeSeconds(double beat)
	{
		double secondsPerBeat = 60.0 / _bpm;
		return _startTime + beat * secondsPerBeat;
	}
}
