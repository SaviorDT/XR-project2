using UnityEngine;
using System.Collections.Generic;

public class PizzaToppingController : MonoBehaviour
{
    [Header("配料清單 (依出現順序拖入)")]
    public List<GameObject> toppings = new List<GameObject>();

    private int currentIndex = 0;

    // 這個 Function 會由 Animator 的 Animation Event 呼叫
    public void AddNextTopping()
    {
        if (currentIndex < toppings.Count)
        {
            // 讓當前的配料顯現
            toppings[currentIndex].SetActive(true);

            // 播放一個小特效（如果有之前下載的骷髏頭或閃光特效可以放這）
            Debug.Log($"已放上第 {currentIndex + 1} 個配料：{toppings[currentIndex].name}");

            currentIndex++;
        }
        else
        {
            Debug.Log("所有配料都放完了！");
        }
    }

    // 重設披薩（如果需要重新開始一輪）
    public void ResetPizza()
    {
        currentIndex = 0;
        foreach (GameObject obj in toppings)
        {
            obj.SetActive(false);
        }
    }
}