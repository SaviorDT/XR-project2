using UnityEngine;

public class HatScaleController : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private Collider targetCollider;

    private bool isScaling;
    private float scaleDuration;
    private float scaleElapsed;
    private float targetScaleY;
    private float localHeight;
    private Vector3 bottomWorld;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isScaling)
        {
            return;
        }

        scaleElapsed += Time.deltaTime;
        float t = scaleDuration <= 0f ? 1f : Mathf.Clamp01(scaleElapsed / scaleDuration);
        float newY = Mathf.Lerp(0f, targetScaleY, t);

        Vector3 scale = transform.localScale;
        scale.y = newY;
        transform.localScale = scale;
        UpdatePositionToKeepBottom(bottomWorld, localHeight, newY);

        Debug.Log($"Scaling hat: elapsed={scaleElapsed:F2}s, t={t:F2}, newY={newY:F2}");

        if (t >= 1f)
        {
            isScaling = false;
        }
    }

    public void SetScale(double scale, double duration)
    {
        targetScaleY = (float)scale;
        scaleDuration = (float)duration;
        scaleElapsed = 0f;

        localHeight = GetLocalHeight();
        bottomWorld = GetBottomWorldPosition(localHeight, transform.localScale.y);

        Vector3 startScale = transform.localScale;
        startScale.y = 0f;
        transform.localScale = startScale;
        UpdatePositionToKeepBottom(bottomWorld, localHeight, 0f);

        Debug.Log($"Starting hat scale animation: targetScaleY={targetScaleY}, duration={scaleDuration}s, localHeight={localHeight}");

        isScaling = true;
    }

    private float GetLocalHeight()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponentInChildren<Renderer>();
        }

        if (targetRenderer != null)
        {
            float lossyY = Mathf.Abs(transform.lossyScale.y);
            return lossyY > 0f ? targetRenderer.bounds.size.y / lossyY : targetRenderer.bounds.size.y;
        }

        if (targetCollider == null)
        {
            targetCollider = GetComponentInChildren<Collider>();
        }

        if (targetCollider != null)
        {
            float lossyY = Mathf.Abs(transform.lossyScale.y);
            return lossyY > 0f ? targetCollider.bounds.size.y / lossyY : targetCollider.bounds.size.y;
        }

        return 1f;
    }

    private Vector3 GetBottomWorldPosition(float localHeight, float currentScaleY)
    {
        float halfHeightWorld = localHeight * currentScaleY * 0.5f;
        return transform.position - transform.up * halfHeightWorld;
    }

    private void UpdatePositionToKeepBottom(Vector3 bottomWorld, float localHeight, float currentScaleY)
    {
        float halfHeightWorld = localHeight * currentScaleY * 0.5f;
        transform.position = bottomWorld + transform.up * halfHeightWorld;
    }
}
