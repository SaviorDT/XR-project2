using UnityEngine;

public class CookingController : MonoBehaviour
{
    [Header("1. 特效與燈光")]
    public ParticleSystem fireEffect;
    public Light stoveLight;

    [Header("2. 動態設定")]
    public Animator bearAnimator;
    public string cookBoolName = "IsCooking";

    [Header("3. 音樂設定")]
    public AudioSource stoveAudio;

    [Header("4. 分數設定 (新加入)")]
    public GameManager gameManager;  // 拖入場景中的 GameManager 物件
    private bool hasAddedScore = false; // 確保每次進入只加一次分

    private void Start()
    {
        if (stoveLight != null) stoveLight.enabled = false;
        if (fireEffect != null) fireEffect.Stop();
        if (stoveAudio != null) stoveAudio.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCooking();

            // --- 加分邏輯開始 ---
            if (gameManager != null && !hasAddedScore)
            {
                gameManager.AddScore(1);
                hasAddedScore = true; // 標記為已加分
                Debug.Log("火爐加分成功！");
            }
            // --- 加分邏輯結束 ---
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopCooking();
            hasAddedScore = false; // 離開後重置，下次進來可以再加分
        }
    }

    void StartCooking()
    {
        if (stoveLight != null) stoveLight.enabled = true;
        if (fireEffect != null) fireEffect.Play();
        if (stoveAudio != null) stoveAudio.Play();
        if (bearAnimator != null) bearAnimator.SetBool(cookBoolName, true);
    }

    void StopCooking()
    {
        if (stoveLight != null) stoveLight.enabled = false;
        if (fireEffect != null) fireEffect.Stop();
        if (stoveAudio != null) stoveAudio.Stop();
        if (bearAnimator != null) bearAnimator.SetBool(cookBoolName, false);
    }
}