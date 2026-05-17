using UnityEngine;

public class OVRGrabberToggle : OVRGrabber
{
    private float _prevFlexToggle;
    private bool _isGrabToggled;
    public bool IsGrabToggled => _isGrabToggled;

    protected override void Awake()
    {
        // Don't call base.Awake() — we handle the event ourselves
        m_anchorOffsetPosition = transform.localPosition;
        m_anchorOffsetRotation = transform.localRotation;

        OVRCameraRig rig = transform.GetComponentInParent<OVRCameraRig>();
        if (rig != null)
        {
            // Only subscribe once
            rig.UpdatedAnchors -= OnUpdatedAnchorsToggle;
            rig.UpdatedAnchors += OnUpdatedAnchorsToggle;
            m_operatingWithoutOVRCameraRig = false;
        }
        else
        {
            m_operatingWithoutOVRCameraRig = true;
        }
    }

    void OnUpdatedAnchorsToggle(OVRCameraRig rig)
    {
        RunToggleGrab();
    }

    public override void Update()
    {
        if (m_operatingWithoutOVRCameraRig)
        {
            RunToggleGrab();
        }
    }

    public void ExternalGrabBegin()
    {
        base.GrabBegin();
        _isGrabToggled = m_grabbedObj != null;
    }

    public void ForceDetectNearby()
    {
        Collider[] nearby = Physics.OverlapSphere(m_gripTransform.position, 0.3f);
        foreach (Collider col in nearby)
        {
            OVRGrabbable grabbable = col.GetComponent<OVRGrabbable>()
                ?? col.GetComponentInParent<OVRGrabbable>();
            if (grabbable != null && !m_grabCandidates.ContainsKey(grabbable))
            {
                m_grabCandidates[grabbable] = 1;
            }
        }
    }

    void RunToggleGrab()
    {
        var toRemove = new System.Collections.Generic.List<OVRGrabbable>();
        foreach (var candidate in m_grabCandidates.Keys)
        {
            if (candidate == null)
                toRemove.Add(candidate);
        }
        foreach (var key in toRemove)
            m_grabCandidates.Remove(key);

        // Continuously detect nearby grabbables
        if (m_grabbedObj == null && !_isGrabToggled)
        {
            Collider[] nearby = Physics.OverlapSphere(
                m_gripTransform.position, 0.15f);

            foreach (Collider col in nearby)
            {
                if (col == null) continue; // null check

                OVRGrabbable grabbable = col.GetComponent<OVRGrabbable>()
                    ?? col.GetComponentInParent<OVRGrabbable>();
                if (grabbable != null && !m_grabCandidates.ContainsKey(grabbable))
                {
                    m_grabCandidates[grabbable] = 1;
                }
            }
        }
        if (m_grabbedObj == null && !_isGrabToggled)
        {
            Collider[] nearby = Physics.OverlapSphere(
                m_gripTransform.position, 0.15f);

            foreach (Collider col in nearby)
            {
                OVRGrabbable grabbable = col.GetComponent<OVRGrabbable>()
                    ?? col.GetComponentInParent<OVRGrabbable>();
                if (grabbable != null && !m_grabCandidates.ContainsKey(grabbable))
                {
                    m_grabCandidates[grabbable] = 1;
                }
            }
        }
        if (m_grabbedObj != null)
        {
            m_grabbedObj.grabbedRigidbody.interpolation
                = RigidbodyInterpolation.None;
            m_grabbedObj.grabbedRigidbody.isKinematic = true;
        }
        Vector3 destPos = m_parentTransform.TransformPoint(m_anchorOffsetPosition);
        Quaternion destRot = m_parentTransform.rotation * m_anchorOffsetRotation;

        if (m_moveHandPosition)
        {
            GetComponent<Rigidbody>().MovePosition(destPos);
            GetComponent<Rigidbody>().MoveRotation(destRot);
        }

        if (!m_parentHeldObject)
        {
            MoveGrabbedObject(destPos, destRot);
        }

        m_lastPos = transform.position;
        m_lastRot = transform.rotation;

        float currentFlex = OVRInput.Get(
            OVRInput.Axis1D.PrimaryHandTrigger, m_controller);

        if (currentFlex >= grabBegin && _prevFlexToggle < grabBegin)
        {
            if (!_isGrabToggled)
            {
                GrabBegin();
                _isGrabToggled = m_grabbedObj != null;
            }
            else
            {
                GrabEnd();
                _isGrabToggled = false;
            }
        }

        if (_isGrabToggled && m_grabbedObj == null)
        {
            _isGrabToggled = false;
        }

        _prevFlexToggle = currentFlex;
    }
}