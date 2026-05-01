using UnityEngine;
using UnityEngine.Events;
using Oculus.Interaction;

public class RollingPinDetector : MonoBehaviour
{
    [Header("=== Grab 設定 ===")]
    [Tooltip("擀麵棍左側的 GrabInteractable")]
    [SerializeField] private GrabInteractable leftGrabInteractable;
    [Tooltip("擀麵棍右側的 GrabInteractable")]
    [SerializeField] private GrabInteractable rightGrabInteractable;

    [Header("=== 滾動偵測參數 ===")]
    [Tooltip("累積多少距離（公尺）後觸發一次事件")]
    [SerializeField] private float requiredDistance = 0.15f;

    [Tooltip("移動速度低於此值時不計入累積（過濾靜止抖動）")]
    [SerializeField] private float minSpeedThreshold = 0.05f;

    [Tooltip("吸附後忽略偵測的緩衝時間（秒），避免吸附瞬間誤觸發")]
    [SerializeField] private float snapIgnoreDuration = 0.2f;

    [Header("=== 事件 ===")]
    public UnityEvent OnRollDetected;

    // 狀態
    private bool isGrabbed = false;
    private Vector3 previousPosition;
    private float accumulatedDistance = 0f;
    private float snapIgnoreTimer = 0f;

    // =====================
    //   Unity 生命週期
    // =====================

    void Start()
    {
        // 左右兩側都監聽，任一手抓住就算
        if (leftGrabInteractable != null)
        {
            leftGrabInteractable.WhenSelectingInteractorAdded.Action += OnGrabbed;
            leftGrabInteractable.WhenSelectingInteractorRemoved.Action += OnReleased;
        }

        if (rightGrabInteractable != null)
        {
            rightGrabInteractable.WhenSelectingInteractorAdded.Action += OnGrabbed;
            rightGrabInteractable.WhenSelectingInteractorRemoved.Action += OnReleased;
        }

        previousPosition = GetTrackingPosition();
    }

    void Update()
    {
        if (!isGrabbed) return;

        CalculateRolling();
    }

    void OnDestroy()
    {
        if (leftGrabInteractable != null)
        {
            leftGrabInteractable.WhenSelectingInteractorAdded.Action -= OnGrabbed;
            leftGrabInteractable.WhenSelectingInteractorRemoved.Action -= OnReleased;
        }

        if (rightGrabInteractable != null)
        {
            rightGrabInteractable.WhenSelectingInteractorAdded.Action -= OnGrabbed;
            rightGrabInteractable.WhenSelectingInteractorRemoved.Action -= OnReleased;
        }
    }

    // =====================
    //   Grab 事件
    // =====================

    private void OnGrabbed(GrabInteractor interactor)
    {
        isGrabbed = true;
        accumulatedDistance = 0f;
        snapIgnoreTimer = snapIgnoreDuration; // 開始緩衝倒數
        previousPosition = GetTrackingPosition();
    }

    private void OnReleased(GrabInteractor interactor)
    {
        // 兩手都放開才算結束
        bool leftHeld = leftGrabInteractable != null
            && leftGrabInteractable.SelectingInteractorCount > 0;
        bool rightHeld = rightGrabInteractable != null
            && rightGrabInteractable.SelectingInteractorCount > 0;

        if (!leftHeld && !rightHeld)
        {
            isGrabbed = false;
            accumulatedDistance = 0f;
        }
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
            previousPosition = GetTrackingPosition();
            return;
        }

        Vector3 currentPosition = GetTrackingPosition();
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

    // =====================
    //   工具方法
    // =====================

    /// <summary>
    /// 追蹤物件本身的中心點位置
    /// </summary>
    private Vector3 GetTrackingPosition()
    {
        return transform.position;
    }
}
