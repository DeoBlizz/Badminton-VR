using System;
using System.Collections;
using UnityEngine;
public class WeatherManager : MonoBehaviour
{
    public static WeatherManager Instance { get; private set; }
    public enum WeatherState { Clear, Windy, Rainy }
    [Header("Sub-Controllers")]
    [SerializeField] private RainController rainController;
    [SerializeField] private WindController windController;
    [SerializeField] private VisibilityController visibilityController;

    [Header("Transition")]
    [Tooltip("Default blend duration in seconds when no override is supplied.")]
    [SerializeField] private float defaultTransitionDuration = 4f;

    [Header("Dynamic Match Weather (optional)")]
    [Tooltip("Enable to randomly change weather during a match.")]
    [SerializeField] private bool enableDynamicWeather = false;
    [SerializeField] private float minWeatherDuration = 30f;
    [SerializeField] private float maxWeatherDuration = 90f;
    public WeatherState CurrentWeather { get; private set; } = WeatherState.Clear;
    public WeatherState PreviousWeather { get; private set; } = WeatherState.Clear;
    public event Action<WeatherState> OnWeatherChanged;

    private Coroutine _transitionCoroutine;
    private Coroutine _dynamicCoroutine;
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        ApplyWeatherImmediate(CurrentWeather);
        if (enableDynamicWeather)
            _dynamicCoroutine = StartCoroutine(DynamicWeatherRoutine());
    }

    public void SetWeather(WeatherState newState, float duration = -1f)
    {
        if (newState == CurrentWeather) return;

        float blendTime = duration < 0f ? defaultTransitionDuration : duration;

        if (_transitionCoroutine != null)
            StopCoroutine(_transitionCoroutine);

        _transitionCoroutine = StartCoroutine(TransitionRoutine(newState, blendTime));
    }

    public void SetWeatherImmediate(WeatherState newState)
    {
        if (_transitionCoroutine != null) StopCoroutine(_transitionCoroutine);
        ApplyWeatherImmediate(newState);
    }

    public void SetDynamicWeather(bool enabled)
    {
        enableDynamicWeather = enabled;
        if (enabled && _dynamicCoroutine == null)
            _dynamicCoroutine = StartCoroutine(DynamicWeatherRoutine());
        else if (!enabled && _dynamicCoroutine != null)
        {
            StopCoroutine(_dynamicCoroutine);
            _dynamicCoroutine = null;
        }
    }
    public void ToggleRain()
    {
    if (CurrentWeather == WeatherState.Rainy)
        SetWeather(WeatherState.Clear);
    else
        SetWeather(WeatherState.Rainy);
    }

    public void ToggleWind()
    {
    if (CurrentWeather == WeatherState.Windy)
        SetWeather(WeatherState.Clear);
    else
        SetWeather(WeatherState.Windy);
    }

// Optional generic UI method
public void SetWeatherFromButton(int stateIndex)
{
    SetWeather((WeatherState)stateIndex);
}

    private IEnumerator TransitionRoutine(WeatherState target, float duration)
    {
        WeatherPreset from = WeatherPreset.FromState(CurrentWeather);
        WeatherPreset to   = WeatherPreset.FromState(target);

        PreviousWeather = CurrentWeather;
        CurrentWeather  = target;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            ApplyBlend(from, to, t);
            yield return null;
        }

        ApplyWeatherImmediate(target);
        OnWeatherChanged?.Invoke(CurrentWeather);
    }

    private void ApplyWeatherImmediate(WeatherState state)
    {
        CurrentWeather = state;
        WeatherPreset p = WeatherPreset.FromState(state);
        rainController?.ApplyPreset(p, 1f);
        windController?.ApplyPreset(p, 1f);
        visibilityController?.ApplyPreset(p, 1f);
    }

    private void ApplyBlend(WeatherPreset from, WeatherPreset to, float t)
    {
        rainController?.ApplyBlend(from, to, t);
        windController?.ApplyBlend(from, to, t);
        visibilityController?.ApplyBlend(from, to, t);
    }

    private IEnumerator DynamicWeatherRoutine()
    {
        while (true)
        {
            float wait = UnityEngine.Random.Range(minWeatherDuration, maxWeatherDuration);
            yield return new WaitForSeconds(wait);

            WeatherState next = (WeatherState)UnityEngine.Random.Range(0, Enum.GetValues(typeof(WeatherState)).Length);
            SetWeather(next);
        }
    }
}