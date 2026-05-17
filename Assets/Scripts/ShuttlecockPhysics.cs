using UnityEngine;

public class ShuttlecockPhysics : MonoBehaviour
{
    private Rigidbody _rb;
    public float dragCoefficient = 0.3f;
    public float orientSpeed = 3f;
    private bool _active = false;

    // Assign in Inspector — drag the head child here
    public Transform head;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.angularDamping = 5f;
        Invoke(nameof(Activate), 0.3f);
    }

    void Activate() => _active = true;

    void FixedUpdate()
    {
        if (!_active) return;

        float speed = _rb.linearVelocity.magnitude;

        if (speed > 0.5f)
        {
            // Apply drag
            _rb.AddForce(-_rb.linearVelocity * dragCoefficient);

            // Orient head in direction of travel using torque
            // Get direction from head to velocity
            Vector3 velocityDir = _rb.linearVelocity.normalized;

            // Find what direction the head currently points
            Vector3 headDir = head != null
                ? (head.position - transform.position).normalized
                : transform.forward;

            // Apply torque to align head with velocity
            Vector3 torque = Vector3.Cross(headDir, velocityDir);
            _rb.AddTorque(torque * orientSpeed * speed);
        }
        else
        {
            // Kill rotation when slow
            _rb.angularVelocity *= 0.8f;
        }
    }
}