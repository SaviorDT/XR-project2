using UnityEngine;

[ExecuteInEditMode]
public class LiquidScroller : MonoBehaviour
{
    [Header("捲動速度")]
    public float scrollSpeedX = 0.1f;
    public float scrollSpeedY = 0.5f;

    [Header("泡泡比例校正 (手動調到圓為止)")]
    public float bubbleDensityX = 15f;
    public float bubbleDensityY = 1f;

    private Renderer rend;
    private MaterialPropertyBlock propBlock;

    void Update()
    {
        if (rend == null) rend = GetComponent<Renderer>();
        if (rend == null) return;

        // --- 核心修正：使用 MaterialPropertyBlock ---
        // 這可以繞過 Animator 或其他系統對材質球實例的鎖定
        if (propBlock == null) propBlock = new MaterialPropertyBlock();
        rend.GetPropertyBlock(propBlock);

        // 根據 Z 軸長度動態計算重複次數
        // 15 是你說 X 軸正常的基準，Scale.z * 1 是讓 Y 軸隨長度增加次數
        Vector4 tilingOffset = new Vector4(
            bubbleDensityX,
            transform.localScale.z * bubbleDensityY,
            0, 0
        );

        // 如果在播放模式，加入捲動效果
        if (Application.isPlaying)
        {
            tilingOffset.z = Time.time * scrollSpeedX;
            tilingOffset.w = Time.time * scrollSpeedY;
        }

        // 強制寫入材質屬性 (在 URP 中，Tiling 和 Offset 是存在 _BaseMap_ST 裡的)
        propBlock.SetVector("_BaseMap_ST", tilingOffset);
        rend.SetPropertyBlock(propBlock);
    }
}