using UnityEngine;

public class EnemyGroundColliderControlller : MonoBehaviour
{
    private BoxCollider2D boxCollider;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void SetFightCollider()
    {
        boxCollider.offset = new Vector2(0.3f, -0.3f);
    }

    void SetDefaultCollider()
    {
        boxCollider.offset = Vector2.zero;
    }
}
