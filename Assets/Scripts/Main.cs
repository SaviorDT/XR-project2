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
        InitializeAnimationController();
        StartCoroutine(StartGameAfterDelay(10f));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator StartGameAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        StartGame(new TestTempoWithoutAudio());
    }

    private void InitializeAnimationController()
    {
        AnimationController ac = AnimationController.Instance;

        GameObject scorebar = GameObject.Find("分數量表最終型");
        GameObject oneStarEffect = GameObject.Find("Shine_ellow");
        ac.SetScorebar(scorebar, oneStarEffect);
    }
}
