using UnityEngine;

public class AutoSnapToHand : MonoBehaviour
{
    [Header("=== 吸附設定 ===")]
    [Tooltip("拖入 [BuildingBlock] Camera Rig > TrackingSpace > RightHandAnchor")]
    [SerializeField] private Transform controllerAnchor;

    [Tooltip("物件相對於手把的位置偏移")]
    [SerializeField] private Vector3 positionOffset = Vector3.zero;

    [Tooltip("物件相對於手把的旋轉偏移")]
    [SerializeField] private Vector3 rotationOffset = Vector3.zero;

    private bool isSnapped = false;

    // =====================
    //   Unity 生命週期
    // =====================

    void Update()
    {
        if (!isSnapped) return;
        if (controllerAnchor == null) return;

        // 每幀跟隨手把位置與旋轉
        transform.position = controllerAnchor.position
            + controllerAnchor.TransformDirection(positionOffset);

        transform.rotation = controllerAnchor.rotation
            * Quaternion.Euler(rotationOffset);
    }

    // =====================
    //   公開方法（接 UnityEvent）
    // =====================

    /// <summary>
    /// 吸附到手上，可接在任意 UnityEvent
    /// </summary>
    public void SnapToHand()
    {
        if (controllerAnchor == null)
        {
            Debug.LogWarning("[AutoSnapToHand] controllerAnchor 未設定！");
            return;
        }

        isSnapped = true;
        Debug.Log("[AutoSnapToHand] 物件已吸附到手上");
    }

    /// <summary>
    /// 從手上脫離，可接在任意 UnityEvent
    /// </summary>
    public void DetachFromHand()
    {
        isSnapped = false;
        Debug.Log("[AutoSnapToHand] 物件已脫離手把");
    }

    /// <summary>
    /// 查詢目前是否吸附中
    /// </summary>
    public bool IsSnapped => isSnapped;
}
