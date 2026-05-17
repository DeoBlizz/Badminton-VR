using UnityEngine;

public class RainController : MonoBehaviour
{
    [Header("Particle System")]
    [SerializeField] private ParticleSystem rainParticleSystem;
    [Header("Splash Particles (optional)")]
    [SerializeField] private ParticleSystem splashParticleSystem;

    private ParticleSystem.EmissionModule _emission;
    private ParticleSystem.MainModule     _main;
    private bool _initialised;

    private void Awake() => Initialise();

    private void Initialise()
    {
        if (_initialised || rainParticleSystem == null) return;
        _emission = rainParticleSystem.emission;
        _main     = rainParticleSystem.main;
        _initialised = true;
    }

    public void ApplyPreset(WeatherPreset preset, float t)
    {
        ApplyValues(preset.rainIntensity, preset.rainParticleRate, preset.rainDropSize);
    }

    public void ApplyBlend(WeatherPreset from, WeatherPreset to, float t)
    {
        WeatherPreset blended = WeatherPreset.Lerp(from, to, t);
        ApplyValues(blended.rainIntensity, blended.rainParticleRate, blended.rainDropSize);
    }

    private void ApplyValues(float intensity, float particleRate, float dropSize)
    {
        Initialise();
        if (rainParticleSystem == null) return;

        bool shouldPlay = intensity > 0.01f;

        _emission.rateOverTime = particleRate;
        _main.startSize        = dropSize;

        if (shouldPlay && !rainParticleSystem.isPlaying)
            rainParticleSystem.Play();
        else if (!shouldPlay && rainParticleSystem.isPlaying)
            rainParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        // Splash particles scale with main intensity
        if (splashParticleSystem != null)
        {
            var splashEmission = splashParticleSystem.emission;
            splashEmission.rateOverTime = particleRate * 0.15f;
            if (shouldPlay && !splashParticleSystem.isPlaying)
                splashParticleSystem.Play();
            else if (!shouldPlay && splashParticleSystem.isPlaying)
                splashParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

    }
}