using UnityEngine;

public class BirdieSpawner : MonoBehaviour
{
    public GameObject birdiePrefab;
    public Transform spawnPoint;
    public OVRInput.Controller controller;
    public OVRInput.Button spawnButton = OVRInput.Button.Two;

    private GameObject _currentBirdie;

    void Update()
    {
        if (OVRInput.GetDown(spawnButton, controller))
        {
            SpawnBirdie();
        }
    }

    void SpawnBirdie()
    {
        // Safely destroy existing birdie
        if (_currentBirdie != null)
        {
            // Unparent before destroying to avoid conflict
            _currentBirdie.transform.SetParent(null);
            Destroy(_currentBirdie);
            _currentBirdie = null;
        }

        // Wait a frame before spawning new one
        StartCoroutine(SpawnNextFrame());
    }

    System.Collections.IEnumerator SpawnNextFrame()
    {
        yield return null; // wait one frame for destroy to complete
        
        if (birdiePrefab != null && spawnPoint != null)
        {
            _currentBirdie = Instantiate(
                birdiePrefab,
                spawnPoint.position,
                spawnPoint.rotation);
        }
    }
}