
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Ennemyground : NetworkBehaviour
{
    public float speed = 2f;
    public float attackRange = 3f;
    public float attackCooldown = 0.7f;

    private Transform player;
    private List<Transform> players = new List<Transform>();
    private Animator animator;
    private Rigidbody2D rb;

    private float lastAttackTime;
    private Vector2 moveDirection = Vector2.zero;
    private bool shouldMove = false;

	private bool isAttacking = false;
	public float damageAmount = 20f;

    void OnEnable()
    {
        PlayerController.OnPlayerSpawned += OnPlayerSpawned;
    }

    void OnDisable()
    {
        PlayerController.OnPlayerSpawned -= OnPlayerSpawned;
    }
    void OnPlayerSpawned(Transform p)
    {
        players.Add(p);
    }

    void SelectClosestPlayer()
    {
        if (players.Count == 1)
        {
            player = players[0];
        }
        else
        {
            foreach (Transform p in players)
            {
                if (Vector2.Distance(transform.position, p.position) <
                    Vector2.Distance(transform.position, player.position))
                {
                    player = p;
                }
            }
        }
    }
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!isServer)
        {
            return;
        }
        
        if (player == null)
        {
            SelectClosestPlayer();
            if (player == null)
            {
                return;
            }
        }

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            moveDirection = (player.position - transform.position).normalized;
            shouldMove = true;

            animator.SetBool("isWalking", true);
            animator.ResetTrigger("Attack");

            if (moveDirection.x > 0)
                transform.localScale = new Vector3(-1, 1, 1);
            else
                transform.localScale = new Vector3(1, 1, 1);
            
            transform.Translate(moveDirection * (speed * Time.deltaTime));
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

	public void AttackHit()
	{
    	isAttacking = true;
    	Debug.Log("Attack started");

    	EnemyAttackTrigger attackTrigger = GetComponentInChildren<EnemyAttackTrigger>();
    	if (attackTrigger != null)
    	{
            attackTrigger.DealDamage();
        	attackTrigger.ResetAttack();
    	}
	}


	void EndAttack()
	{
    	isAttacking = false;
	}

	public bool IsAttacking()
    {
        return isAttacking;
    }
}