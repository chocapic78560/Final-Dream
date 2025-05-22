using UnityEngine;
using Mirror;
public class Mouvement : NetworkBehaviour
{
    public Rigidbody2D rb;
    public NetworkAnimator animator;
    public float Runspeed = 40f;
    private float horizontalMove = 0f;
    private bool isGrounded = false;
    private bool facingRight = true;
    private bool crouch = false;
	private float attackCooldown = 1f;
	private float nextAttackTime = 0f;
	private string[] attackTriggers = { "Attack", "Attack2" };
	public Transform attackPoint;
	public float attackRange = 0.5f;
	public int attackDamage = 25;
	public LayerMask enemyLayers;
    


    [Command]
    private void CmdSetAnimationState(string paramName, bool value)
    {
        // Update animation on server and propagate to all clients
        animator.animator.SetBool(paramName, value);
        RpcSetAnimationState(paramName, value);
    }

    [ClientRpc]
    private void RpcSetAnimationState(string paramName, bool value)
    {
        // Update animation on all clients
        animator.animator.SetBool(paramName, value);
    }

    [Command]
    private void CmdSetSpeedState(float value)
    {
        // Update speed on server and propagate to all clients
        animator.animator.SetFloat("Speed", value);
        RpcSetSpeedState(value);
    }

    [ClientRpc]
    private void RpcSetSpeedState(float value)
    {
        // Update speed on all clients
        animator.animator.SetFloat("Speed", value);
    }
    
    public override void OnStartClient()
    {
        base.OnStartClient();
        
        // Position the player based on whether they're host or client
        if (isLocalPlayer)
        {
            Vector3 spawnPos = isServer ? 
                spawnPoint.spawnPositions[0] : // Host position
                spawnPoint.spawnPositions[1];  // Client position
            
            transform.position = spawnPos;
        }
    }
    
    private void ResetAnimationStates()
    {
        if (animator != null && animator.animator != null)
        {
            animator.animator.SetBool("IsJumping", false);
            animator.animator.SetBool("JumpMid", false);
            animator.animator.SetBool("JumpFall", false);
            animator.animator.SetFloat("Speed", 0f);
        }
    }
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        if (!isLocalPlayer)
        {
            rb.simulated = false;
        }
        
        ResetAnimationStates();
    }

    void Update()
    {
		if (!isLocalPlayer) return;

		Healthmanager health = GetComponent<Healthmanager>();
		if (health != null && health.isDead) return;

        if (!isLocalPlayer) return;

		if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time >= nextAttackTime)
    	{
        	nextAttackTime = Time.time + attackCooldown;
        	CmdAttack();
    	}

        PlayerMovement();
        UpdateJumpAnimation();
    }

	[Command]
	private void CmdAttack()
	{
    	int index = Random.Range(0, attackTriggers.Length);
    	string attackToPlay = attackTriggers[index];
    	RpcAttack(attackToPlay);
	}

	[ClientRpc]
	private void RpcAttack(string attackTrigger)
	{
    	animator.animator.SetTrigger(attackTrigger);

    	if (!isLocalPlayer) return;

    	Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

    	foreach (Collider2D enemy in hitEnemies)
    	{
        	if (enemy.TryGetComponent(out EnemyHealth enemyHealth))
        	{
            	enemyHealth.TakeDamage(attackDamage);
        	}
    	}
	}


    private void PlayerMovement()
    {
        horizontalMove = Input.GetAxis("Horizontal") * Runspeed;
        CmdSetSpeedState(Mathf.Abs(horizontalMove));
        animator.animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        if (Input.GetButton("Horizontal"))
        {
            transform.Translate(7f * Input.GetAxis("Horizontal") * Time.deltaTime, 0, 0);
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector2.up * 10f, ForceMode2D.Impulse);
            isGrounded = false;
            animator.animator.SetBool("IsJumping", true);
        }
        
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            crouch = true;
            CmdSetAnimationState("IsCrouching",true);
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            crouch = false;
            CmdSetAnimationState("IsCrouching",false);
        }
        
        if (horizontalMove > 0 && !facingRight)
        {
            if (isServer) RpcFlip();
            else CmdRequestFlip();
        }
        else if (horizontalMove < 0 && facingRight)
        {
            if (isServer) RpcFlip();
            else CmdRequestFlip();
        }
    }

    private void UpdateJumpAnimation()
    {
        if (!isGrounded)
        {
            if (rb.linearVelocity.y > 2f) // Montée
            {
                SetJumpingAnimationState(true,false,false);
            }
            else if (rb.linearVelocity.y <= 2f && rb.linearVelocity.y >= -2f) // Sommet du saut
            {
                SetJumpingAnimationState(false,true,false);
            }
            else if (rb.linearVelocity.y < -2f) // Descente
            {
                SetJumpingAnimationState(false,false,true);
            }
        }
    }

    [ClientRpc]
    private void RpcFlip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        facingRight = !facingRight;
    }

    private void OnLanding()
    {
        if (!isLocalPlayer) return;
        SetJumpingAnimationState(false,false,false);
    }
    
    private void SetJumpingAnimationState(bool isJumping, bool isJumpMid, bool isJumpFall)
    {
        if (!isLocalPlayer) return;
        if (animator != null && animator.animator != null)
        {
            CmdSetAnimationState("IsJumping", isJumping);
            CmdSetAnimationState("JumpMid", isJumpMid);
            CmdSetAnimationState("JumpFall", isJumpFall);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("floor"))
        {
            isGrounded = true;
            OnLanding();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("floor"))
        {
            isGrounded = false;
        }
    }
    
    [Command]
    private void CmdRequestFlip()
    {
        RpcFlip();
    }
}
