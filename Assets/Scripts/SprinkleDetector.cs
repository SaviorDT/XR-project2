using UnityEngine;
using UnityEngine.Events;

public class SprinkleDetector : MonoBehaviour
{
    [Header("=== 揮動偵測參數 ===")]
    [Tooltip("觸發揮動所需的最低速度（m/s）")]
    [SerializeField] private float swingThreshold = 2.0f;

    [Tooltip("左右速度必須比前後和垂直速度大幾倍才算橫向揮動")]
    [SerializeField] private float sidewaysDominance = 1.5f;

    [Tooltip("兩次揮動之間的最短間隔")]
    [SerializeField] private float cooldownTime = 0.3f;

    [Header("=== 進階優化 ===")]
    [Range(0, 1)]
    [Tooltip("速度平滑係數")]
    [SerializeField] private float velocitySmoothing = 0.7f;

    [Header("=== 參考物件 ===")]
    [Tooltip("拖入 [BuildingBlock] Camera Rig > TrackingSpace > CenterEyeAnchor")]
    [SerializeField] private Transform centerEyeAnchor;

    [Header("=== 外部引用 ===")]
    [SerializeField] private GameCore gameCore;
    [SerializeField] private TempoEventType eventType = TempoEventType.put;

    public UnityEvent OnSprinkleDetected;

    private bool isDetecting = true;
    private bool swingProcessed = false;
    private Vector3 previousPosition;
    private Vector3 smoothedVelocity;
    private float lastSwingTime = -999f;

    public void SetCallback(UnityAction callback)
    {
        OnSprinkleDetected.RemoveListener(callback);
        OnSprinkleDetected.AddListener(callback);
    }

    void Update()
    {
        if (!isDetecting) return;

        CalculateVelocity();
        DetectSideSwing();
    }

    public void StartDetecting()
    {
        isDetecting = true;
        previousPosition = transform.position;
        smoothedVelocity = Vector3.zero;
        swingProcessed = false;
        Debug.Log("<color=yellow>[Sprinkle]</color> 偵測開啟");
    }

    public void StopDetecting()
    {
        isDetecting = false;
        swingProcessed = false;
        Debug.Log("<color=yellow>[Sprinkle]</color> 偵測關閉");
    }

    private void CalculateVelocity()
    {
        Vector3 rawVelocity = (transform.position - previousPosition) / Time.deltaTime;
        previousPosition = transform.position;
        smoothedVelocity = Vector3.Lerp(smoothedVelocity, rawVelocity, 1f - velocitySmoothing);
    }

    private void DetectSideSwing()
    {
        if (centerEyeAnchor == null) return;

        float speed = smoothedVelocity.magnitude;

        // 取得玩家面向的水平參考軸
        Vector3 playerForward = centerEyeAnchor.forward;
        playerForward.y = 0f;
        playerForward.Normalize();
        Vector3 playerRight = Vector3.Cross(Vector3.up, playerForward);

        // 投影速度分量
        float sidewaysSpeed = Mathf.Abs(Vector3.Dot(smoothedVelocity, playerRight));
        float forwardSpeed = Mathf.Abs(Vector3.Dot(smoothedVelocity, playerForward));
        float verticalSpeed = Mathf.Abs(smoothedVelocity.y);

        // 判定是否為橫向揮動
        bool isSideways = sidewaysSpeed > forwardSpeed * sidewaysDominance
                       && sidewaysSpeed > verticalSpeed * sidewaysDominance;

        if (speed > swingThreshold && isSideways)
        {
            if (!swingProcessed && Time.time - lastSwingTime > cooldownTime)
            {
                TriggerSprinkle();
            }
        }
        else if (speed < swingThreshold * 0.5f)
        {
            swingProcessed = false;
        }
    }

    private void TriggerSprinkle()
    {
        lastSwingTime = Time.time;
        swingProcessed = true;

        OnSprinkleDetected?.Invoke();
        gameCore?.OnInput(eventType);
        
        Debug.Log($"<color=yellow>[Sprinkle]</color> 偵測到橫向灑料！速度: {smoothedVelocity.magnitude:F2}");
    }
}