using Mirror;
using UnityEngine;

public class EnemyHealth : NetworkBehaviour
{
    public int maxHealth = 100;
    
    [SyncVar]
    public int currentHealth;
    
    private bool isDead = false;

    private Animator animator;
    private NetworkAnimator networkAnimator;

    void Start()
    {
        networkAnimator = GetComponent<NetworkAnimator>();
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        if (animator != null)
        {
            networkAnimator.SetTrigger("Die");
        }

        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
        {
            if (script != this) script.enabled = false;
        }

        float deathAnimationDuration = 2f;
        Destroy(gameObject, deathAnimationDuration);
    }
}