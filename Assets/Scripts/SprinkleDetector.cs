using System;
using UnityEngine;

public class SprinkleDetector : MonoBehaviour
{
    [Header("=== 揮動偵測參數 ===")]
    [Tooltip("觸發揮動所需的最低速度（m/s）")]
    [SerializeField] private float swingThreshold = 2.0f;

    [Tooltip("左右速度必須比前後和垂直速度大幾倍才算橫向揮動")]
    [SerializeField] private float sidewaysDominance = 1.5f;

    [Tooltip("兩次揮動之間的最短間隔（秒）")]
    [SerializeField] private float cooldownTime = 0.3f;

    [Tooltip("速度降到此比例以下才算揮動結束")]
    [SerializeField] private float swingEndSpeedRatio = 0.5f;

    [Header("=== 參考物件 ===")]
    [Tooltip("拖入 [BuildingBlock] Camera Rig > TrackingSpace > CenterEyeAnchor")]
    [SerializeField] private Transform centerEyeAnchor;

    // callback，由外部注入
    private Action onSprinkleDetected;

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
        DetectSideSwing();
    }

    // =====================
    //   公開方法
    // =====================

    /// <summary>
    /// 注入 callback，由呼叫者決定觸發後要做什麼
    /// </summary>
    public void SetCallback(Action callback)
    {
        onSprinkleDetected = callback;
    }

    /// <summary>
    /// 開始偵測，接在 AutoSnapToHand.SnapToHand() 之後
    /// </summary>
    public void StartDetecting()
    {
        isDetecting = true;
        previousPosition = transform.position;
        ResetSwingState();
        Debug.Log("[Sprinkle] 開始偵測灑料揮動");
    }

    /// <summary>
    /// 停止偵測，接在 AutoSnapToHand.DetachFromHand() 之後
    /// </summary>
    public void StopDetecting()
    {
        isDetecting = false;
        ResetSwingState();
        Debug.Log("[Sprinkle] 停止偵測灑料揮動");
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
    //   左右揮動偵測（相對玩家面向）
    // =====================

    private void DetectSideSwing()
    {
        if (centerEyeAnchor == null)
        {
            Debug.LogWarning("[Sprinkle] centerEyeAnchor 未設定！");
            return;
        }

        float speed = currentVelocity.magnitude;

        // 取得玩家面向的水平前方與右方
        Vector3 playerForward = centerEyeAnchor.forward;
        playerForward.y = 0f;
        playerForward.Normalize();
        Vector3 playerRight = Vector3.Cross(Vector3.up, playerForward);

        // 把速度投影到左右軸、前後軸、垂直軸
        float sidewaysSpeed = Mathf.Abs(Vector3.Dot(currentVelocity, playerRight));
        float forwardSpeed = Mathf.Abs(Vector3.Dot(currentVelocity, playerForward));
        float verticalSpeed = Mathf.Abs(currentVelocity.y);

        // 左右分量必須同時大於前後和垂直分量才算橫向揮動
        bool isSideways = sidewaysSpeed > forwardSpeed * sidewaysDominance
                       && sidewaysSpeed > verticalSpeed * sidewaysDominance;

        // 1. 偵測揮動開始（左右任一方向都算）
        if (!swingInProgress && speed > swingThreshold && isSideways)
        {
            swingInProgress = true;
        }

        // 2. 偵測揮動結束並觸發 callback
        if (swingInProgress && speed < swingThreshold * swingEndSpeedRatio)
        {
            if (Time.time - lastSwingTime > cooldownTime)
            {
                lastSwingTime = Time.time;
                onSprinkleDetected?.Invoke();
                Debug.Log("[Sprinkle] 偵測到左右揮動！");
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
