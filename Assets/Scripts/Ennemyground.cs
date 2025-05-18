using UnityEngine;

public class Ennemyground : MonoBehaviour
{
    public float speed = 2f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1.5f;

    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;

    private float lastAttackTime;
    private Vector2 moveDirection = Vector2.zero;
    private bool shouldMove = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            // Se déplace vers le joueur
            moveDirection = (player.position - transform.position).normalized;
            shouldMove = true;

            // Animation
            animator.SetBool("isWalking", true);
            animator.ResetTrigger("Attack");

            // Flip
            if (moveDirection.x > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else
                transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            shouldMove = false;
            moveDirection = Vector2.zero;

            animator.SetBool("isWalking", false);

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                animator.SetTrigger("Attack");
                lastAttackTime = Time.time;
            }
        }
    }

    void FixedUpdate()
    {
        if (shouldMove)
        {
            rb.MovePosition(rb.position + moveDirection * speed * Time.fixedDeltaTime);
        }
    }
}