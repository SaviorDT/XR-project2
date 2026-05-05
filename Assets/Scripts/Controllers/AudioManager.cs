using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Input Effect Audio")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip[] perfectInputClips;
    [SerializeField] private AudioClip goodInputClip;
    [SerializeField] private AudioClip missInputClip;

    private int _perfectStreak;

    private void OnEnable()
    {
        AnimationController.Instance.PerfectInputEffectRequested += HandlePerfectInputEffect;
        AnimationController.Instance.GoodInputEffectRequested += HandleGoodInputEffect;
        AnimationController.Instance.MissInputEffectRequested += HandleMissInputEffect;
    }

    private void OnDisable()
    {
        AnimationController.Instance.PerfectInputEffectRequested -= HandlePerfectInputEffect;
        AnimationController.Instance.GoodInputEffectRequested -= HandleGoodInputEffect;
        AnimationController.Instance.MissInputEffectRequested -= HandleMissInputEffect;
    }

    private void HandlePerfectInputEffect()
    {
        _perfectStreak++;
        PlayPerfectByStreak(_perfectStreak);
    }

    private void HandleGoodInputEffect()
    {
        _perfectStreak = 0;
        PlayOneShot(goodInputClip);
    }

    private void HandleMissInputEffect()
    {
        _perfectStreak = 0;
        PlayOneShot(missInputClip);
    }

    private void PlayPerfectByStreak(int streak)
    {
        if (sfxSource == null || perfectInputClips == null || perfectInputClips.Length == 0)
        {
            return;
        }

        int index = Mathf.Clamp(streak - 1, 0, perfectInputClips.Length - 1);
        AudioClip clip = perfectInputClips[index];
        if (clip == null)
        {
            return;
        }

        sfxSource.PlayOneShot(clip);
    }

    private void PlayOneShot(AudioClip clip)
    {
        if (sfxSource == null || clip == null)
        {
            return;
        }

        sfxSource.PlayOneShot(clip);
    }
}
