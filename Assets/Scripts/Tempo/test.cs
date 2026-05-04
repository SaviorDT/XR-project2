using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TestTempo : TempoTemplate
{
	public double bpm { get; } = 135;
	public AudioClip music { get; } = LoadMusic();
	public int maxScore { get; } = 50000;
	private static TempoBatchEventHandler _eventHandler = new();
	public List<TempoBatch> events { get; } = new()
	{
		new TempoBatch(2, TempoEventType.roll, _eventHandler, GetRoller(), GetDough())
		.AddEvent(new List<KeyValuePair<double, TempoBatchEventType>>
		{
			new (0.0, TempoBatchEventType.start),
			new (2, TempoBatchEventType.getToolt),
			new (2, TempoBatchEventType.getTool),

			new (8, TempoBatchEventType.tutor),
			new (12, TempoBatchEventType.tutor),
			new (16, TempoBatchEventType.getOffToolt),
			
			new (16, TempoBatchEventType.player_input),
			new (20, TempoBatchEventType.player_input),

			new (24, TempoBatchEventType.getOffTool),
			new (24, TempoBatchEventType.end)
		}),

		new TempoBatch(6, TempoEventType.cut, _eventHandler, GetKnife(), GetCucumber())
		.AddEvent(new List<KeyValuePair<double, TempoBatchEventType>>
		{
			new (24, TempoBatchEventType.start),

			new (26, TempoBatchEventType.getTool),
			
			new (32, TempoBatchEventType.player_input),
			new (33, TempoBatchEventType.player_input),
			new (34, TempoBatchEventType.player_input),
			new (35, TempoBatchEventType.player_input),
			new (36, TempoBatchEventType.player_input),
			new (37, TempoBatchEventType.player_input),

			new (38, TempoBatchEventType.getOffTool),
			new (38, TempoBatchEventType.end)
		}),

		new TempoBatch(9, TempoEventType.put, _eventHandler, GetBowl(), GetPizza())
		.AddEvent(new List<KeyValuePair<double, TempoBatchEventType>>
		{
			new (40, TempoBatchEventType.start),

			new (42, TempoBatchEventType.getTool),
			
			new (50, TempoBatchEventType.player_input),
			new (51, TempoBatchEventType.player_input),
			new (52, TempoBatchEventType.player_input),
			new (53, TempoBatchEventType.player_input),
			new (54, TempoBatchEventType.player_input),

			new (66, TempoBatchEventType.player_input),
			new (68, TempoBatchEventType.player_input),
			new (69, TempoBatchEventType.player_input),
			new (70, TempoBatchEventType.player_input),

			new (72, TempoBatchEventType.getOffTool),
			new (72, TempoBatchEventType.end)
		}),

		new TempoBatch(1, TempoEventType.send, _eventHandler, GetPizza())
		.AddEvent(new List<KeyValuePair<double, TempoBatchEventType>>
		{
			new (72, TempoBatchEventType.start),

			new (74, TempoBatchEventType.getTool),
			
			new (78, TempoBatchEventType.player_input),

			new (80, TempoBatchEventType.getOffTool),
			new (80, TempoBatchEventType.end)
		})
	};

	private static GameObject GetRoller() => GameObject.Find("roller");
	private static GameObject GetDough() => GameObject.Find("dough (5)");
	private static GameObject GetKnife() => GameObject.Find("kitchenknife");
	private static GameObject GetCucumber() => GameObject.Find("cucumber_Shaved_Half7777777704");
	private static GameObject GetBowl() => null;
	private static GameObject GetPizza() => GameObject.Find("Pizzaaa (4)");

	private static AudioClip LoadMusic()
	{
#if UNITY_EDITOR
		return AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/Margarito.wav");
#else
		return null;
#endif
	}


	private class TempoBatchEventHandler : TempoBatchEventCallback
	{
		private int cut_count = 0, put_count = 0;
		private TempoEventType _currentAction;
		public void OnStart(TempoEventType tempoEventType)
		{
			_currentAction = tempoEventType;
			Debug.Log($"事件開始！類型：{tempoEventType}");
		}
		public void OnGetToolt() => Debug.Log("拿出工具提示！");
		public void OnGetOffToolt() => Debug.Log("收回工具提示！");
		public void OnGetTool()
		{
			switch (_currentAction)
			{
				case TempoEventType.roll:
                    GetRoller().GetComponent<AutoSnapToHand>()?.SnapToHand();
					AnimationController.Instance.ShowDough(GetDough());
					break;
				case TempoEventType.cut:
					GetKnife().GetComponent<AutoSnapToHand>()?.SnapToHand();
					AnimationController.Instance.ShowCucumber(GetCucumber());
					break;
				case TempoEventType.put:
					Debug.Log("拿出盤子提示");
					AnimationController.Instance.ShowPizza(GetPizza());
					break;
				case TempoEventType.send:
					Debug.Log("準備送出提示");
					break;
			}
		}
		public void OnGetOffTool()
		{
			switch (_currentAction)
			{
				case TempoEventType.roll:
					GetRoller().GetComponent<AutoSnapToHand>()?.DetachFromHand();
					AnimationController.Instance.HideDough(GetDough());
					break;
				case TempoEventType.cut:
					GetKnife().GetComponent<AutoSnapToHand>()?.DetachFromHand();
					AnimationController.Instance.HideCucumber(GetCucumber());
					break;
				case TempoEventType.put:
					Debug.Log("收回盤子提示！");
					break;
				case TempoEventType.send:
					Debug.Log("送出提示！");
					break;
			}
		}
		public void OnTutorEvent() => Debug.Log("教學事件！");
		public void OnPlayerInput()
		{
			switch (_currentAction)
			{
				case TempoEventType.roll:
					Debug.Log("玩家對 Roll 的輸入！");
					break;
				case TempoEventType.cut:
					AnimationController.Instance.SetCucumberSlices(GetCucumber(), ++cut_count);
					break;
				case TempoEventType.put:
					AnimationController.Instance.SetPizzaPieces(GetPizza(), ++put_count);
					break;
				case TempoEventType.send:
					AnimationController.Instance.SendPizza(GetPizza());
					break;
			}
		}
		public void OnEnd() => Debug.Log("事件結束！");

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
	}
}