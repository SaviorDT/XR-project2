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

    [Header("=== 脫離設定 ===")]
    [Tooltip("脫離後物件回到的位置（世界座標）")]
    [SerializeField] private Vector3 detachPosition = Vector3.zero;

    public bool IsSnapped { get; private set; }

    // =====================
    //   Unity 生命週期
    // =====================

    void Update()
    {
        if (!IsSnapped) return;
        if (controllerAnchor == null) return;

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

        IsSnapped = true;
        Debug.Log("[AutoSnapToHand] 物件已吸附到手上");
    }

    /// <summary>
    /// 從手上脫離，可接在任意 UnityEvent
    /// </summary>
    public void DetachFromHand()
    {
        IsSnapped = false;
        transform.position = detachPosition;
        Debug.Log("[AutoSnapToHand] 物件已脫離手把");
    }
}
