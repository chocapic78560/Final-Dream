using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class EnemyAttackTrigger : NetworkBehaviour
{
    private EnnemygroundMulti enemy;
    private bool hasDealtDamageThisAttack = false;
    private Queue<GameObject> playersInZone = new Queue<GameObject>();

    void Start()
    {
        enemy = GetComponentInParent<EnnemygroundMulti>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playersInZone.Enqueue(other.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playersInZone.Dequeue();
        }
    }
    public void DealDamage()
    {
        if (!enemy || !enemy.IsAttacking()) return;

        while (playersInZone.Count > 0 && playersInZone.Peek() == null)
        {
            playersInZone.Dequeue();
        }

        if (playersInZone.Count > 0 && !hasDealtDamageThisAttack)
        {
            HealthManager health = playersInZone.Peek().GetComponent<HealthManager>();
            if (health != null)
            {
                Debug.Log("Dégâts infligés !");
                health.CmdTakeDamage(enemy.damageAmount);
                hasDealtDamageThisAttack = true;
            }
        }
    }

    public void ResetAttack()
    {
        hasDealtDamageThisAttack = false;
    }

}