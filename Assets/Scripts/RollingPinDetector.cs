using System;
using UnityEngine;

public class RollingPinDetector : MonoBehaviour
{
    [Header("=== 滾動偵測參數 ===")]
    [Tooltip("累積多少距離（公尺）後觸發一次事件")]
    [SerializeField] private float requiredDistance = 0.15f;

    [Tooltip("移動速度低於此值時不計入累積（過濾靜止抖動）")]
    [SerializeField] private float minSpeedThreshold = 0.05f;

    [Tooltip("開始偵測後忽略的緩衝時間（秒），避免吸附瞬間誤觸發")]
    [SerializeField] private float snapIgnoreDuration = 0.2f;

    [Tooltip("前後移動分量必須比左右移動分量大幾倍才計入")]
    [SerializeField] private float forwardDominance = 1.5f;

    [Header("=== 參考物件 ===")]
    [Tooltip("拖入 [BuildingBlock] Camera Rig > TrackingSpace > CenterEyeAnchor")]
    [SerializeField] private Transform centerEyeAnchor;

    // callback，由外部注入
    private Action onRollDetected;

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
    //   公開方法
    // =====================

    /// <summary>
    /// 注入 callback，由呼叫者決定觸發後要做什麼
    /// </summary>
    public void SetCallback(Action callback)
    {
        onRollDetected = callback;
    }

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
        if (centerEyeAnchor == null)
        {
            Debug.LogWarning("[RollingPin] centerEyeAnchor 未設定！");
            return;
        }

        // 緩衝時間內跳過偵測，持續更新 previousPosition 避免累積殘差
        if (snapIgnoreTimer > 0f)
        {
            snapIgnoreTimer -= Time.deltaTime;
            previousPosition = transform.position;
            return;
        }

        Vector3 currentPosition = transform.position;
        Vector3 delta = currentPosition - previousPosition;
        delta.y = 0f; // 忽略上下

        // 取得玩家面向的水平前方與右方
        Vector3 playerForward = centerEyeAnchor.forward;
        playerForward.y = 0f;
        playerForward.Normalize();
        Vector3 playerRight = Vector3.Cross(Vector3.up, playerForward);

        // 把移動量投影到前後軸和左右軸
        float forwardAmount = Mathf.Abs(Vector3.Dot(delta, playerForward));
        float sidewaysAmount = Mathf.Abs(Vector3.Dot(delta, playerRight));

        // 前後分量必須大於左右分量才計入
        bool isForwardMovement = forwardAmount > sidewaysAmount * forwardDominance;

        float speed = delta.magnitude / Time.deltaTime;

        if (speed > minSpeedThreshold && isForwardMovement)
        {
            accumulatedDistance += forwardAmount;
        }

        previousPosition = currentPosition;

        // 累積距離達標則觸發
        if (accumulatedDistance >= requiredDistance)
        {
            accumulatedDistance = 0f;
            onRollDetected?.Invoke();
            Debug.Log("[RollingPin] 偵測到滾動！");
        }
    }
}
