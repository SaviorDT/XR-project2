using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [Header("Input Effect Particles")]
    [SerializeField] private ParticleSystem perfectInputParticle;
    [SerializeField] private ParticleSystem goodInputParticle;
    [SerializeField] private ParticleSystem missInputParticle;

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
        if (perfectInputParticle != null)
        {
            perfectInputParticle.Play();
        }
    }

    private void HandleGoodInputEffect()
    {
        if (goodInputParticle != null)
        {
            goodInputParticle.Play();
        }
    }

    private void HandleMissInputEffect()
    {
        if (missInputParticle != null)
        {
            missInputParticle.Play();
        }
    }
}
