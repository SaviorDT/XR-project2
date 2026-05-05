using UnityEngine;
using System.Collections.Generic;

public class BearPizzaMaster : MonoBehaviour
{
    [Header("配料設定")]
    [Tooltip("請將披薩上的 5 片起司和 4 片番茄依序拖入此清單")]
    public List<GameObject> pizzaToppings = new List<GameObject>();

    private int currentToppingIndex = 0;

    // 當老師動作到「放料」那一格時，Animation Event 會呼叫這個 Function
    public void AddNextTopping()
    {
        // 檢查清單是否有東西，且是否還有沒顯示的配料
        if (pizzaToppings != null && currentToppingIndex < pizzaToppings.Count)
        {
            if (pizzaToppings[currentToppingIndex] != null)
            {
                // 直接讓配料顯現
                pizzaToppings[currentToppingIndex].SetActive(true);

                Debug.Log($"熊老師放了：{pizzaToppings[currentToppingIndex].name} (第 {currentToppingIndex + 1} 個)");

                currentToppingIndex++;
            }
        }
        else if (currentToppingIndex >= pizzaToppings.Count && pizzaToppings.Count > 0)
        {
            Debug.Log("所有配料都放完了！");
        }
    }

    // 提供一個重設功能，方便你反覆測試
    public void ResetAllToppings()
    {
        currentToppingIndex = 0;
        foreach (var topping in pizzaToppings)
        {
            if (topping != null) topping.SetActive(false);
        }
    }
}
