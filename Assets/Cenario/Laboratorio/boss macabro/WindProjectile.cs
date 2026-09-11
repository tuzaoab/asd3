using UnityEngine;

public class WindProjectile : MonoBehaviour
{
    [Header("Configurações do Vento")]
    public float velocidadeVento = 10f;
    public float forcaDoEmpurrao = 18f; // Aumentei um pouco para o impacto ser bem visível!
    public float tempoDeVida = 3f;      

    private Vector2 direcaoMovimento;

    void Start()
    {
        // 1. Garante que o projétil suma depois de um tempo para não pesar o jogo
        Destroy(gameObject, tempoDeVida);

        // 2. Encontra o jogador para definir a linha de profundidade (Y)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // No 2.5D, o vento mira na altura Y atual do player
            float direcaoX = (player.transform.position.x > transform.position.x) ? 1f : -1f;
            direcaoMovimento = new Vector2(direcaoX, 0f);

            // Ajusta a posição inicial do vento para ficar na mesma linha Y do jogador
            Vector3 posicaoAjustada = transform.position;
            posicaoAjustada.y = player.transform.position.y;
            transform.position = posicaoAjustada;
        }
        else
        {
            direcaoMovimento = Vector2.right;
        }
    }

    void Update()
    {
        // Move o vento em linha reta horizontal usando a velocidade definida
        transform.Translate(direcaoMovimento * velocidadeVento * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Checagem de segurança 2.5D: Só acerta se o jogador estiver na mesma linha Y
            float diferencaProfundidade = Mathf.Abs(transform.position.y - other.transform.position.y);
            
            if (diferencaProfundidade < 0.8f) // Se o player andou muito para cima ou para baixo, ele desviou!
            {
                Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                {
                    // CORREÇÃO AQUI: Zeramos a velocidade atual do player por um instante 
                    // para a força do vento não ser cancelada pelo script de movimento dele.
                    playerRb.velocity = Vector2.zero;

                    // Aplicamos o empurrão usando velocidade direta combinada com AddForce para garantir o tranco
                    Vector2 vetorEmpurrao = direcaoMovimento * forcaDoEmpurrao;
                    playerRb.AddForce(vetorEmpurrao, ForceMode2D.Impulse);
                    
                    // Se o seu player tiver um script de "Knockback" ou "Stun", 
                    // você pode chamar ele aqui para o jogador não conseguir andar contra o vento imediatamente!
                }

                // Avisa a IA do Boss para o sistema adaptativo
                MutantBirdAI boss = Object.FindFirstObjectByType<MutantBirdAI>();
                if (boss != null) boss.RegistrarAcertoNoPlayer();

                // Aplique o dano aqui se tiver o script:
                // other.GetComponent<PlayerHealth>().TomarDano(1);

                Destroy(gameObject); // O vento se desfaz ao colidir
            }
        }
    }
}