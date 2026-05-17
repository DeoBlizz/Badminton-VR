using UnityEngine;

public class FloatingRacket : MonoBehaviour
{
    public OVRInput.Controller controller;
    public OVRInput.Button recallButton = OVRInput.Button.PrimaryIndexTrigger;
    public Transform floatTarget;
    public float floatSpeed = 5f;
    public OVRGrabber leftGrabber;
    public OVRGrabber rightGrabber;
    public Transform playerHead; // assign CenterEyeAnchor

    private Rigidbody _rb;
    private bool _isFloating = false;

    bool IsHeld => (leftGrabber != null && leftGrabber.grabbedObject != null)
                || (rightGrabber != null && rightGrabber.grabbedObject != null);

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (OVRInput.GetDown(recallButton, controller))
        {
            if (!IsHeld)
            {
                _isFloating = true;
            }
        }

        if (IsHeld)
        {
            _isFloating = false;
        }
    }

    void FixedUpdate()
    {
        if (_isFloating)
        {
            Vector3 targetPos = floatTarget.position;
            float distance = Vector3.Distance(transform.position, targetPos);

            // Move toward float target
            _rb.linearVelocity = (targetPos - transform.position) * floatSpeed;

            // Orient flat facing player
            if (playerHead != null)
            {
                Vector3 dirToPlayer = (playerHead.position - transform.position).normalized;
                dirToPlayer.y = 0; // keep it level
                
                if (dirToPlayer != Vector3.zero)
                {
                    Quaternion targetRot = Quaternion.LookRotation(dirToPlayer, Vector3.up);
                    // Rotate 90 degrees so face is flat toward player
                    targetRot *= Quaternion.Euler(90, 0, 0);
                    
                    Quaternion rotDiff = targetRot * Quaternion.Inverse(transform.rotation);
                    rotDiff.ToAngleAxis(out float angle, out Vector3 axis);
                    if (angle > 180f) angle -= 360f;
                    
                    if (axis.magnitude > 0.01f)
                    {
                        _rb.angularVelocity = axis * angle * Mathf.Deg2Rad * 5f;
                    }
                }
            }
            else
            {
                _rb.angularVelocity = Vector3.Lerp(
                    _rb.angularVelocity, Vector3.zero, 0.3f);
            }
        }
    }
}