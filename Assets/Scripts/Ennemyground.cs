
using System.Collections.Generic;
using Mirror;
using Unity.Netcode.Components;
using UnityEngine;
using NetworkAnimator = Mirror.NetworkAnimator;

public class Ennemyground : NetworkBehaviour
{
    public float speed = 2f;
    public float attackRange = 3f;
    public float attackCooldown = 0.7f;
    
    private Transform player;
    private List<Transform> players = new List<Transform>();
    private Animator animator;
    private NetworkAnimator networkAnimator;
    private Rigidbody2D rb;

    private float lastAttackTime;
    private Vector2 moveDirection = Vector2.zero;
    private bool shouldMove = false;

	private bool isAttacking = false;
	public float damageAmount = 20f;
    
    private NetworkTransformReliable networkTransform;

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
        if (players.Count >= 1)
        {
            player = players[0];
            foreach (Transform p in players)
            {
                if (Vector2.Distance(networkTransform.transform.position, p.position) <
                    Vector2.Distance(networkTransform.transform.position, player.position))
                {
                    player = p;
                }
            }
        }
    }
    void Start()
    {
        networkAnimator = GetComponent<NetworkAnimator>();
        networkTransform = GetComponent<NetworkTransformReliable>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!isServer)
        {
            return;
        }
        SelectClosestPlayer();
        if (player == null)
        {
            return;
        }
        
        GoTowardsPlayer();
    }

    void GoTowardsPlayer()
    {
        float distance = Vector2.Distance(networkTransform.transform.position, player.position);

        if (distance > attackRange)
        {
            moveDirection = (player.position - networkTransform.transform.position).normalized;
            shouldMove = true;

            networkAnimator.animator.SetBool("isWalking", true);
            networkAnimator.animator.ResetTrigger("Attack");

            if (moveDirection.x > 0)
                networkTransform.transform.localScale = new Vector3(-1, 1, 1);
            else
                networkTransform.transform.localScale = new Vector3(1, 1, 1);
            
            networkTransform.transform.Translate(moveDirection * (speed * Time.deltaTime));
        }
        else
        {
            shouldMove = false;
            moveDirection = Vector2.zero;

            networkAnimator.animator.SetBool("isWalking", false);

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                networkAnimator.animator.SetTrigger("Attack");
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