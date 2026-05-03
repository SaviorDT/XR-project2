using System.Collections;
using UnityEngine;

public class Main : MonoBehaviour
{
    private GameCore _gameCore;
    public void StartGame(TempoTemplate template)
    {
        if (template == null)
        {
            Debug.LogError("Cannot start game: TempoTemplate is null.");
            return;
        }
        if (_gameCore != null)
        {
            Debug.LogWarning("Game is already running. Restarting with new template.");
        }
        _gameCore = new GameCore(template);
        _gameCore.Start();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(StartCountdown());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator StartCountdown()
    {
        const int countdownSeconds = 10;
        for (int remaining = countdownSeconds; remaining > 0; remaining--)
        {
            Debug.Log($"Game starts in {remaining}...");
            yield return new WaitForSeconds(1f);
        }

        StartGame(new TestTempo());
    }
}
