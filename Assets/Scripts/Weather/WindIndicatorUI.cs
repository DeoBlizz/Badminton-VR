using TMPro;
using UnityEngine;

public class WindIndicatorUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI windLabel;
    [SerializeField] private Transform       arrowTransform;   // rotates to show direction
    [SerializeField] private Transform       cameraTransform;  // assign XR camera rig

    [Header("Update Rate")]
    [SerializeField] private float updateInterval = 0.2f;      // seconds between refreshes
    private float _timer;

    private WindController _wind;

    private void Start()
    {
        _wind = FindFirstObjectByType<WindController>();
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer < updateInterval) return;
        _timer = 0f;
        Refresh();
    }

    private void Refresh()
    {
        if (_wind == null) return;

        Vector3 vel = _wind.CurrentWindVelocity;
        float   speed = vel.magnitude;

        // Update label
        if (windLabel != null)
        {
            string beaufort = SpeedToBeaufort(speed);
            windLabel.text = $"<b>WIND</b>\n{speed:F1} m/s\n{beaufort}";
        }

        // Rotate arrow to match horizontal wind direction
        if (arrowTransform != null && speed > 0.1f)
        {
            Vector3 flat = new Vector3(vel.x, 0f, vel.z).normalized;
            if (flat.sqrMagnitude > 0.001f)
                arrowTransform.rotation = Quaternion.LookRotation(flat, Vector3.up);
        }

        // Billboard: face the VR camera
        if (cameraTransform != null)
        {
            Vector3 toCamera = cameraTransform.position - transform.position;
            toCamera.y = 0f;
            if (toCamera.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(-toCamera, Vector3.up);
        }
    }

    private static string SpeedToBeaufort(float mps)
    {
        if (mps < 0.5f)  return "Calm";
        if (mps < 3.4f)  return "Light Breeze";
        if (mps < 7.9f)  return "Gentle Breeze";
        if (mps < 13.9f) return "Moderate Breeze";
        if (mps < 20.7f) return "Fresh Breeze";
        return "Strong Wind";
    }
}