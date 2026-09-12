using UnityEngine;

public class GiantTentacle : MonoBehaviour
{
    [Header("Configurações")]
    public float tempoAtaqueAtivo = 1.5f; // Tempo total de vida do Prefab na cena
    public int danoDoTentaculo = 1;

    [Header("Componentes")]
    [Tooltip("Arraste o Collider do tentáculo que funciona como a área de dano.")]
    public Collider2D hitboxCollider;

    void Start()
    {
        // Garante que o tentáculo se destrua após o tempo total de vida definido pelo Boss
        Destroy(gameObject, tempoAtaqueAtivo);
        
        // Boa prática: Garante que ele comece com a hitbox desligada para não dar dano fantasma
        if (hitboxCollider != null) hitboxCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Aplica o dano no jogador
            Character playerComponent = other.GetComponent<Character>();
            if (playerComponent != null)
            {
                playerComponent.TakeDamage(danoDoTentaculo);
            }

            // Avisa a IA do Boss para o sistema adaptativo de pesos
            MutantBirdAI boss = Object.FindFirstObjectByType<MutantBirdAI>();
            if (boss != null)
            {
                boss.RegistrarAcertoNoPlayer();
            }

            Debug.Log("burger");
        }
    }

    // =================================================================
    // FUNÇÕES PARA OS ANIMATION EVENTS (CHAMADOS PELA ANIMAÇÃO)
    // =================================================================

    /// <summary>
    /// Chame esta função no frame em que o tentáculo atinge o ápice/chão.
    /// </summary>
    public void AtivarHitboxTentaculo()
    {
        if (hitboxCollider != null)
        {
            hitboxCollider.enabled = true;
        }
    }

    /// <summary>
    /// Chame esta função alguns frames depois, quando o golpe acabar e ele começar a sumir.
    /// </summary>
    public void DesativarHitboxTentaculo()
    {
        if (hitboxCollider != null)
        {
            hitboxCollider.enabled = false;
        }
    }
}