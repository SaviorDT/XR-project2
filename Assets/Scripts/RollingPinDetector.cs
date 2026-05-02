using UnityEngine;
using UnityEngine.Events;

public class RollingPinDetector : MonoBehaviour
{
    [Header("=== 滾動偵測參數 ===")]
    [Tooltip("累積多少距離（公尺）後觸發一次事件")]
    [SerializeField] private float requiredDistance = 0.15f;

    [Tooltip("移動速度低於此值時不計入累積（過濾靜止抖動）")]
    [SerializeField] private float minSpeedThreshold = 0.05f;

    [Tooltip("開始偵測後忽略的緩衝時間（秒），避免吸附瞬間誤觸發")]
    [SerializeField] private float snapIgnoreDuration = 0.2f;

    [Header("=== 事件 ===")]
    public UnityEvent OnRollDetected;

    private bool isDetecting = false;
    private Vector3 previousPosition;
    private float accumulatedDistance = 0f;
    private float snapIgnoreTimer = 0f;

    // =====================
    //   Unity 生命週期
    // =====================

    void Update()
    {
        if (!isDetecting) return;

        CalculateRolling();
    }

    // =====================
    //   公開方法（接 UnityEvent）
    // =====================

    /// <summary>
    /// 開始偵測，接在 AutoSnapToHand.SnapToHand() 之後
    /// </summary>
    public void StartDetecting()
    {
        isDetecting = true;
        accumulatedDistance = 0f;
        snapIgnoreTimer = snapIgnoreDuration;
        previousPosition = transform.position;
        Debug.Log("[RollingPin] 開始偵測滾動");
    }

    /// <summary>
    /// 停止偵測，接在 AutoSnapToHand.DetachFromHand() 之後
    /// </summary>
    public void StopDetecting()
    {
        isDetecting = false;
        accumulatedDistance = 0f;
        Debug.Log("[RollingPin] 停止偵測滾動");
    }

    // =====================
    //   滾動計算
    // =====================

    private void CalculateRolling()
    {
        // 緩衝時間內跳過偵測，持續更新 previousPosition 避免累積殘差
        if (snapIgnoreTimer > 0f)
        {
            snapIgnoreTimer -= Time.deltaTime;
            previousPosition = transform.position;
            return;
        }

        Vector3 currentPosition = transform.position;
        Vector3 delta = currentPosition - previousPosition;

        // 只計算水平移動（忽略上下）
        delta.y = 0f;

        float speed = delta.magnitude / Time.deltaTime;

        // 速度夠才累積距離
        if (speed > minSpeedThreshold)
        {
            accumulatedDistance += delta.magnitude;
        }

        previousPosition = currentPosition;

        // 累積距離達標則觸發
        if (accumulatedDistance >= requiredDistance)
        {
            accumulatedDistance = 0f;
            OnRollDetected?.Invoke();
            Debug.Log("[RollingPin] 偵測到滾動！");
        }
    }
}
