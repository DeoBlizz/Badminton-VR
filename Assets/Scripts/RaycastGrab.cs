using System.Collections;
using UnityEngine;

public class RaycastGrabber : MonoBehaviour
{
    public OVRInput.Controller controller;
    public float grabRange = 5f;
    public float grabSpeed = 10f;
    public LayerMask grabbableLayer;
    public Transform gripTransform;
    public OVRGrabberToggle grabberToggle;
    private GameObject _rayGrabbedObject;
    private Rigidbody _rayGrabbedRb;
    private bool _isRayGrabbing;
    private LineRenderer _lineRenderer;

    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        if (_lineRenderer != null)
        {
            _lineRenderer.startWidth = 0.01f;
            _lineRenderer.endWidth = 0.01f;
            _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            _lineRenderer.startColor = Color.blue;
            _lineRenderer.endColor = Color.blue;
            _lineRenderer.positionCount = 2;
        }
    }

    void Update()
    {
        UpdateLineRenderer();
        RayGrab();
    }

    void UpdateLineRenderer()
    {
        if (_lineRenderer == null) return;
        _lineRenderer.enabled = !_isRayGrabbing;
        _lineRenderer.SetPosition(0, transform.position);
        _lineRenderer.SetPosition(1, transform.position + transform.forward * grabRange);
    }

    void RayGrab()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, controller))
        {
            if (!_isRayGrabbing)
                TryRaycast();
            else
                Release();
        }

        // Pull racket toward hand
        if (_isRayGrabbing && _rayGrabbedObject != null)
        {
            Vector3 targetPos = gripTransform.position;
            float distanceToHand = Vector3.Distance(
                _rayGrabbedObject.transform.position, targetPos);

            if (distanceToHand > 0.3f)
            {
                // Still pulling toward hand
                _rayGrabbedRb.linearVelocity =
                    (targetPos - _rayGrabbedObject.transform.position) * grabSpeed;
            }
            else
            {
                // Close enough — just release, let OVRGrabberToggle handle grab naturally
                Release();
            }
        }
    }

    void TryRaycast()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit,
            grabRange, grabbableLayer, QueryTriggerInteraction.Collide))
        {
            OVRGrabbable grabbable = hit.collider.GetComponentInParent<OVRGrabbable>();
            if (grabbable != null)
            {
                _rayGrabbedObject = grabbable.gameObject;
                _rayGrabbedRb = _rayGrabbedObject.GetComponent<Rigidbody>();
                _isRayGrabbing = true;
            }
        }
    }

    void Release()
    {
        if (_rayGrabbedRb != null)
            _rayGrabbedRb.linearVelocity = Vector3.zero;

        _isRayGrabbing = false;
        _rayGrabbedObject = null;
        _rayGrabbedRb = null;

        // Force nearby detection so normal grab works immediately
        grabberToggle?.ForceDetectNearby();
    }
}