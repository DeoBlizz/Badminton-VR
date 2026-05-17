using UnityEngine;

public class GrabDebug : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger entered: {other.gameObject.name} layer: {other.gameObject.layer}");
    }
    
    void OnTriggerStay(Collider other)
    {
        Debug.Log($"Trigger staying: {other.gameObject.name}");
    }
}
