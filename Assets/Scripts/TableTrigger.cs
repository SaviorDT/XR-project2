using UnityEngine;

public class TableTrigger : MonoBehaviour
{
    [Header("連結設定")]
    public GameManager gameManager;

    private void OnTriggerEnter(Collider other)
    {
        // 1. 檢查是不是熊老師進入區域
        if (other.CompareTag("Player"))
        {
            // 2. 詢問 GameManager 是否已經滿分
            if (gameManager != null && gameManager.IsFullScore)
            {
                gameManager.ShowAudience();
            }
            else
            {
                Debug.Log("分數還不夠喔，觀眾不想出來看熊老師煮菜。");
            }
        }
    }
}