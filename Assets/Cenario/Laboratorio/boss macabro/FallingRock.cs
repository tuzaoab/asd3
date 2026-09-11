using UnityEngine;

public class FallingRock : MonoBehaviour
{
    [Header("Configurações de Queda")]
    public float velocidadeQueda = 12f;
    private float yDoChaoDaArena; 

    [Header("Referências")]
    public Transform spriteDaPedra;   
    public Transform spriteDaSombra;  

    private bool jaImpactou = false;
    private float alturaInicial;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            yDoChaoDaArena = player.transform.position.y;
        }
        else
        {
            yDoChaoDaArena = transform.position.y - 6f; 
        }

        alturaInicial = transform.position.y;

        // Se a pedra spawnar abaixo do chão por erro, jogamos ela pra cima
        if (alturaInicial <= yDoChaoDaArena)
        {
            alturaInicial = yDoChaoDaArena + 8f;
            transform.position = new Vector3(transform.position.x, alturaInicial, transform.position.z);
        }

        // A sombra se desprende do pai para ficar fixa no chão da arena, seguindo apenas o X
        if (spriteDaSombra != null)
        {
            spriteDaSombra.SetParent(null); 
            spriteDaSombra.position = new Vector3(transform.position.x, yDoChaoDaArena, spriteDaSombra.position.z);
            spriteDaSombra.localScale = new Vector3(0.2f, 0.1f, 1f); 
        }
    }

    void Update()
    {
        if (jaImpactou) return;

        // AGORA SIM: O objeto pai (que tem o Collider) DESCE fisicamente pelo mapa!
        transform.position += Vector3.down * velocidadeQueda * Time.deltaTime;

        // Atualiza a posição X da sombra caso a pedra se mova (ou para ficar alinhada)
        if (spriteDaSombra != null)
        {
            spriteDaSombra.position = new Vector3(transform.position.x, yDoChaoDaArena, spriteDaSombra.position.z);
        }

        // Cálculo visual do crescimento da sombra baseado na distância até o chão
        float distanciaTotal = alturaInicial - yDoChaoDaArena;
        float distanciaAtual = transform.position.y - yDoChaoDaArena;

        if (distanciaAtual > 0 && spriteDaSombra != null && distanciaTotal > 0)
        {
            float progresso = Mathf.Clamp01(1f - (distanciaAtual / distanciaTotal));
            float escalaSombra = Mathf.Lerp(0.2f, 1.2f, progresso);
            spriteDaSombra.localScale = new Vector3(escalaSombra, escalaSombra / 2f, 1f); 
        }

        // Quando o Objeto Pai tocar o chão da arena, acontece o impacto
        if (transform.position.y <= yDoChaoDaArena)
        {
            ImpactarNoChao();
        }
    }

    void ImpactarNoChao()
    {
        if (jaImpactou) return;
        jaImpactou = true;

        if (spriteDaSombra != null) 
        {
            Destroy(spriteDaSombra.gameObject);
        }
        
        Destroy(gameObject); 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Se bater no Player e não tiver impactado ainda
        if (other.CompareTag("Player") && !jaImpactou)
        {
            // Como o colisor desce junto com a pedra, a colisão é REAL no frame do toque!
            Character playerScript = other.GetComponent<Character>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(1);
            }

            MutantBirdAI boss = Object.FindFirstObjectByType<MutantBirdAI>();
            if (boss != null) boss.RegistrarAcertoNoPlayer();

            ImpactarNoChao();
        }
    }
}