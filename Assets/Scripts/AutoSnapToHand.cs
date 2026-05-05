using UnityEngine;

public class AutoSnapToHand : MonoBehaviour
{
    [Header("=== 吸附設定 ===")]
    [Tooltip("拖入 [BuildingBlock] Camera Rig > TrackingSpace > RightControllerAnchor")]
    [SerializeField] private Transform controllerAnchor;

    [Tooltip("物件相對於手把的位置偏移（手把的局部空間）")]
    [SerializeField] private Vector3 positionOffset = Vector3.zero;

    [Tooltip("物件相對於手把的旋轉偏移")]
    [SerializeField] private Vector3 rotationOffset = Vector3.zero;

    [Header("=== 脫離設定 ===")]
    [Tooltip("脫離後物件回到的位置（世界座標）")]
    [SerializeField] private Vector3 detachPosition = Vector3.zero;

    public bool IsSnapped { get; private set; }

    void Update()
    {
        if (!IsSnapped || controllerAnchor == null) return;

        // 先算出最終旋轉，再用它決定位移方向
        Quaternion finalRotation = controllerAnchor.rotation * Quaternion.Euler(rotationOffset);
        transform.rotation = finalRotation;
        transform.position = controllerAnchor.position + finalRotation * positionOffset;
    }

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

    public void DetachFromHand()
    {
        IsSnapped = false;
        transform.position = detachPosition;
        Debug.Log("[AutoSnapToHand] 物件已脫離手把");
    }
}