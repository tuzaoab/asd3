using UnityEngine;

public class BossAttackDamage : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se colidiu com o jogador
        Character player = other.GetComponent<Character>();
        if (player != null)
        {
            player.TakeDamage(1); // Aplica 1 de dano no player

            // Avisa a IA do Boss que ele acertou o golpe (para o sistema adaptativo funcionar!)
            MutantBirdAI boss = GetComponentInParent<MutantBirdAI>();
            if (boss != null)
            {
                boss.RegistrarAcertoNoPlayer();
            }
        }
    }
}