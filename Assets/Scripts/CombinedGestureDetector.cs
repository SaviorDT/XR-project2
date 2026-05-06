using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 合併偵測器：整合 DrumStick（下揮）、PizzaThrow（左投）、RollingPin（滾動）、Sprinkle（橫向揮動）
/// - 初始化即開始偵測（isDetecting 預設 true）
/// - 單次 Update 可同時觸發多種事件（四條偵測邏輯互相獨立）
/// - 每種事件各自回傳，不互相干擾
/// </summary>
public class CombinedGestureDetector : MonoBehaviour
{
    // =========================================================
    //   DrumStick — 下揮偵測
    // =========================================================
    [Header("=== DrumStick 下揮偵測 ===")]
    [Tooltip("觸發下揮所需的最低速度（建議 1.5~3.0）")]
    [SerializeField] private float drumSwingThreshold = 2.2f;

    [Tooltip("下揮方向判定（1.0 是正下方，0.3~0.5 比較寬鬆）")]
    [SerializeField] private float drumDownwardAngleThreshold = 0.4f;

    [Tooltip("兩次下揮之間的最短間隔（秒）")]
    [SerializeField] private float drumCooldownTime = 0.2f;

    [Range(0, 1)]
    [Tooltip("速度平滑係數（DrumStick）")]
    [SerializeField] private float drumVelocitySmoothing = 0.7f;

    [SerializeField] private TempoEventType drumEventType = TempoEventType.cut;
    public UnityEvent OnDrumSwingDetected;

    // 私有狀態 — DrumStick
    private bool   drumSwingProcessed = false;
    private float  drumLastSwingTime  = -999f;
    private Vector3 drumPreviousPosition;
    private Vector3 drumSmoothedVelocity;

    // =========================================================
    //   PizzaThrow — 右至左橫向投擲偵測
    // =========================================================
    [Header("=== PizzaThrow 右至左投擲偵測 ===")]
    [Tooltip("觸發投擲所需的最低速度（m/s）")]
    [SerializeField] private float pizzaThrowThreshold = 2.0f;

    [Tooltip("水平速度必須比垂直速度大幾倍才算橫向揮舞")]
    [SerializeField] private float pizzaHorizontalDominance = 1.5f;

    [Tooltip("兩次投擲之間的最短間隔（秒）")]
    [SerializeField] private float pizzaCooldownTime = 0.3f;

    [Range(0, 1)]
    [Tooltip("速度平滑係數（PizzaThrow）")]
    [SerializeField] private float pizzaVelocitySmoothing = 0.7f;

    [Tooltip("拖入 [BuildingBlock] Camera Rig > TrackingSpace > CenterEyeAnchor（PizzaThrow 用）")]
    [SerializeField] private Transform pizzaCenterEyeAnchor;

    [SerializeField] private TempoEventType pizzaEventType = TempoEventType.send;
    public UnityEvent OnPizzaThrowDetected;

    // 私有狀態 — PizzaThrow
    private bool   pizzaThrowProcessed = false;
    private float  pizzaLastThrowTime  = -999f;
    private Vector3 pizzaPreviousPosition;
    private Vector3 pizzaSmoothedVelocity;

    // =========================================================
    //   RollingPin — 前後滾動偵測
    // =========================================================
    [Header("=== RollingPin 前後滾動偵測 ===")]
    [Tooltip("累積多少距離（公尺）後觸發一次滾動事件")]
    [SerializeField] private float rollingRequiredDistance = 0.15f;

    [Tooltip("移動速度低於此值時不計入累積（過濾靜止抖動）")]
    [SerializeField] private float rollingMinSpeedThreshold = 0.05f;

    [Tooltip("前後移動分量必須比左右移動分量大幾倍才計入")]
    [SerializeField] private float rollingForwardDominance = 1.5f;

    [Tooltip("開始偵測後忽略的緩衝時間（秒），避免吸附瞬間誤觸發")]
    [SerializeField] private float rollingSnapIgnoreDuration = 0.2f;

    [Range(0, 1)]
    [Tooltip("速度平滑係數（RollingPin）")]
    [SerializeField] private float rollingVelocitySmoothing = 0.5f;

    [SerializeField] private TempoEventType rollingEventType = TempoEventType.roll;
    public UnityEvent OnRollDetected;

    // 私有狀態 — RollingPin
    private float   rollingAccumulatedDistance = 0f;
    private float   rollingSnapIgnoreTimer     = 0f;
    private Vector3 rollingPreviousPosition;
    private Vector3 rollingSmoothedVelocity;

    // =========================================================
    //   Sprinkle — 左右橫向揮動偵測
    // =========================================================
    [Header("=== Sprinkle 橫向揮動偵測 ===")]
    [Tooltip("觸發橫向揮動所需的最低速度（m/s）")]
    [SerializeField] private float sprinkleSwingThreshold = 2.0f;

    [Tooltip("左右速度必須比前後和垂直速度大幾倍才算橫向揮動")]
    [SerializeField] private float sprinkleSidewaysDominance = 1.5f;

    [Tooltip("兩次橫向揮動之間的最短間隔（秒）")]
    [SerializeField] private float sprinkleCooldownTime = 0.3f;

    [Range(0, 1)]
    [Tooltip("速度平滑係數（Sprinkle）")]
    [SerializeField] private float sprinkleVelocitySmoothing = 0.7f;

    [Tooltip("拖入 [BuildingBlock] Camera Rig > TrackingSpace > CenterEyeAnchor（Sprinkle 用）")]
    [SerializeField] private Transform sprinkleCenterEyeAnchor;

    [SerializeField] private TempoEventType sprinkleEventType = TempoEventType.put;
    public UnityEvent OnSprinkleDetected;

    // 私有狀態 — Sprinkle
    private bool   sprinkleSwingProcessed = false;
    private float  sprinkleLastSwingTime  = -999f;
    private Vector3 sprinklePreviousPosition;
    private Vector3 sprinkleSmoothedVelocity;

    // =========================================================
    //   共用外部引用
    // =========================================================
    [Header("=== 外部引用 ===")]
    [SerializeField] private GameCore gameCore;

    // =========================================================
    //   Unity 生命週期
    // =========================================================

    public void SetGameCoreReference(GameCore core)
    {
        gameCore = core;
    }
    private void Start()
    {
        // 初始化各偵測器的位置基準
        drumPreviousPosition     = transform.position;
        pizzaPreviousPosition    = transform.position;
        rollingPreviousPosition  = transform.position;
        sprinklePreviousPosition = transform.position;

        // RollingPin 的吸附緩衝計時器
        rollingSnapIgnoreTimer = rollingSnapIgnoreDuration;

        Debug.Log("[CombinedGestureDetector] 所有偵測器已啟動");
    }

    private void Update()
    {
        // 四條偵測管線各自獨立執行，互不干擾
        // 同一 Update 可同時觸發多個事件

        UpdateDrumStick();
        UpdatePizzaThrow();
        UpdateRollingPin();
        UpdateSprinkle();
    }

    // =========================================================
    //   公開回調綁定（對應原各腳本的 SetCallback）
    // =========================================================

    public void SetDrumSwingCallback(UnityAction callback)
    {
        OnDrumSwingDetected.RemoveListener(callback);
        OnDrumSwingDetected.AddListener(callback);
    }

    public void SetPizzaThrowCallback(UnityAction callback)
    {
        OnPizzaThrowDetected.RemoveListener(callback);
        OnPizzaThrowDetected.AddListener(callback);
    }

    public void SetRollCallback(UnityAction callback)
    {
        OnRollDetected.RemoveListener(callback);
        OnRollDetected.AddListener(callback);
    }

    public void SetSprinkleCallback(UnityAction callback)
    {
        OnSprinkleDetected.RemoveListener(callback);
        OnSprinkleDetected.AddListener(callback);
    }

    // =========================================================
    //   DrumStick 內部邏輯
    // =========================================================

    private void UpdateDrumStick()
    {
        // 計算平滑速度
        Vector3 rawVelocity = (transform.position - drumPreviousPosition) / Time.deltaTime;
        drumPreviousPosition = transform.position;
        drumSmoothedVelocity = Vector3.Lerp(drumSmoothedVelocity, rawVelocity, 1f - drumVelocitySmoothing);

        float speed       = drumSmoothedVelocity.magnitude;
        float downwardDot = Vector3.Dot(drumSmoothedVelocity.normalized, Vector3.down);

        if (speed > drumSwingThreshold && downwardDot > drumDownwardAngleThreshold)
        {
            if (!drumSwingProcessed && Time.time - drumLastSwingTime > drumCooldownTime)
            {
                drumLastSwingTime   = Time.time;
                drumSwingProcessed  = true;

                OnDrumSwingDetected?.Invoke();
                gameCore?.OnInput(drumEventType);

                Debug.Log($"<color=cyan>[DrumStick]</color> 偵測到擊打！速度: {drumSmoothedVelocity.magnitude:F2}");
            }
        }
        else if (speed < drumSwingThreshold * 0.5f)
        {
            drumSwingProcessed = false;
        }
    }

    // =========================================================
    //   PizzaThrow 內部邏輯
    // =========================================================

    private void UpdatePizzaThrow()
    {
        if (pizzaCenterEyeAnchor == null) return;

        // 計算平滑速度
        Vector3 rawVelocity = (transform.position - pizzaPreviousPosition) / Time.deltaTime;
        pizzaPreviousPosition = transform.position;
        pizzaSmoothedVelocity = Vector3.Lerp(pizzaSmoothedVelocity, rawVelocity, 1f - pizzaVelocitySmoothing);

        float speed = pizzaSmoothedVelocity.magnitude;

        // 取得玩家面向的水平參考軸
        Vector3 playerForward = pizzaCenterEyeAnchor.forward;
        playerForward.y = 0f;
        playerForward.Normalize();
        Vector3 playerRight = Vector3.Cross(Vector3.up, playerForward);

        float rightwardSpeed  = Vector3.Dot(pizzaSmoothedVelocity, playerRight);
        float verticalSpeed   = Mathf.Abs(pizzaSmoothedVelocity.y);
        float horizontalSpeed = new Vector2(pizzaSmoothedVelocity.x, pizzaSmoothedVelocity.z).magnitude;

        bool isHorizontal = horizontalSpeed > verticalSpeed * pizzaHorizontalDominance;
        bool isLeftward   = rightwardSpeed < 0;

        if (speed > pizzaThrowThreshold && isHorizontal && isLeftward)
        {
            if (!pizzaThrowProcessed && Time.time - pizzaLastThrowTime > pizzaCooldownTime)
            {
                pizzaLastThrowTime   = Time.time;
                pizzaThrowProcessed  = true;

                OnPizzaThrowDetected?.Invoke();
                gameCore?.OnInput(pizzaEventType);

                Debug.Log($"<color=red>[PizzaThrow]</color> 偵測到右至左投擲！速度: {pizzaSmoothedVelocity.magnitude:F2}");
            }
        }
        else if (speed < pizzaThrowThreshold * 0.5f)
        {
            pizzaThrowProcessed = false;
        }
    }

    // =========================================================
    //   RollingPin 內部邏輯
    // =========================================================

    private void UpdateRollingPin()
    {
        // 吸附緩衝計時
        if (rollingSnapIgnoreTimer > 0f)
        {
            rollingSnapIgnoreTimer -= Time.deltaTime;
            rollingPreviousPosition = transform.position;
            return;
        }

        // 計算平滑速度
        Vector3 rawDelta    = transform.position - rollingPreviousPosition;
        Vector3 rawVelocity = rawDelta / Time.deltaTime;
        rollingSmoothedVelocity = Vector3.Lerp(rollingSmoothedVelocity, rawVelocity, 1f - rollingVelocitySmoothing);

        // 忽略 Y 軸位移
        Vector3 horizontalDelta = rawDelta;
        horizontalDelta.y = 0f;

        // 使用局部 Forward 作為滾動方向
        Vector3 rollDirection = transform.forward;
        rollDirection.y = 0f;
        rollDirection.Normalize();
        Vector3 pinAxleDirection = Vector3.Cross(Vector3.up, rollDirection);

        float forwardAmount  = Mathf.Abs(Vector3.Dot(horizontalDelta, rollDirection));
        float sidewaysAmount = Mathf.Abs(Vector3.Dot(horizontalDelta, pinAxleDirection));

        bool isForwardMovement = forwardAmount > sidewaysAmount * rollingForwardDominance;
        float currentSpeed     = rollingSmoothedVelocity.magnitude;

        if (currentSpeed > rollingMinSpeedThreshold && isForwardMovement)
        {
            rollingAccumulatedDistance += forwardAmount;
        }

        if (rollingAccumulatedDistance >= rollingRequiredDistance)
        {
            rollingAccumulatedDistance = 0f;

            OnRollDetected?.Invoke();
            gameCore?.OnInput(rollingEventType);

            Debug.Log($"<color=orange>[RollingPin]</color> 偵測到滾動！當前平滑速度: {rollingSmoothedVelocity.magnitude:F2}");
        }

        rollingPreviousPosition = transform.position;
    }

    // =========================================================
    //   Sprinkle 內部邏輯
    // =========================================================

    private void UpdateSprinkle()
    {
        if (sprinkleCenterEyeAnchor == null) return;

        // 計算平滑速度
        Vector3 rawVelocity = (transform.position - sprinklePreviousPosition) / Time.deltaTime;
        sprinklePreviousPosition = transform.position;
        sprinkleSmoothedVelocity = Vector3.Lerp(sprinkleSmoothedVelocity, rawVelocity, 1f - sprinkleVelocitySmoothing);

        float speed = sprinkleSmoothedVelocity.magnitude;

        // 取得玩家面向的水平參考軸
        Vector3 playerForward = sprinkleCenterEyeAnchor.forward;
        playerForward.y = 0f;
        playerForward.Normalize();
        Vector3 playerRight = Vector3.Cross(Vector3.up, playerForward);

        float sidewaysSpeed = Mathf.Abs(Vector3.Dot(sprinkleSmoothedVelocity, playerRight));
        float forwardSpeed  = Mathf.Abs(Vector3.Dot(sprinkleSmoothedVelocity, playerForward));
        float verticalSpeed = Mathf.Abs(sprinkleSmoothedVelocity.y);

        bool isSideways = sidewaysSpeed > forwardSpeed  * sprinkleSidewaysDominance
                       && sidewaysSpeed > verticalSpeed * sprinkleSidewaysDominance;

        if (speed > sprinkleSwingThreshold && isSideways)
        {
            if (!sprinkleSwingProcessed && Time.time - sprinkleLastSwingTime > sprinkleCooldownTime)
            {
                sprinkleLastSwingTime   = Time.time;
                sprinkleSwingProcessed  = true;

                OnSprinkleDetected?.Invoke();
                gameCore?.OnInput(sprinkleEventType);

                Debug.Log($"<color=yellow>[Sprinkle]</color> 偵測到橫向灑料！速度: {sprinkleSmoothedVelocity.magnitude:F2}");
            }
        }
        else if (speed < sprinkleSwingThreshold * 0.5f)
        {
            sprinkleSwingProcessed = false;
        }
    }
}
