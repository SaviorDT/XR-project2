using UnityEngine;
using UnityEngine.Events;

public class DrumStickSwingDetector : MonoBehaviour
{
    [Header("=== 揮動偵測參數 ===")]
    [Tooltip("觸發揮動所需的最低速度（m/s）")]
    [SerializeField] private float swingThreshold = 2.0f;

    [Tooltip("速度方向與正下方的相似度閾值（-1~1，越高越嚴格）")]
    [SerializeField] private float downwardAngleThreshold = 0.5f;

    [Tooltip("兩次揮動之間的最短間隔（秒）")]
    [SerializeField] private float cooldownTime = 0.3f;

    [Tooltip("速度降到此比例以下才算揮動結束")]
    [SerializeField] private float swingEndSpeedRatio = 0.5f;

    [Header("=== 事件 ===")]
    public UnityEvent OnSwingDetected;

    private bool isDetecting = false;
    private bool swingInProgress = false;
    private Vector3 previousPosition;
    private Vector3 currentVelocity;
    private float lastSwingTime = -999f;

    // =====================
    //   Unity 生命週期
    // =====================

    void Update()
    {
        if (!isDetecting) return;

        CalculateVelocity();
        DetectDownSwing();
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
        previousPosition = transform.position;
        ResetSwingState();
        Debug.Log("[DrumStick] 開始偵測揮動");
    }

    /// <summary>
    /// 停止偵測，接在 AutoSnapToHand.DetachFromHand() 之後
    /// </summary>
    public void StopDetecting()
    {
        isDetecting = false;
        ResetSwingState();
        Debug.Log("[DrumStick] 停止偵測揮動");
    }

    // =====================
    //   速度計算
    // =====================

    private void CalculateVelocity()
    {
        currentVelocity = (transform.position - previousPosition) / Time.deltaTime;
        previousPosition = transform.position;
    }

    // =====================
    //   揮動偵測
    // =====================

    private void DetectDownSwing()
    {
        float speed = currentVelocity.magnitude;
        float downwardDot = Vector3.Dot(currentVelocity.normalized, Vector3.down);
        bool isMovingDown = downwardDot > downwardAngleThreshold;

        // 1. 偵測揮動開始
        if (!swingInProgress && speed > swingThreshold && isMovingDown)
        {
            swingInProgress = true;
        }

        // 2. 偵測揮動結束並觸發事件
        if (swingInProgress && speed < swingThreshold * swingEndSpeedRatio)
        {
            if (Time.time - lastSwingTime > cooldownTime)
            {
                lastSwingTime = Time.time;
                OnSwingDetected?.Invoke();
                Debug.Log("[DrumStick] 偵測到向下揮動！");
            }
            ResetSwingState();
        }
    }

    // =====================
    //   工具方法
    // =====================

    private void ResetSwingState()
    {
        swingInProgress = false;
    }
}
