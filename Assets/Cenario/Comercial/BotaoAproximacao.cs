using UnityEngine;
using System.Collections;

public class BotaoAproximacao : MonoBehaviour
{
    [Header("Indicador no Cenário (Mundo)")]
    public GameObject indicadorE;
    private SpriteRenderer spriteRendererIndicador;

    [Header("Sprites da Animação de Clique")]
    public Sprite spriteNormal;
    public Sprite spriteApertado;
    public Sprite spriteSolto;

    [Header("Indicador de UI (Tela)")]
    public GameObject iconeUIBebida; // Arraste aqui a imagem do Canvas

    [Header("Configurações do Power-Up")]
    public float novaVelocidadeCaminhada = 6f;
    public float novaVelocidadeCorrida = 10f;
    public float duracaoEfeito = 10f;

    private bool playerPerto = false;
    private GameObject playerObjeto;
    private bool jaAtivou = false;

    void Start()
    {
        if (indicadorE != null)
        {
            spriteRendererIndicador = indicadorE.GetComponent<SpriteRenderer>();
            indicadorE.SetActive(false);
        }

        // Garante que o ícone da tela começa desativado
        if (iconeUIBebida != null)
        {
            iconeUIBebida.SetActive(false);
        }
    }

    void Update()
    {
        if (playerPerto && Input.GetKeyDown(KeyCode.E) && !jaAtivou)
        {
            StartCoroutine(SequenciaAnimacaoEClique());
        }
    }

    IEnumerator SequenciaAnimacaoEClique()
    {
        jaAtivou = true;

        // --- ANIMAÇÃO TROCANDO OS SPRITES ---
        if (spriteRendererIndicador != null)
        {
            spriteRendererIndicador.sprite = spriteApertado;
            yield return new WaitForSeconds(0.2f);

            spriteRendererIndicador.sprite = spriteSolto;
            yield return new WaitForSeconds(0.2f);

            spriteRendererIndicador.sprite = spriteNormal;
            yield return new WaitForSeconds(0.25f);
        }

        // --- APLICAÇÃO DO TURBO E ATIVAÇÃO DA UI ---
        if (playerObjeto != null)
        {
            PlayerMovement movimento = playerObjeto.GetComponent<PlayerMovement>();

            if (movimento != null)
            {
                float velocidadeCaminhadaOriginal = movimento.walkSpeed;
                float velocidadeCorridaOriginal = movimento.runSpeed;

                // Ativa a velocidade rápida
                movimento.walkSpeed = novaVelocidadeCaminhada;
                movimento.runSpeed = novaVelocidadeCorrida;

                // Some com o "E" flutuante do cenário
                if (indicadorE != null) indicadorE.SetActive(false);

                // LIGA O ÍCONE NA TELA DO JOGADOR
                if (iconeUIBebida != null) iconeUIBebida.SetActive(true);

                // Espera o tempo do efeito (10 segundos)
                yield return new WaitForSeconds(duracaoEfeito);

                // Devolve as velocidades normais
                if (movimento != null)
                {
                    movimento.walkSpeed = velocidadeCaminhadaOriginal;
                    movimento.runSpeed = velocidadeCorridaOriginal;
                }

                // DESLIGA O ÍCONE NA TELA DO JOGADOR
                if (iconeUIBebida != null) iconeUIBebida.SetActive(false);
            }
        }

        jaAtivou = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerPerto = true;
            playerObjeto = other.gameObject;

            if (indicadorE != null && !jaAtivou)
            {
                if (spriteRendererIndicador != null) spriteRendererIndicador.sprite = spriteNormal;
                indicadorE.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerPerto = false;
            playerObjeto = null;

            if (indicadorE != null)
            {
                indicadorE.SetActive(false);
            }
        }
    }
}