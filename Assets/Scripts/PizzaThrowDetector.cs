using System;
using UnityEngine;

public class PizzaThrowDetector : MonoBehaviour
{
    [Header("=== 揮舞偵測參數 ===")]
    [Tooltip("觸發投擲所需的最低速度（m/s）")]
    [SerializeField] private float throwThreshold = 2.0f;

    [Tooltip("水平速度必須比垂直速度大幾倍才算橫向揮舞")]
    [SerializeField] private float horizontalDominance = 1.5f;

    [Tooltip("兩次投擲之間的最短間隔（秒）")]
    [SerializeField] private float cooldownTime = 0.3f;

    [Tooltip("速度降到此比例以下才算揮舞結束")]
    [SerializeField] private float throwEndSpeedRatio = 0.5f;

    [Header("=== 參考物件 ===")]
    [Tooltip("拖入 [BuildingBlock] Camera Rig > TrackingSpace > CenterEyeAnchor")]
    [SerializeField] private Transform centerEyeAnchor;

    // callback，由外部注入
    private Action onThrowDetected;

    private bool isDetecting = false;
    private bool throwInProgress = false;
    private Vector3 previousPosition;
    private Vector3 currentVelocity;
    private float lastThrowTime = -999f;

    // =====================
    //   Unity 生命週期
    // =====================

    void Update()
    {
        if (!isDetecting) return;

        CalculateVelocity();
        DetectThrow();
    }

    // =====================
    //   公開方法
    // =====================

    /// <summary>
    /// 注入 callback，由呼叫者決定觸發後要做什麼
    /// </summary>
    public void SetCallback(Action callback)
    {
        onThrowDetected = callback;
    }

    /// <summary>
    /// 開始偵測，接在 AutoSnapToHand.SnapToHand() 之後
    /// </summary>
    public void StartDetecting()
    {
        isDetecting = true;
        previousPosition = transform.position;
        ResetThrowState();
        Debug.Log("[PizzaThrow] 開始偵測投擲");
    }

    /// <summary>
    /// 停止偵測，接在 AutoSnapToHand.DetachFromHand() 之後
    /// </summary>
    public void StopDetecting()
    {
        isDetecting = false;
        ResetThrowState();
        Debug.Log("[PizzaThrow] 停止偵測投擲");
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
    //   橫向揮舞偵測（相對玩家面向的右→左）
    // =====================

    private void DetectThrow()
    {
        if (centerEyeAnchor == null)
        {
            Debug.LogWarning("[PizzaThrow] centerEyeAnchor 未設定！");
            return;
        }

        float speed = currentVelocity.magnitude;

        // 取得玩家面向的水平右方向（忽略Y軸傾斜）
        Vector3 playerForward = centerEyeAnchor.forward;
        playerForward.y = 0f;
        playerForward.Normalize();
        Vector3 playerRight = Vector3.Cross(Vector3.up, playerForward);

        // 把速度投影到玩家的左右軸和垂直軸
        float rightwardSpeed = Vector3.Dot(currentVelocity, playerRight); // 正值=向右，負值=向左
        float verticalSpeed = Mathf.Abs(currentVelocity.y);
        float horizontalSpeed = new Vector2(currentVelocity.x, currentVelocity.z).magnitude;

        // 確認是橫向移動且相對玩家往左
        bool isHorizontal = horizontalSpeed > verticalSpeed * horizontalDominance;
        bool isLeftward = rightwardSpeed < 0;

        // 1. 偵測揮舞開始
        if (!throwInProgress && speed > throwThreshold && isHorizontal && isLeftward)
        {
            throwInProgress = true;
        }

        // 2. 偵測揮舞結束並觸發 callback
        if (throwInProgress && speed < throwThreshold * throwEndSpeedRatio)
        {
            if (Time.time - lastThrowTime > cooldownTime)
            {
                lastThrowTime = Time.time;
                onThrowDetected?.Invoke();
                Debug.Log("[PizzaThrow] 偵測到右向左揮舞！");
            }
            ResetThrowState();
        }
    }

    // =====================
    //   工具方法
    // =====================

    private void ResetThrowState()
    {
        throwInProgress = false;
    }
}
