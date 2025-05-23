using UnityEngine;

public class ChangeMusic : MonoBehaviour
{
    public GameObject music1;
    public GameObject music2;
    public GameObject music3;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            music1.SetActive(true);
            music2.SetActive(false);
            music3.SetActive(false);
        }
    }
}
