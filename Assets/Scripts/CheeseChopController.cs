using UnityEngine;
using System.Collections.Generic;

public class CheeseChopController : MonoBehaviour
{
    [Header("物件設定")]
    public GameObject wholeCheese;        // 完整的起司條
    public List<GameObject> cheeseSlices; // 所有的起司切片（預先擺好並隱藏）

    private int chopCount = 0;

    // 由熊老師的切菜動畫事件 (Animation Event) 呼叫
    public void OnCheeseChop()
    {
        if (cheeseSlices == null || cheeseSlices.Count == 0) return;

        if (chopCount < cheeseSlices.Count)
        {
            // 1. 顯示下一片起司切片
            cheeseSlices[chopCount].SetActive(true);

            // 2. 如果切到一半，可以讓完整的起司條縮短或隱藏（視你的模型而定）
            // 例如：如果是最後一刀，就關閉整條起司
            if (chopCount == cheeseSlices.Count - 1)
            {
                if (wholeCheese != null) wholeCheese.SetActive(false);
            }

            Debug.Log($"切起司！第 {chopCount + 1} 片出現");
            chopCount++;
        }
    }

    // 重設起司狀態（換關或重玩時呼叫）
    public void ResetCheese()
    {
        chopCount = 0;
        if (wholeCheese != null) wholeCheese.SetActive(true);
        foreach (var slice in cheeseSlices)
        {
            if (slice != null) slice.SetActive(false);
        }
    }
}