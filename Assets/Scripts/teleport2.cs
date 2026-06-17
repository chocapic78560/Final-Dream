using UnityEngine;

public class Teleport2 : MonoBehaviour
{
    public GameObject music2;
    public GameObject music3;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Vector3 newPosition = other.transform.position;
            newPosition.x += 100f;
            other.transform.position = newPosition;
        }
        
        music2.SetActive(false);
        music3.SetActive(true);
    }
}
