using System.Collections.Generic;

public class TestTempo : TempoTemplate
{
	public double bpm { get; } = 120;
	public List<TempoBatch> events { get; } = new()
	{
		new TempoBatch(2, TempoEventType.roll).AddEvent(new List<KeyValuePair<double, TempoBatchEventType>>
		{
			new (0.0, TempoBatchEventType.start),
			new (4, TempoBatchEventType.getToolt),
			new (4, TempoBatchEventType.getTool),
			new (8, TempoBatchEventType.tutor),
			new (9, TempoBatchEventType.tutor),
			new (10, TempoBatchEventType.tutor),
			
			new (12, TempoBatchEventType.player_input),
			new (13, TempoBatchEventType.player_input),
			new (14, TempoBatchEventType.player_input),

			new (16, TempoBatchEventType.getOffToolt),
			new (16, TempoBatchEventType.getOffTool),
			new (16, TempoBatchEventType.end)
		})
	};
}