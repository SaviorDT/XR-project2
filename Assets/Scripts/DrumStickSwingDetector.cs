using UnityEngine;
using UnityEngine.Events;
using Oculus.Interaction;

public class DrumStickSwingDetector : MonoBehaviour
{
    [Header("=== Grab 設定 ===")]
    [SerializeField] private GrabInteractable grabInteractable;

    [Header("=== 揮動偵測參數 ===")]
    [SerializeField] private float swingThreshold = 2.0f;
    [SerializeField] private float downwardAngleThreshold = 0.5f;
    [SerializeField] private float cooldownTime = 0.3f;
    [SerializeField] private float swingEndSpeedRatio = 0.5f;

    [Header("=== 事件 ===")]
    public UnityEvent OnSwingDetected;

    private bool isGrabbed = false;
    private bool swingInProgress = false;
    private Vector3 previousPosition;
    private Vector3 currentVelocity;
    private float lastSwingTime = -999f;

    // =====================
    //   Unity 生命週期
    // =====================

    void Start()
    {
        if (grabInteractable == null)
            grabInteractable = GetComponent<GrabInteractable>();

        grabInteractable.WhenSelectingInteractorAdded.Action += OnGrabbed;
        grabInteractable.WhenSelectingInteractorRemoved.Action += OnReleased;

        previousPosition = transform.position;
    }

    void Update()
    {
        if (!isGrabbed) return;

        CalculateVelocity();
        DetectDownSwing();
    }

    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.WhenSelectingInteractorAdded.Action -= OnGrabbed;
            grabInteractable.WhenSelectingInteractorRemoved.Action -= OnReleased;
        }
    }

    // =====================
    //   Grab 事件
    // =====================

    private void OnGrabbed(GrabInteractor interactor)
    {
        isGrabbed = true;
        previousPosition = transform.position;
        ResetSwingState();
    }

    private void OnReleased(GrabInteractor interactor)
    {
        isGrabbed = false;
        ResetSwingState();
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
