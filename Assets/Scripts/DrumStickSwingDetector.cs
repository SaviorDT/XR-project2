using UnityEngine;
using UnityEngine.Events;

public class DrumStickSwingDetector : MonoBehaviour
{
    [Header("=== 揮動偵測參數 ===")]
    [Tooltip("觸發揮動所需的最低速度（建議 1.5~3.0）")]
    [SerializeField] private float swingThreshold = 2.2f;

    [Tooltip("下揮方向判定（1.0 是正下方，0.3~0.5 比較寬鬆）")]
    [SerializeField] private float downwardAngleThreshold = 0.4f;

    [Tooltip("兩次揮動之間的最短間隔")]
    [SerializeField] private float cooldownTime = 0.2f;

    [Header("=== 進階優化 ===")]
    [Range(0, 1)]
    [Tooltip("速度平滑係數，越高越平穩但延遲越高")]
    [SerializeField] private float velocitySmoothing = 0.7f;

    [Header("=== 外部引用 ===")]
    [SerializeField] private GameCore gameCore;
    [SerializeField] private TempoEventType eventType = TempoEventType.cut;

    public UnityEvent OnSwingDetected;

    private bool isDetecting = true;
    private bool swingProcessed = false; // 確保單次揮擊只觸發一次
    private Vector3 previousPosition;
    private Vector3 smoothedVelocity;
    private float lastSwingTime = -999f;
    
    public void SetCallback(UnityAction callback)
{
    // 先移除舊的，避免重複綁定導致一次揮動觸發兩次
    OnSwingDetected.RemoveListener(callback);
    OnSwingDetected.AddListener(callback);
}

    void Update()
    {
        if (!isDetecting) return;

        CalculateVelocity();
        DetectDownSwing();
    }

    public void StartDetecting()
    {
        isDetecting = true;
        previousPosition = transform.position;
        smoothedVelocity = Vector3.zero;
        swingProcessed = false;
        Debug.Log("[DrumStick] 偵測開啟");
    }

    public void StopDetecting()
    {
        isDetecting = false;
        swingProcessed = false;
    }

    private void CalculateVelocity()
    {
        // 計算原始速度
        Vector3 rawVelocity = (transform.position - previousPosition) / Time.deltaTime;
        previousPosition = transform.position;

        // 平滑處理速度，減少 VR 控制器震顫造成的誤判
        smoothedVelocity = Vector3.Lerp(smoothedVelocity, rawVelocity, 1f - velocitySmoothing);
    }

    private void DetectDownSwing()
    {
        float speed = smoothedVelocity.magnitude;
        // 計算與世界坐標下方的夾角相似度
        float downwardDot = Vector3.Dot(smoothedVelocity.normalized, Vector3.down);

        // 偵測邏輯：
        // 1. 速度超過閾值
        // 2. 方向朝下
        // 3. 冷卻時間已過
        if (speed > swingThreshold && downwardDot > downwardAngleThreshold)
        {
            if (!swingProcessed && Time.time - lastSwingTime > cooldownTime)
            {
                TriggerSwing();
            }
        }
        else if (speed < swingThreshold * 0.5f)
        {
            // 當速度降下來，重置狀態，準備下一次揮擊
            swingProcessed = false;
        }
    }

    private void TriggerSwing()
    {
        lastSwingTime = Time.time;
        swingProcessed = true; // 標記已處理，直到速度放慢前不再重複觸發

        OnSwingDetected?.Invoke();
        gameCore?.OnInput(eventType);
        
        Debug.Log($"<color=cyan>[DrumStick]</color> 偵測到擊打！速度: {smoothedVelocity.magnitude:F2}");
    }
}