using UnityEngine;

public class WindController : MonoBehaviour
{
    [Header("Shuttlecock")]
    [Tooltip("Leave null to auto-find by tag 'Shuttlecock' at runtime.")]
    [SerializeField] private Rigidbody shuttlecockRigidbody;
    [SerializeField] private string shuttlecockTag = "Shuttlecock";

    [Header("Physics Tuning")]
    [Tooltip("Scales how strongly wind force is felt relative to the shuttle mass.")]
    [Range(0f, 2f)]
    [SerializeField] private float shuttlecockDragCoefficient = 0.8f;

    [Header("Turbulence Noise")]
    [SerializeField] private float turbulenceScale = 0.7f;
    [SerializeField] private float turbulenceSpeed = 0.4f;
    private float _noiseOffset;

    // Live values set by WeatherManager via ApplyPreset / ApplyBlend
    private Vector3 _windDirection  = Vector3.forward;
    private float   _windStrength   = 0f;
    private float   _turbulence     = 0f;
    private float   _rainDownwardForce  = 0f;
    private float   _rainDragMultiplier = 1f;

    // Base drag cached from the Rigidbody — restored when rain clears
    private float _baseDrag = -1f;

    public Vector3 CurrentWindVelocity { get; private set; }

    private void Awake()
    {
        _noiseOffset = Random.Range(0f, 100f);
    }

    private void Start()
    {
        if (shuttlecockRigidbody == null)
            TryFindShuttlecock();
    }

    private void FixedUpdate()
    {
        if (shuttlecockRigidbody == null)
        {
            TryFindShuttlecock();
            return;
        }

        float mass = shuttlecockRigidbody.mass;

        CurrentWindVelocity = CalculateWindVelocity();

        if (_windStrength > 0.01f)
        {
            Vector3 force = CurrentWindVelocity * shuttlecockDragCoefficient * mass;
            shuttlecockRigidbody.AddForce(force, ForceMode.Force);
        }

        if (_rainDownwardForce > 0.01f)
        {
            // Push the shuttle downward — heavier air from rainfall
            shuttlecockRigidbody.AddForce(Vector3.down * _rainDownwardForce * mass, ForceMode.Force);

            // Increase drag so shots fall shorter and feel heavier in the wet
            if (_baseDrag >= 0f)
                shuttlecockRigidbody.linearDamping = _baseDrag * _rainDragMultiplier;
        }
        else if (_baseDrag >= 0f)
        {
            // Rain cleared — restore original drag
            shuttlecockRigidbody.linearDamping = _baseDrag;
        }
    }

    public void ApplyPreset(WeatherPreset preset, float t)
    {
        _windDirection       = preset.windDirection;
        _windStrength        = preset.windStrength;
        _turbulence          = preset.windTurbulence;
        _rainDownwardForce   = preset.rainDownwardForce;
        _rainDragMultiplier  = preset.rainDragMultiplier;
    }

    public void ApplyBlend(WeatherPreset from, WeatherPreset to, float t)
    {
        WeatherPreset blended = WeatherPreset.Lerp(from, to, t);
        _windDirection       = blended.windDirection;
        _windStrength        = blended.windStrength;
        _turbulence          = blended.windTurbulence;
        _rainDownwardForce   = blended.rainDownwardForce;
        _rainDragMultiplier  = blended.rainDragMultiplier;
    }

    public void RegisterShuttlecock(Rigidbody rb)
    {
        shuttlecockRigidbody = rb;
        _baseDrag = rb != null ? rb.linearDamping : -1f;
    }

    private Vector3 CalculateWindVelocity()
    {
        float time = Time.time * turbulenceSpeed + _noiseOffset;
        float nx = (Mathf.PerlinNoise(time,        0.3f) - 0.5f) * 2f;
        float ny = (Mathf.PerlinNoise(0.7f,        time) - 0.5f) * 2f;
        float nz = (Mathf.PerlinNoise(time + 3.1f, 1.4f) - 0.5f) * 2f;
        Vector3 noise    = new Vector3(nx, ny * 0.3f, nz) * turbulenceScale;
        Vector3 baseWind = _windDirection * _windStrength;
        Vector3 gustWind = noise * _windStrength * _turbulence;
        return baseWind + gustWind;
    }

    private void TryFindShuttlecock()
    {
        GameObject obj = GameObject.FindWithTag(shuttlecockTag);
        if (obj != null)
        {
            shuttlecockRigidbody = obj.GetComponent<Rigidbody>();
            _baseDrag = shuttlecockRigidbody != null ? shuttlecockRigidbody.linearDamping : -1f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, CurrentWindVelocity);
    }
}