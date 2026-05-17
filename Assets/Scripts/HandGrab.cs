using UnityEngine;

public class HandGrabPose : MonoBehaviour
{
    public OVRGrabberToggle grabber;
    public OVRSkeleton skeleton;

    private OVRPlugin.HandState _handState;

    void LateUpdate()
    {
        // If grabbed, force all finger bones to closed position
        if (grabber != null && grabber.IsGrabToggled)
        {
            foreach (var bone in skeleton.Bones)
            {
                // Curl fingers inward
                bone.Transform.localRotation = Quaternion.Lerp(
                    bone.Transform.localRotation,
                    Quaternion.Euler(60, 0, 0),
                    0.3f);
            }
        }
    }
}