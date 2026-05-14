using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("分數柱設定")]
    public RectTransform scoreBar;
    // 根據你的描述：
    // 最左邊 (0分) = -0.12f
    // 最右邊 (滿分) = -1.258f
    private float startX = -0.12f;
    private float endX = -1.258f;

    [Header("數值設定")]
    public int currentScore = 4; // 初始 basic 分 (8 分之 4)
    public int maxScore = 8;

    [Header("特效與觸發物件")]
    public GameObject starEffect;
    public GameObject audienceGroup;

    // 新增：提供外部檢查是否滿分
    public bool IsFullScore => currentScore >= maxScore;

    void Start()
    {
        if (starEffect != null) starEffect.SetActive(false);
        if (audienceGroup != null) audienceGroup.SetActive(false);
        UpdateScoreBar();
    }

    public void AddScore(int amount)
    {
        currentScore = Mathf.Clamp(currentScore + amount, 0, maxScore);
        UpdateScoreBar();
        CheckMilestones();
    }

    void UpdateScoreBar()
    {
        if (scoreBar != null)
        {
            float scorePercentage = (float)currentScore / maxScore;

            // 使用 Lerp 自動計算從 -0.12 往 -1.258 移動的過程
            float targetX = Mathf.Lerp(startX, endX, scorePercentage);

            Vector3 pos = scoreBar.localPosition;
            pos.x = targetX;
            scoreBar.localPosition = pos;
        }
    }

    void CheckMilestones()
    {
        // 到達 3/4 (6分) 顯示星星
        if (currentScore >= 6 && starEffect != null)
        {
            if (!starEffect.activeSelf) starEffect.SetActive(true);
        }
        else if (currentScore < 6 && starEffect != null)
        {
            // 如果被扣分掉回 6 分以下，可以考慮關閉特效
            starEffect.SetActive(false);
        }
    }

    // 這個 Function 我們現在交給 TableTrigger 來精準控制
    public void ShowAudience()
    {
        if (audienceGroup != null)
        {
            audienceGroup.SetActive(true);
            Debug.Log("太棒了！觀眾出現為熊老師喝采！");
        }
    }
}