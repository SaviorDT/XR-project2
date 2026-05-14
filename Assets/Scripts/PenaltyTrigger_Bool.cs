using UnityEngine;

public class PenaltyTrigger_Bool : MonoBehaviour
{
    [Header("1. 特效與音樂")]
    public ParticleSystem explosionEffect;
    public AudioSource explosionAudio;

    [Header("2. 熊老師動畫")]
    public Animator bearAnimator;
    public string isFallingBoolName = "IsFalling"; // 請在 Animator 設一個 Bool 叫 IsFalling

    [Header("3. 分數設定")]
    public GameManager gameManager;
    public int penaltyAmount = -2;

    private bool hasDeductedScore = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartPenalty();

            // 進入瞬間扣分
            if (gameManager != null && !hasDeductedScore)
            {
                gameManager.AddScore(penaltyAmount);
                hasDeductedScore = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopPenalty();
            hasDeductedScore = false; // 離開後重置，下次進來會再扣分
        }
    }

    void StartPenalty()
    {
        // 1. 讓熊老師倒下 (Bool 設為 true)
        if (bearAnimator != null)
        {
            bearAnimator.SetBool(isFallingBoolName, true);
        }

        // 2. 播放爆炸特效與音樂
        if (explosionEffect != null) explosionEffect.Play();
        if (explosionAudio != null) explosionAudio.Play();

        Debug.Log("熊老師倒下了！");
    }

    void StopPenalty()
    {
        // 1. 讓熊老師爬起來 (Bool 設為 false，回到原本的 Walk/Idle 邏輯)
        if (bearAnimator != null)
        {
            bearAnimator.SetBool(isFallingBoolName, false);
        }

        Debug.Log("熊老師離開了扣分區。");
    }
}