using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : NetworkBehaviour
{
    [Header("Health Settings")]
    [SyncVar(hook = nameof(OnHealthChanged))]
    public float healthAmount = 100f;
    
    [Header("Respawn Settings")]
    public Vector3 respawnPosition;
    public float respawnDelay = 3f;
    
    [Header("Water Damage Settings")]
    [SyncVar(hook = nameof(OnWaterStateChanged))]
    private bool isInWater = false;
    public float waterDamageRate = 1f;
    public float waterDamageAmount = 5f;

    [Header("State Management")]
    [SyncVar(hook = nameof(OnDeathStateChanged))]
    public bool isDead = false;
    
    [SyncVar]
    private bool deathAnimationPlayed = false;

    // Local References
    private Animator animator;
    private Mouvement mouvement;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    
    // UI References
    [SerializeField] private Image healthBar;
    
    private string assignedBar;
    private bool isPlayerLocal = false;

    // Constantes
    private const float MAX_HEALTH = 100f;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        mouvement = GetComponent<Mouvement>();
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        
        if (isServer && isLocalPlayer)
        {
            netIdentity.AssignClientAuthority(connectionToClient);
            Debug.Log($"Autorité donnée au client : {connectionToClient}");
        }
    }
    
    public override void OnStartClient()
    {
        base.OnStartClient();

        if (netIdentity != null)
        {
            isPlayerLocal = isLocalPlayer;

            if (isLocalPlayer)
            {
                assignedBar = "HealthBar";
            }
            else
            {
                assignedBar = "HealthBar player2";
            }

            GameObject p1HealthObj = GameObject.FindGameObjectWithTag(assignedBar);
            if (p1HealthObj != null)
            {
                Transform bar = p1HealthObj.transform.Find("Health");
                if (bar != null) healthBar = bar.GetComponent<Image>();
            }

            UpdateHealthBar();

            UpdateVisualState();
        }
    }
    
    void OnHealthChanged(float oldHealth, float newHealth)
    {
        healthAmount = Mathf.Clamp(newHealth, 0, MAX_HEALTH);
        
        UpdateHealthBar();
        
        if (isServer && healthAmount <= 0 && !isDead)
        {
            ServerDie();
        }
    }
    
    void UpdateHealthBar()
    {
        healthBar.fillAmount = healthAmount / MAX_HEALTH;
        Debug.Log("Health Bar Updated");
    }
    
    void OnDeathStateChanged(bool oldVal, bool newVal)
    {
        if (newVal && !deathAnimationPlayed)
        {
            PlayDeathAnimation();
            RpcPlayDeathAnimation();
        }
        else if (!newVal)
        {
            ResetDeathAnimation();
        }
        
        UpdateVisualState();
    }
    
    void UpdateVisualState()
    {
        if (isDead)
        {
            if (mouvement != null) mouvement.enabled = false;
            
            // Ne pas désactiver le SpriteRenderer immédiatement pour permettre à l'animation de mort de jouer
            if (rb != null) rb.gravityScale = 0f;
        }
        else
        {
            if (mouvement != null) mouvement.enabled = true;
            if (boxCollider != null) boxCollider.enabled = true;
            if (spriteRenderer != null) spriteRenderer.enabled = true;
            if (rb != null) rb.gravityScale = 1.99f;
        }
        Debug.Log("visual state");
    }
    
    // Gestion de l'animation de mort
    void PlayDeathAnimation()
    {
        if (deathAnimationPlayed) return;
        
        if (animator != null)
        {
            animator.SetTrigger("Die");
            deathAnimationPlayed = true;
            
            StartCoroutine(DisableSpriteAfterAnimation());
        }
    }

    [ClientRpc]
    public void RpcPlayDeathAnimation()
    {
        PlayDeathAnimation();
    }
    
    // Gestion de la réinitialisation après respawn
    void ResetDeathAnimation()
    {
        deathAnimationPlayed = false;
        
        if (animator != null)
        {
            animator.ResetTrigger("Die");
            animator.Play("Idle");
        }
    }
    
    // Coroutine pour désactiver le sprite après que l'animation soit terminée
    IEnumerator DisableSpriteAfterAnimation()
    {
        yield return new WaitForSeconds(1.2f);
        
        if (isDead && spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
    }

    // Gestion de l'état dans l'eau
    void OnWaterStateChanged(bool oldVal, bool newVal)
    {
        Debug.Log("OnWaterStateChanged");
        if (newVal && isServer)
        {
            StartCoroutine(WaterDamageCoroutine());
        }
    }

    // Coroutine de dégâts dans l'eau (exécutée uniquement sur le serveur)
    [Server]
    IEnumerator WaterDamageCoroutine()
    {
        while (isInWater && !isDead)
        {
            TryDealDamage(waterDamageAmount);
            yield return new WaitForSeconds(waterDamageRate);
        }
    }
    
    // Méthode serveur pour gérer la mort
    [Server]
    void ServerDie()
    {
        if (isDead) return;
        
        isDead = true;
        StartCoroutine(ServerRespawnCoroutine());
    }
    
    // Coroutine de respawn (exécutée uniquement sur le serveur)
    [Server]
    IEnumerator ServerRespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnDelay);
        
        healthAmount = MAX_HEALTH;
        isDead = false;
        RpcChangeHealth();
        
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.transform.position = respawnPosition;
        }
        RpcUpdatePosition(respawnPosition);
        
        
        RpcReactivatePlayer();
    }
    
    [ClientRpc]
    public void RpcUpdatePosition(Vector3 newPosition)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.transform.position = respawnPosition;
        }
    }
    
    [ClientRpc]
    public void RpcReactivatePlayer()
    {
        if (mouvement != null) mouvement.enabled = true;
        if (boxCollider != null) boxCollider.enabled = true;
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (rb != null) rb.gravityScale = 1.99f;

        ResetDeathAnimation();  // Remet l'animation en état normal

        Debug.Log("Le joueur est bien réactivé après le respawn !");
    }

    [ClientRpc]
    public void RpcChangeHealth()
    {
        UpdateHealthBar();
    }
    
    // Méthode pour infliger des dégâts (exécutée uniquement sur le serveur)
    [Server]
    public void TakeDamage(float damage)
    {
        if (isDead) return;
        
        healthAmount -= damage;

        if (!isServer)
        {
            Debug.Log("TakeDamage called from client");
        }
    }

    public void TryDealDamage(float damage)
    {
        Debug.Log("TryDealDamage() appelée par le joueur local !");
        RpcTakeDamage(damage);
    }

    [ClientRpc]
    public void RpcTakeDamage(float damage)
    {
        healthAmount -= damage;
        healthAmount = Mathf.Clamp(healthAmount, 0, MAX_HEALTH);
        UpdateHealthBar();

        if (healthAmount <= 0 && !isDead)
        {
            isDead = true;
            UpdateVisualState();  // Désactive le sprite et autres éléments
        }
    }

    [Command]
    public void CmdTakeDamage(float damage)
    {
        healthAmount -= damage;
        healthAmount = Mathf.Clamp(healthAmount, 0, MAX_HEALTH);
        UpdateHealthBar();

        if (healthAmount <= 0 && !isDead)
        {
            isDead = true;
            UpdateVisualState();  // Désactive le sprite et autres éléments
        }
        RpcTakeDamage(damage);
    }
    
    // Méthode pour soigner (exécutée uniquement sur le serveur)
    [Server]
    public void Heal(float healAmount)
    {
        if (isDead) return;
        
        healthAmount += healAmount;
        healthAmount = Mathf.Clamp(healthAmount, 0, MAX_HEALTH);
    }

    // Gestion des déclencheurs (exécutée uniquement sur le serveur)
    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;
        
        if (other.CompareTag("Water"))
        {
            CmdSetInWater(true);
        }

        if (other.CompareTag("Fall"))
        {
            CmdChangeHealth();
        }
    }

    [Command]
    void CmdChangeHealth()
    {
        healthAmount = 0;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            CmdSetInWater(false);
        }
    }

    // Méthode appelée par le client pour demander au serveur de vérifier la mort
    [Command]
    void CmdCheckDeath()
    {
        if (!isDead && healthAmount <= 0)
        {
            ServerDie();
        }
    }
    
    [Command]
    public void CmdSetInWater(bool inWater)
    {
        isInWater = inWater; // Mise à jour sur le serveur
    }
    
    void Update()
    {
        if (!isPlayerLocal) return;

        if (healthAmount <= 0 && !isDead)
        {
            CmdCheckDeath();
        }
    }
}