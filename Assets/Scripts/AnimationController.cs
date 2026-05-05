using System;
using UnityEngine;

public class AnimationController
{
    private static readonly AnimationController _instance = new();
    private GameObject _scorebar, _1StarEffect;

    public event Action PerfectInputEffectRequested;
    public event Action GoodInputEffectRequested;
    public event Action MissInputEffectRequested;

    public static AnimationController Instance => _instance;

    private AnimationController()
    {
    }

    public AnimationController SetScorebar(GameObject scorebar, GameObject oneStarEffect)
    {
        _scorebar = scorebar;
        _1StarEffect = oneStarEffect;
        return this;
    }

    public AnimationController SetRoll(GameObject roller, GameObject dough)
    {
        // Implementation for setting roll-related objects
        return this;
    }

    public void UpdateScorebar(double score)
    {
        if (_scorebar == null)
        {
            Debug.LogWarning("Scorebar is not set.");
            return;
        }

        Transform cubeTransform = _scorebar.transform.Find("Cube");
        if (cubeTransform == null)
        {
            Debug.LogWarning("Scorebar is missing child named 'Cube'.");
            return;
        }

        Vector3 localPosition = cubeTransform.localPosition;
        localPosition.x = (float)-score;
        cubeTransform.localPosition = localPosition;
    }
    public void ShowPerfectInputEffect()
    {
        PerfectInputEffectRequested?.Invoke();
    }
    public void ShowGoodInputEffect()
    {
        GoodInputEffectRequested?.Invoke();
    }
    public void ShowMissInputEffect()
    {
        MissInputEffectRequested?.Invoke();
    }
    public void Play1StarEffect() 
    {
        if (_1StarEffect != null) _1StarEffect.GetComponent<ParticleSystem>().Play();
    }
    public void ShowDough(GameObject dough)
    {
        if (dough == null) return;

        SimpleController mover = dough.GetComponent<SimpleController>();
        if (mover == null) mover = dough.AddComponent<SimpleController>();

        mover.MoveTo(new Vector3(-25.0760002f, -6.26548004f, 28.8579998f), 0.5f);
    }
    public void HideDough(GameObject dough)
    {
        if (dough == null) return;

        SimpleController mover = dough.GetComponent<SimpleController>();
        if (mover == null) mover = dough.AddComponent<SimpleController>();

        mover.MoveTo(new Vector3(-25.0760002f, -6.26548004f, 30.9060001f), 0.5f);
    }

    public void ShowCucumber(GameObject cucumber)
    {
        if (cucumber == null) return;
        for (int i = 0; i < 7; i++)
        {
            cucumber.transform.GetChild(i).gameObject.SetActive(false);
        }

        SimpleController mover = cucumber.GetComponent<SimpleController>();
        if (mover == null) mover = cucumber.AddComponent<SimpleController>();

        mover.MoveTo(new Vector3(-29.3659992f, 0.875f, 25.4750004f), 0.5f);
    }
    public void HideCucumber(GameObject cucumber)
    {
        if (cucumber == null) return;

        SimpleController mover = cucumber.GetComponent<SimpleController>();
        if (mover == null) mover = cucumber.AddComponent<SimpleController>();

        mover.MoveTo(new Vector3(-29.1170006f, 0.894999981f, 25.4689999f), 0.5f);
    }
    public void SetCucumberSlices(GameObject cucumber, int sliceCount)
    {
        if (cucumber == null) return;

        for (int i = 0; i <= sliceCount; i++)
        {
            cucumber.transform.GetChild(i).gameObject.SetActive(true);
        }
    }
    public void ShowPizza(GameObject pizza)
    {
        if (pizza == null) return;

        for (int i = 0; i < 9; i++)
        {
            pizza.transform.GetChild(i).gameObject.SetActive(false);
        }

        SimpleController mover = pizza.GetComponent<SimpleController>();
        if (mover == null) mover = pizza.AddComponent<SimpleController>();

        mover.MoveTo(new Vector3(-29.3419991f, 0.833000004f, 25.4946918f), 0.5f);
    }
    public void SetPizzaPieces(GameObject pizza, int pieceCount)
    {
        if (pizza == null) return;

        for (int i = 0; i < pieceCount; i++)
        {
            pizza.transform.GetChild(i).gameObject.SetActive(true);
        }
    }
    public void SendPizza(GameObject pizza)
    {
        if (pizza == null) return;

        SimpleController mover = pizza.GetComponent<SimpleController>();
        if (mover == null) mover = pizza.AddComponent<SimpleController>();

        mover.MoveTo(new Vector3(-28.2313995f, 1.38800001f, 28.6140003f), 0.8f);
        mover.RotateTo(new Vector3(0f, 1080f, 0f), 0.8f);

        mover.MoveTo(new Vector3(-28.2313995f, 1.38800001f, 28.9249992f), 0.5f, SimpleController.CommandOption.Queue);
        mover.RotateTo(new Vector3(0f, 1440f, 0f), 0.5f, SimpleController.CommandOption.Queue);
    }
    public void SetFinalScore(GameObject scoreTextField, GameObject hat, int score, double scorePercentage)
    {
        double maxHatScale = 3.5;
        double hatScale = scorePercentage * maxHatScale;

        scoreTextField.GetComponent<ScoreTextFieldController>().SetScore(score, 3.5);
        hat.GetComponent<HatScaleController>().SetScale(hatScale, 3.5);


    }
}