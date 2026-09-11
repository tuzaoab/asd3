using UnityEngine;

public class GritoDamage : MonoBehaviour
{
    private bool jaDeuDanoNesteAtaque = false;

    // Toda vez que a animação ativa a hitbox, o OnEnable roda e reseta a permissão de dar dano
    void OnEnable()
    {
        jaDeuDanoNesteAtaque = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Se já acertou o player desta vez, ignora colisões extras até o colisor sumir e voltar
        if (jaDeuDanoNesteAtaque) return;

        if (other.CompareTag("Player"))
        {
            Character player = other.GetComponent<Character>();
            if (player != null)
            {
                player.TakeDamage(1); // Garante 1 de dano
                jaDeuDanoNesteAtaque = true; // Bloqueia novos acertos imediatos

                // Avisa a IA do Boss
                MutantBirdAI boss = GetComponentInParent<MutantBirdAI>();
                if (boss != null)
                {
                    boss.RegistrarAcertoNoPlayer();
                }
            }
        }
    }
}