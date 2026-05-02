using UnityEngine;
using System.Collections.Generic;

public class SlicingSystem : MonoBehaviour
{
    [Header("物件設定 (絲瓜)")]
    // 拖入 Rhino 切好的那 7 個子物件 (Loofah_01 ~ 07)
    public List<GameObject> vegetableSlices;
    // 尚未切開的完整絲瓜
    public GameObject wholeVeggie;

    [Header("回饋設定")]
    public ParticleSystem chopEffect;
    public AudioSource chopSound;

    private int currentSliceIndex = 0;

    // 每次落刀觸發一次 (總共會被呼叫 6 次)
    public void OnChop()
    {
        // 加一個安全檢查：確保 index 不會超出 List 的範圍
        if (currentSliceIndex < 6 && currentSliceIndex + 1 < vegetableSlices.Count)
        {
            if (currentSliceIndex == 0)
            {
                if (wholeVeggie != null) wholeVeggie.SetActive(false);
                if (vegetableSlices.Count > 1)
                {
                    vegetableSlices[0].SetActive(true);
                    vegetableSlices[1].SetActive(true);
                }
            }
            else
            {
                vegetableSlices[currentSliceIndex + 1].SetActive(true);
            }

            PlayFeedback();
            currentSliceIndex++;
        }
        else
        {
            Debug.LogWarning("落刀次數超過了切片數量！請檢查動畫事件。");
        }
    }

    private void PlayFeedback()
    {
        if (chopEffect != null) chopEffect.Play();
        if (chopSound != null) chopSound.Play();

        // 這裡可以加入簡單的螢幕震動或手感回饋 (Haptic Feedback)
        Debug.Log($"切到第 {currentSliceIndex + 1} 塊了！");
    }

    // 重置絲瓜狀態
    public void ResetLoofah()
    {
        currentSliceIndex = 0;
        if (wholeVeggie != null) wholeVeggie.SetActive(true);
        foreach (var slice in vegetableSlices) slice.SetActive(false);
    }
}
