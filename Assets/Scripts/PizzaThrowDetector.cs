using UnityEngine;
using UnityEngine.Events;

public class PizzaThrowDetector : MonoBehaviour
{
    [Header("=== 揮舞偵測參數 ===")]
    [Tooltip("觸發投擲所需的最低速度（m/s）")]
    [SerializeField] private float throwThreshold = 2.0f;

    [Tooltip("水平速度必須比垂直速度大幾倍才算橫向揮舞")]
    [SerializeField] private float horizontalDominance = 1.5f;

    [Tooltip("兩次投擲之間的最短間隔（秒）")]
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
    [SerializeField] private TempoEventType eventType = TempoEventType.send; // 依照要求改為 send

    public UnityEvent OnThrowDetected;

    private bool isDetecting = false;
    private bool throwProcessed = false; 
    private Vector3 previousPosition;
    private Vector3 smoothedVelocity;
    private float lastThrowTime = -999f;

    public void SetCallback(UnityAction callback)
    {
        OnThrowDetected.RemoveListener(callback);
        OnThrowDetected.AddListener(callback);
    }

    void Update()
    {
        if (!isDetecting) return;

        CalculateVelocity();
        DetectLeftwardThrow();
    }

    public void StartDetecting()
    {
        isDetecting = true;
        previousPosition = transform.position;
        smoothedVelocity = Vector3.zero;
        throwProcessed = false;
        Debug.Log("<color=red>[PizzaThrow]</color> 偵測開啟");
    }

    public void StopDetecting()
    {
        isDetecting = false;
        throwProcessed = false;
        Debug.Log("<color=red>[PizzaThrow]</color> 偵測關閉");
    }

    private void CalculateVelocity()
    {
        Vector3 rawVelocity = (transform.position - previousPosition) / Time.deltaTime;
        previousPosition = transform.position;
        smoothedVelocity = Vector3.Lerp(smoothedVelocity, rawVelocity, 1f - velocitySmoothing);
    }

    private void DetectLeftwardThrow()
    {
        if (centerEyeAnchor == null) return;

        float speed = smoothedVelocity.magnitude;

        // 取得玩家面向的水平參考軸
        Vector3 playerForward = centerEyeAnchor.forward;
        playerForward.y = 0f;
        playerForward.Normalize();
        Vector3 playerRight = Vector3.Cross(Vector3.up, playerForward);

        // 投影速度分量
        float rightwardSpeed = Vector3.Dot(smoothedVelocity, playerRight); // 正值=向右，負值=向左
        float verticalSpeed = Mathf.Abs(smoothedVelocity.y);
        float horizontalSpeed = new Vector2(smoothedVelocity.x, smoothedVelocity.z).magnitude;

        // 判定判定條件：水平主導且方向向左[cite: 6]
        bool isHorizontal = horizontalSpeed > verticalSpeed * horizontalDominance;
        bool isLeftward = rightwardSpeed < 0;

        if (speed > throwThreshold && isHorizontal && isLeftward)
        {
            if (!throwProcessed && Time.time - lastThrowTime > cooldownTime)
            {
                TriggerThrow();
            }
        }
        else if (speed < throwThreshold * 0.5f)
        {
            throwProcessed = false;
        }
    }

    private void TriggerThrow()
    {
        lastThrowTime = Time.time;
        throwProcessed = true;

        OnThrowDetected?.Invoke();
        gameCore?.OnInput(eventType);
        
        Debug.Log($"<color=red>[PizzaThrow]</color> 偵測到右至左投擲！速度: {smoothedVelocity.magnitude:F2}");
    }
}