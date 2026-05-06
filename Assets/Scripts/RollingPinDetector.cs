using UnityEngine;
using UnityEngine.Events;

public class RollingPinDetector : MonoBehaviour
{
    [Header("=== 滾動偵測參數 ===")]
    [Tooltip("累積多少距離（公尺）後觸發一次事件")]
    [SerializeField] private float requiredDistance = 0.15f;

    [Tooltip("移動速度低於此值時不計入累積（過濾靜止抖動）")]
    [SerializeField] private float minSpeedThreshold = 0.05f;

    [Tooltip("前後移動分量必須比左右移動分量大幾倍才計入")]
    [SerializeField] private float forwardDominance = 1.5f;

    [Tooltip("開始偵測後忽略的緩衝時間（秒），避免吸附瞬間誤觸發")]
    [SerializeField] private float snapIgnoreDuration = 0.2f;

    [Header("=== 進階優化 ===")]
    [Range(0, 1)]
    [Tooltip("速度平滑係數，越高越平穩但延遲越高")]
    [SerializeField] private float velocitySmoothing = 0.5f;

    [Header("=== 外部引用 ===")]
    [SerializeField] private GameCore gameCore;
    [SerializeField] private TempoEventType eventType = TempoEventType.roll; // 設定為 roll 事件

    public UnityEvent OnRollDetected;

    private bool isDetecting = true;
    private Vector3 previousPosition;
    private Vector3 smoothedVelocity;
    private float accumulatedDistance = 0f;
    private float snapIgnoreTimer = 0f;

    // =====================
    //   公開方法
    // =====================

    public void SetCallback(UnityAction callback)
    {
        // 依照 DrumStick 格式：移除舊的並增加新的監聽
        OnRollDetected.RemoveListener(callback);
        OnRollDetected.AddListener(callback);
    }

    public void StartDetecting()
    {
        isDetecting = true;
        accumulatedDistance = 0f;
        snapIgnoreTimer = snapIgnoreDuration; //
        previousPosition = transform.position;
        smoothedVelocity = Vector3.zero;
        Debug.Log("[RollingPin] 偵測開啟"); // 依照 DrumStick 格式
    }

    public void StopDetecting()
    {
        isDetecting = false;
        accumulatedDistance = 0f;
        Debug.Log("[RollingPin] 偵測關閉");
    }

    // =====================
    //   Unity 生命週期
    // =====================

    void Update()
    {
        if (!isDetecting) return;

        CalculateVelocityAndRolling();
    }

    private void CalculateVelocityAndRolling()
    {
        // 1. 緩衝時間檢查[cite: 2]
        if (snapIgnoreTimer > 0f)
        {
            snapIgnoreTimer -= Time.deltaTime;
            previousPosition = transform.position;
            return;
        }

        // 2. 計算平滑速度 (參考 DrumStick 格式)[cite: 1]
        Vector3 rawDelta = transform.position - previousPosition;
        Vector3 rawVelocity = rawDelta / Time.deltaTime;
        smoothedVelocity = Vector3.Lerp(smoothedVelocity, rawVelocity, 1f - velocitySmoothing);
        
        // 3. 執行滾動邏輯判斷
        DetectRolling(rawDelta);

        previousPosition = transform.position;
    }

    private void DetectRolling(Vector3 delta)
    {
        // 忽略 Y 軸位移[cite: 2]
        Vector3 horizontalDelta = delta;
        horizontalDelta.y = 0f;

        // 使用局部座標軸 (Local Forward) 作為滾動方向
        Vector3 rollDirection = transform.forward;
        rollDirection.y = 0f;
        rollDirection.Normalize();

        // 取得與滾動方向垂直的長軸方向
        Vector3 pinAxleDirection = Vector3.Cross(Vector3.up, rollDirection);

        // 投影移動量
        float forwardAmount = Mathf.Abs(Vector3.Dot(horizontalDelta, rollDirection));
        float sidewaysAmount = Mathf.Abs(Vector3.Dot(horizontalDelta, pinAxleDirection));

        // 判定移動方向是否正確[cite: 2]
        bool isForwardMovement = forwardAmount > sidewaysAmount * forwardDominance;
        float currentSpeed = smoothedVelocity.magnitude;

        if (currentSpeed > minSpeedThreshold && isForwardMovement)
        {
            accumulatedDistance += forwardAmount;
        }

        // 達到目標距離則觸發[cite: 2]
        if (accumulatedDistance >= requiredDistance)
        {
            TriggerRoll();
        }
    }

    private void TriggerRoll()
    {
        accumulatedDistance = 0f;

        // 執行回傳[cite: 1, 3]
        OnRollDetected?.Invoke();
        gameCore?.OnInput(eventType);

        // 使用與 DrumStick 相同的富文本 Debug Log 格式[cite: 1]
        Debug.Log($"<color=orange>[RollingPin]</color> 偵測到滾動！累積距離重置。當前平滑速度: {smoothedVelocity.magnitude:F2}");
    }
}