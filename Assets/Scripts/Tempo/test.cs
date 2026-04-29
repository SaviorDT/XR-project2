using System.Collections.Generic;

public class TestTempo : TempoTemplate
{
	public double bpm { get; } = 120;
	public List<KeyValuePair<double, TempoEventType>> events { get; } = new()
	{
		new(1.0, TempoEventType.cut),
		new(2.0, TempoEventType.cut),
		new(3.0, TempoEventType.send),
	};
}