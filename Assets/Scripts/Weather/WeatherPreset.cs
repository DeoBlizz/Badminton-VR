using UnityEngine;
[System.Serializable]
public class WeatherPreset
{
    [Header("Rain")]
    [Range(0f, 1f)] public float rainIntensity = 0f;   // 0 = off, 1 = heavy
    public float rainParticleRate  = 0f;                // particles / second
    public float rainDropSize      = 0.05f;

    [Header("Rain Physics (Shuttlecock)")]
    [Tooltip("Extra downward force on the shuttlecock. 0 = none, ~6 = noticeable.")]
    public float rainDownwardForce  = 0f;
    [Tooltip("Multiplier on the shuttlecock base drag. 1 = no change, 2.5 = shorter shots.")]
    public float rainDragMultiplier = 1f;

    [Header("Wind")]
    public Vector3 windDirection   = Vector3.forward;
    [Range(0f, 30f)] public float windStrength = 0f;    // metres/sec equivalent
    [Range(0f, 1f)]  public float windTurbulence = 0f;  // gust randomness

    [Header("Visibility / Fog")]
    public bool  fogEnabled        = false;
    public Color fogColor          = new Color(0.7f, 0.7f, 0.75f);
    [Range(0f, 1f)] public float fogDensity = 0f;       // 0 = none, 1 = very thick

    [Header("Ambient Lighting")]
    public Color ambientColor      = Color.white;
    [Range(0f, 2f)] public float ambientIntensity = 1f;

    public static WeatherPreset FromState(WeatherManager.WeatherState state)
    {
        switch (state)
        {
            case WeatherManager.WeatherState.Clear:
                return new WeatherPreset
                {
                    rainIntensity    = 0f,
                    rainParticleRate = 0f,
                    rainDropSize     = 0.04f,
                    rainDownwardForce  = 0f,
                    rainDragMultiplier = 1f,
                    windDirection    = new Vector3(1f, 0f, 0.3f).normalized,
                    windStrength     = 1f,
                    windTurbulence   = 0.05f,
                    fogEnabled       = false,
                    fogDensity       = 0f,
                    ambientColor     = new Color(1f, 0.97f, 0.9f),
                    ambientIntensity = 1.2f,
                };

            case WeatherManager.WeatherState.Windy:
                return new WeatherPreset
                {
                    rainIntensity    = 0f,
                    rainParticleRate = 0f,
                    rainDropSize     = 0.04f,
                    rainDownwardForce  = 0f,
                    rainDragMultiplier = 1f,
                    windDirection    = new Vector3(1f, 0f, 0.5f).normalized,
                    windStrength     = 12f,
                    windTurbulence   = 0.4f,
                    fogEnabled       = false,
                    fogDensity       = 0f,
                    ambientColor     = new Color(0.9f, 0.92f, 1f),
                    ambientIntensity = 1.0f,
                };

            case WeatherManager.WeatherState.Rainy:
                return new WeatherPreset
                {
                    rainIntensity    = 0.6f,
                    rainParticleRate = 800f,
                    rainDropSize     = 0.05f,
                    rainDownwardForce  = 5f,
                    rainDragMultiplier = 2f,
                    windDirection    = new Vector3(0.8f, -0.1f, 0.6f).normalized,
                    windStrength     = 6f,
                    windTurbulence   = 0.2f,
                    fogEnabled       = true,
                    fogColor         = new Color(0.65f, 0.68f, 0.72f),
                    fogDensity       = 0.35f,
                    ambientColor     = new Color(0.6f, 0.65f, 0.75f),
                    ambientIntensity = 0.7f,
                };

            default:
                return new WeatherPreset();
        }
    }

    public static WeatherPreset Lerp(WeatherPreset a, WeatherPreset b, float t)
    {
        return new WeatherPreset
        {
            rainIntensity    = Mathf.Lerp(a.rainIntensity,    b.rainIntensity,    t),
            rainParticleRate = Mathf.Lerp(a.rainParticleRate, b.rainParticleRate, t),
            rainDropSize       = Mathf.Lerp(a.rainDropSize,       b.rainDropSize,       t),
            rainDownwardForce  = Mathf.Lerp(a.rainDownwardForce,  b.rainDownwardForce,  t),
            rainDragMultiplier = Mathf.Lerp(a.rainDragMultiplier, b.rainDragMultiplier, t),
            windDirection    = Vector3.Slerp(a.windDirection,  b.windDirection,    t).normalized,
            windStrength     = Mathf.Lerp(a.windStrength,     b.windStrength,     t),
            windTurbulence   = Mathf.Lerp(a.windTurbulence,   b.windTurbulence,   t),
            fogEnabled       = t >= 0.5f ? b.fogEnabled : a.fogEnabled,
            fogColor         = Color.Lerp(a.fogColor,          b.fogColor,         t),
            fogDensity       = Mathf.Lerp(a.fogDensity,       b.fogDensity,       t),
            ambientColor     = Color.Lerp(a.ambientColor,      b.ambientColor,     t),
            ambientIntensity = Mathf.Lerp(a.ambientIntensity, b.ambientIntensity, t),
        };
    }
}