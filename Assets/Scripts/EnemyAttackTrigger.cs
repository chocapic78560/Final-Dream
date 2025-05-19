using UnityEngine;

public class EnemyAttackTrigger : MonoBehaviour
{
    private Ennemyground enemy;
    private bool hasDealtDamageThisAttack = false;

    void Start()
    {
        enemy = GetComponentInParent<Ennemyground>();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!enemy || !enemy.IsAttacking()) return;

        if (other.CompareTag("Player") && !hasDealtDamageThisAttack)
        {
            Healthmanager health = other.GetComponent<Healthmanager>();
            if (health != null)
            {
                Debug.Log("Dégâts infligés !");
                health.TakeDamage(enemy.damageAmount);
                hasDealtDamageThisAttack = true;
            }
        }
    }

    public void ResetAttack()
    {
        hasDealtDamageThisAttack = false;
    }

}