using UnityEngine;

public class TeleportOnTouch : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Vector3 newPosition = other.transform.position;
            newPosition.x += 100f;
            other.transform.position = newPosition;
        }
    }
}



