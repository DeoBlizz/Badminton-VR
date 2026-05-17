using UnityEngine;

public class ShuttlecockHit : MonoBehaviour
{
    private Rigidbody _rb;
    public float rimForceMultiplier = 1.5f;
    public float netForceMultiplier = 3f;
    public float minHitForce = 8f;
    private float _hitCooldown = 0.2f;
    private Vector3 _racketVelocity;
    private Vector3 _racketLastLocalPos;
    private Transform _racketTransform;
    private Transform _playerTransform;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();

        GameObject racket = GameObject.FindWithTag("Racket");
        if (racket != null)
            _racketTransform = racket.transform;

        GameObject player = GameObject.Find("OVRPlayerController");
        if (player != null)
        {
            _playerTransform = player.transform;
            if (_racketTransform != null)
                _racketLastLocalPos = _playerTransform.InverseTransformPoint(
                    _racketTransform.position);
        }
    }

    void Update()
    {
        if (_racketTransform == null || _playerTransform == null) return;

        Vector3 currentLocalPos = _playerTransform.InverseTransformPoint(
            _racketTransform.position);
        _racketVelocity = (currentLocalPos - _racketLastLocalPos) / Time.deltaTime;
        _racketLastLocalPos = currentLocalPos;
    }

    void FixedUpdate()
    {
        _hitCooldown -= Time.fixedDeltaTime;
        CheckOverlap();
    }

    void CheckOverlap()
    {
        if (_hitCooldown > 0) return;

        Collider[] overlaps = Physics.OverlapSphere(transform.position, 0.03f);
        foreach (Collider col in overlaps)
        {
            if (col.CompareTag("Racket"))
            {
                ApplyForce(col.gameObject.name);
                return;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (_hitCooldown > 0) return;
        if (!collision.collider.gameObject.CompareTag("Racket")) return;
        ApplyForce(collision.collider.gameObject.name);
    }

    void ApplyForce(string hitObjectName)
    {
        Vector3 worldVelocity = _playerTransform != null
            ? _playerTransform.TransformDirection(_racketVelocity)
            : _racketVelocity;

        float hitSpeed = worldVelocity.magnitude;
        if (hitSpeed < 0.5f) return;

        bool isNetHit = hitObjectName.ToLower().Contains("net");
        float force = Mathf.Max(
            hitSpeed * (isNetHit ? netForceMultiplier : rimForceMultiplier),
            minHitForce);

        _rb.linearVelocity = Vector3.zero;
        _rb.AddForce(worldVelocity.normalized * force, ForceMode.Impulse);
        _hitCooldown = 0.2f;

        Debug.Log($"Hit: {(isNetHit ? "Net" : "Rim")} Speed: {hitSpeed} Force: {force}");
    }
}