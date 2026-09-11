using UnityEngine;
using UnityEngine.UI; // Obrigatório para reconhecer o Slider

public class BossSpawnerTrigger : MonoBehaviour
{
    [Header("Configurações do Boss")]
    [Tooltip("Arraste o Prefab do Boss aqui.")]
    public GameObject bossPrefab;
    
    [Tooltip("O ponto exato onde o Boss vai nascer (perto do tubo). Se deixar vazio, nasce na posição deste Trigger.")]
    public Transform pontoDeSpawn;

    [Header("Objetos do Cenário para Desativar")]
    [Tooltip("Arraste aqui o objeto do tubo que você quer sumir da tela.")]
    public GameObject objetoTuboParaDesativar;

    [Header("UI do Jogo (Cena)")]
    [Tooltip("Arraste o Slider desativado da Barra de Vida do Boss aqui.")]
    public Slider barraDeVidaUI;

    private bool jaSpawnou = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !jaSpawnou)
        {
            jaSpawnou = true;
            SpawnarBoss();
        }
    }

    void SpawnarBoss()
    {
        Vector3 posicaoNascimento = transform.position;
        if (pontoDeSpawn != null)
        {
            posicaoNascimento = pontoDeSpawn.position;
        }

        if (bossPrefab != null)
        {
            // 1. Instancia o Boss na cena
            GameObject bossInstanciado = Instantiate(bossPrefab, posicaoNascimento, Quaternion.identity);
            
            // 2. Configura a referência do Player no Boss automaticamente
            MutantBirdAI scriptAI = bossInstanciado.GetComponent<MutantBirdAI>();
            if (scriptAI != null)
            {
                scriptAI.player = GameObject.FindGameObjectWithTag("Player").transform;
                scriptAI.currentState = MutantBirdAI.BossState.Idle; // Liga a AI dele para começar a agir
                Debug.Log("A batalha contra o Pássaro Mutante Começou de verdade!");
            }

            // 3. Encontra o script de vida no Boss e injeta a Barra de Vida
            MutantBirdHealth scriptVida = bossInstanciado.GetComponent<MutantBirdHealth>();
            if (scriptVida != null && barraDeVidaUI != null)
            {
                // Liga o objeto da interface na tela e configura os valores máximos
                scriptVida.InicializarBarraDeVida(barraDeVidaUI);
            }
        }
        else
        {
            Debug.LogWarning("Esqueceu de colocar o Prefab do Boss no script do Trigger do Tubo!");
        }

        // 4. Desliga o objeto do tubo
        if (objetoTuboParaDesativar != null)
        {
            objetoTuboParaDesativar.SetActive(false);
        }

        // 5. Destrói o gatilho de spawn
        Destroy(gameObject);
    }
}