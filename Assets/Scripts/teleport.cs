using UnityEngine;

public class TeleportOnTouch : MonoBehaviour
{
    public GameObject music1;
    public GameObject music2;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Vector3 newPosition = other.transform.position;
            newPosition.x += 100f;
            other.transform.position = newPosition;
        }
        
        music1.SetActive(false);
        music2.SetActive(true);
    }
}



