using UnityEngine;

public class BossContactDamage : MonoBehaviour
{
    [Header("Configurações de Ataque por Contato")]
    public float attackCooldown = 1f; // Tempo entre um dano de encostar e outro
    private float nextAttackTime = 0f;

    private void OnTriggerStay2D(Collider2D other)
    {
        // Verifica se quem entrou no colisor de contato foi o jogador
        Character player = other.GetComponent<Character>();
        
        if (player != null && Time.time >= nextAttackTime)
        {
            player.TakeDamage(1); // Dá 1 de dano no player
            nextAttackTime = Time.time + attackCooldown;

            // Avisa o sistema adaptativo do Boss que ele pontuou acertando o jogador
            MutantBirdAI bossAI = GetComponentInParent<MutantBirdAI>();
            if (bossAI != null)
            {
                bossAI.RegistrarAcertoNoPlayer();
            }
        }
    }
}