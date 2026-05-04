using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MasterChefController : MonoBehaviour
{
    [Header("設定參數")]
    public string triggerName = "StartCooking"; // Animator 裡的 Trigger 名稱
    public KeyCode debugKey = KeyCode.Space;    // 測試用的按鍵

    [Header("目標物件 (手動拖入或自動抓取)")]
    public List<Animator> allAnimators = new List<Animator>();

    void Update()
    {
        // 1. 按下空白鍵即可全體測試
        if (Input.GetKeyDown(debugKey))
        {
            StartAllActions();
        }
    }

    // 這個 Function 可以給 UI Button 的 OnClick 呼叫
    public void StartAllActions()
    {
        Debug.Log("開始煮菜！觸發所有動畫...");

        // 遍歷清單中所有的 Animator 並觸發
        foreach (Animator anim in allAnimators)
        {
            if (anim != null)
            {
                anim.SetTrigger(triggerName);
            }
        }

        // 也可以順便尋找場景中所有帶有特定標籤的物件
        // GameObject[] foods = GameObject.FindGameObjectsWithTag("Food");
        // foreach (GameObject food in foods) { ... }
    }
}
