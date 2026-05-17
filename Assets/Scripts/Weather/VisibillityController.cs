using UnityEngine;
using UnityEngine.Rendering;
public class VisibilityController : MonoBehaviour
{
    public void ApplyPreset(WeatherPreset preset, float t)
    {
        ApplyFog(preset.fogEnabled, preset.fogColor, preset.fogDensity);
        ApplyAmbient(preset.ambientColor, preset.ambientIntensity);
    }

    public void ApplyBlend(WeatherPreset from, WeatherPreset to, float t)
    {
        WeatherPreset blended = WeatherPreset.Lerp(from, to, t);
        ApplyFog(blended.fogEnabled, blended.fogColor, blended.fogDensity);
        ApplyAmbient(blended.ambientColor, blended.ambientIntensity);
    }

    private void ApplyFog(bool enabled, Color color, float density)
    {
        RenderSettings.fog      = enabled;
        RenderSettings.fogColor = color;
        RenderSettings.fogMode  = FogMode.Linear;
        RenderSettings.fogStartDistance = Mathf.Lerp(80f, 5f,  density);
        RenderSettings.fogEndDistance   = Mathf.Lerp(200f, 30f, density);
    }

    private void ApplyAmbient(Color color, float intensity)
    {
        RenderSettings.ambientMode  = AmbientMode.Flat;
        RenderSettings.ambientLight = color * intensity;
    }
}