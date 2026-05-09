using UnityEngine;
using System.Collections.Generic;

public class BearCheeseMaster : MonoBehaviour
{
    [Header("物件設定")]
    [Tooltip("完整的起司條模型")]
    public GameObject wholeCheese;

    [Header("切片設定")]
    [Tooltip("請依序拖入所有的起司小切片")]
    public List<GameObject> cheeseSlices = new List<GameObject>();

    private int chopCount = 0;

    // 當老師動畫執行到「切」的那一格時，Animation Event 會呼叫此 Method
    public void OnCheeseChop()
    {
        // 安全檢查：如果沒設起司片就直接跳出
        if (cheeseSlices == null || cheeseSlices.Count == 0) return;

        // 【方案三核心邏輯】：第一刀下去時，立刻關閉整條起司
        if (chopCount == 0 && wholeCheese != null)
        {
            wholeCheese.SetActive(false);
            Debug.Log("第一刀！隱藏完整起司條。");
        }

        // 依序顯示切片
        if (chopCount < cheeseSlices.Count)
        {
            if (cheeseSlices[chopCount] != null)
            {
                cheeseSlices[chopCount].SetActive(true);
                Debug.Log($"切起司：顯示第 {chopCount + 1} 個切片。");
            }
            chopCount++;
        }
        else
        {
            Debug.Log("所有起司片都切完了！");
        }
    }

    // 重設功能：方便你測試或進入下一關
    public void ResetCheeseLevel()
    {
        chopCount = 0;
        if (wholeCheese != null) wholeCheese.SetActive(true);
        foreach (var slice in cheeseSlices)
        {
            if (slice != null) slice.SetActive(false);
        }
    }
}
