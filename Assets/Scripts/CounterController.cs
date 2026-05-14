using UnityEngine;

public class CounterController_Final : MonoBehaviour
{
    [Header("1. 切菜特效與絲瓜")]
    // 使用陣列，你可以在 Inspector 設定 Size 為 2，然後把兩個特效拖進去
    public ParticleSystem[] chopEffects;
    public GameObject luffaObject;

    [Header("2. 熊老師動畫")]
    public Animator bearAnimator;
    public string chopBoolName = "IsChopping";

    [Header("3. 音效設定")]
    public AudioSource chopAudio;

    private bool hasTriggeredThisTime = false;

    [Header("4. 分數設定 (新加入)")]
    public GameManager gameManager;  // 拖入場景中的 GameManager 物件
    private bool hasAddedScore = false; // 確保每次進入只加一次分

    private void Start()
    {
        // 初始隱藏絲瓜
        if (luffaObject != null) luffaObject.SetActive(false);

        // 初始停止所有特效
        foreach (ParticleSystem ps in chopEffects)
        {
            if (ps != null) ps.Stop();
        }

        if (chopAudio != null) chopAudio.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartChopping();

            // --- 加分邏輯開始 ---
            if (gameManager != null && !hasAddedScore)
            {
                gameManager.AddScore(1);
                hasAddedScore = true; // 標記為已加分
                Debug.Log("火爐加分成功！");
            }
            // --- 加分邏輯結束 ---

            if (!hasTriggeredThisTime)
            {
                Debug.Log("【作業紀錄】吧台加分點觸發成功");
                hasTriggeredThisTime = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopChopping();
            hasTriggeredThisTime = false;
            hasAddedScore = false; // 離開後重置，下次進來可以再加分

        }
    }

    void StartChopping()
    {
        if (luffaObject != null) luffaObject.SetActive(true);

        // 啟動陣列中所有的特效
        foreach (ParticleSystem ps in chopEffects)
        {
            if (ps != null) ps.Play();
        }

        if (chopAudio != null) chopAudio.Play();

        if (bearAnimator != null) bearAnimator.SetBool(chopBoolName, true);
    }

    void StopChopping()
    {
        // 停止陣列中所有的特效
        foreach (ParticleSystem ps in chopEffects)
        {
            if (ps != null) ps.Stop();
        }

        if (chopAudio != null) chopAudio.Stop();

        if (bearAnimator != null) bearAnimator.SetBool(chopBoolName, false);
    }
}