using System.Collections.Generic;

public enum TempoEventType
{
	roll,
	cut,
	put,
	send,
	rollt,
	cutt,
	putt,
	sendt
}

public interface TempoTemplate
{
	double bpm { get; }
	List<KeyValuePair<double, TempoEventType>> events { get; }
}
