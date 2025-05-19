using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackTrigger : MonoBehaviour
{
    private Ennemyground enemy;
    private bool hasDealtDamageThisAttack = false;
    private Queue<GameObject> playersInZone = new Queue<GameObject>();

    void Start()
    {
        enemy = GetComponentInParent<Ennemyground>();
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
            Healthmanager health = playersInZone.Peek().GetComponent<Healthmanager>();
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