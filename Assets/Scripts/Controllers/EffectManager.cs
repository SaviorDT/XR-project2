using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [Header("Input Effect Particles")]
    [SerializeField] private ParticleSystem perfectInputParticle;
    [SerializeField] private ParticleSystem goodInputParticle;
    [SerializeField] private ParticleSystem missInputParticle;
    [SerializeField] private GameObject ovenFireParticle;

    private void OnEnable()
    {
        AnimationController.Instance.PerfectInputEffectRequested += HandlePerfectInputEffect;
        AnimationController.Instance.GoodInputEffectRequested += HandleGoodInputEffect;
        AnimationController.Instance.MissInputEffectRequested += HandleMissInputEffect;
        AnimationController.Instance.OvenFireEffectRequested += PlayOvenFireEffect;
    }

    private void OnDisable()
    {
        AnimationController.Instance.PerfectInputEffectRequested -= HandlePerfectInputEffect;
        AnimationController.Instance.GoodInputEffectRequested -= HandleGoodInputEffect;
        AnimationController.Instance.MissInputEffectRequested -= HandleMissInputEffect;
        AnimationController.Instance.OvenFireEffectRequested -= PlayOvenFireEffect;
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

    public void PlayOvenFireEffect()
    {
        if (ovenFireParticle != null)
        {
            ovenFireParticle.SetActive(true);
        }
    }
}
