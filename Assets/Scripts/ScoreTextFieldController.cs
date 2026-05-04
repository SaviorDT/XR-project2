using UnityEngine;
using TMPro;

public class ScoreTextFieldController : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    private int _targetScore;
    private double _durationSeconds;
    private double _elapsedSeconds;
    private bool _isAnimating;

    private void Awake()
    {
        if (_text == null)
        {
            _text = GetComponent<TMP_Text>();
        }
    }
    private void Start()
    {
    }

    public void SetScore(int score, double duration)
    {
        _targetScore = score;
        _durationSeconds = duration;
        _elapsedSeconds = 0d;

        if (_durationSeconds <= 0d)
        {
            _isAnimating = false;
            SetText(_targetScore);
            return;
        }

        _isAnimating = true;
        SetText(0);
    }

    private void Update()
    {
        if (!_isAnimating)
        {
            return;
        }

        _elapsedSeconds += Time.deltaTime;
        if (_elapsedSeconds >= _durationSeconds)
        {
            _isAnimating = false;
            SetText(_targetScore);
            return;
        }

        double progress = _elapsedSeconds / _durationSeconds;
        int currentScore = (int)System.Math.Round(_targetScore * progress);
        SetText(currentScore);
    }

    private void SetText(int score)
    {
        if (_text == null)
        {
            return;
        }

        _text.text = $"score : {score}";
    }
}
