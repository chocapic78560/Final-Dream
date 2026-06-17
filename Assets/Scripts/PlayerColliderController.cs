using UnityEngine;

public class PlayerColliderController : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void SetStandingCollider()
    {
        boxCollider.offset = Vector2.zero;
        boxCollider.size = new Vector2(0.92f, 1.89f);
    }

    void SetCrouchingCollider()
    {
        boxCollider.offset = new Vector2(0f,-0.31f);
        boxCollider.size = new Vector2(0.92f,1.28f);
    }

    void SetDyingCollider()
    {
        boxCollider.size = new Vector2(0.92f, 1.28f);
    }
}
