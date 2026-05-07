using UnityEngine;
using TMPro;

public class RecordScoreController : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    private void Awake()
    {
        if (_text == null)
        {
            _text = GetComponent<TMP_Text>();
        }
    }
    void OnEnable()
    {
        GameCore.OnRecordScoreUpdated += UpdateRecordScore;
    }
    void OnDisable()
    {
        GameCore.OnRecordScoreUpdated -= UpdateRecordScore;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void UpdateRecordScore(int score)
    {
        Debug.Log($"RecordScoreController received score update: {score}");
        if (_text == null)
        {
            return;
        }

        _text.text = $"Record: {score}";
    }
}
