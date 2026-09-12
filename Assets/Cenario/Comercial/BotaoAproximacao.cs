using UnityEngine;
using TMPro;
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

    [Header("Cooldown no Cenário")]
    public TextMeshPro textoCooldown;
    public float tempoCooldown = 25f;

    [Header("Indicador de UI (Tela)")]
    public GameObject iconeUIBebida;

    [Header("Configurações do Power-Up")]
    public float novaVelocidadeCaminhada = 6f;
    public float novaVelocidadeCorrida = 10f;
    public float duracaoEfeito = 10f;

    [Header("Áudio de Ativação")]
    public AudioSource audioSource; // Arraste o AudioSource aqui (ou ele pega automatico)
    public AudioClip somAtivacao;   // Arraste o som (ex: beber, powerup, clique) aqui

    private bool playerPerto = false;
    private GameObject playerObjeto;
    private bool emCooldown = false;

    void Start()
    {
        if (indicadorE != null)
        {
            spriteRendererIndicador = indicadorE.GetComponent<SpriteRenderer>();
            if (spriteRendererIndicador != null) spriteRendererIndicador.enabled = false;
        }

        if (textoCooldown != null)
        {
            textoCooldown.sortingOrder = 999; // Força ficar em cima de tudo
            textoCooldown.gameObject.SetActive(false);
        }

        if (iconeUIBebida != null)
        {
            iconeUIBebida.SetActive(false);
        }

        // Pega o AudioSource automaticamente se estiver no mesmo objeto
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (playerPerto && Input.GetKeyDown(KeyCode.E) && !emCooldown)
        {
            StartCoroutine(RotinaAcaoECooldown());
        }
    }

    IEnumerator RotinaAcaoECooldown()
    {
        emCooldown = true;

        // TOCA O SOM INSTANTANEAMENTE AO APERTAR 'E'
        if (audioSource != null && somAtivacao != null)
        {
            audioSource.PlayOneShot(somAtivacao);
        }

        // 1. Animação do clique no botão "E"
        if (spriteRendererIndicador != null)
        {
            if (spriteApertado != null) spriteRendererIndicador.sprite = spriteApertado;
            yield return new WaitForSeconds(0.12f);

            if (spriteSolto != null) spriteRendererIndicador.sprite = spriteSolto;
            yield return new WaitForSeconds(0.12f);

            spriteRendererIndicador.enabled = false;
        }

        // 2. LIGA O COOLDOWN (Fica visível independente da distância)
        if (textoCooldown != null)
        {
            textoCooldown.gameObject.SetActive(true);
            textoCooldown.text = tempoCooldown.ToString();
        }

        // 3. Aplica o Power-Up de velocidade no Player
        PlayerMovement movimento = null;
        float velWalkOrig = 0f;
        float velRunOrig = 0f;

        if (playerObjeto != null)
        {
            movimento = playerObjeto.GetComponent<PlayerMovement>();
            if (movimento != null)
            {
                velWalkOrig = movimento.walkSpeed;
                velRunOrig = movimento.runSpeed;

                movimento.walkSpeed = novaVelocidadeCaminhada;
                movimento.runSpeed = novaVelocidadeCorrida;

                if (iconeUIBebida != null) iconeUIBebida.SetActive(true);
            }
        }

        // 4. Executa a contagem regressiva
        float tempoRestante = tempoCooldown;

        while (tempoRestante > 0)
        {
            if (textoCooldown != null)
                textoCooldown.text = Mathf.CeilToInt(tempoRestante).ToString();

            yield return new WaitForSeconds(1f);
            tempoRestante -= 1f;

            // Retira o efeito de velocidade após atingir a duração definida (10s)
            if (tempoRestante == (tempoCooldown - duracaoEfeito) && movimento != null)
            {
                movimento.walkSpeed = velWalkOrig;
                movimento.runSpeed = velRunOrig;
                if (iconeUIBebida != null) iconeUIBebida.SetActive(false);
            }
        }

        // Reset de segurança dos atributos
        if (movimento != null)
        {
            movimento.walkSpeed = velWalkOrig;
            movimento.runSpeed = velRunOrig;
            if (iconeUIBebida != null) iconeUIBebida.SetActive(false);
        }

        // 5. O cooldown acabou: apaga o número
        if (textoCooldown != null) textoCooldown.gameObject.SetActive(false);

        // Se o player ainda estiver perto quando o cooldown acabar, reativa a tecla "E"
        if (playerPerto && spriteRendererIndicador != null)
        {
            if (spriteNormal != null) spriteRendererIndicador.sprite = spriteNormal;
            spriteRendererIndicador.enabled = true;
        }

        emCooldown = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerPerto = true;
            playerObjeto = other.gameObject;

            // Só mostra o ícone "E" se NÃO estiver em cooldown
            if (!emCooldown && spriteRendererIndicador != null)
            {
                if (spriteNormal != null) spriteRendererIndicador.sprite = spriteNormal;
                spriteRendererIndicador.enabled = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerPerto = false;

            // Esconde APENAS a tecla "E" ao se afastar (o textoCooldown continua ativo)
            if (spriteRendererIndicador != null) spriteRendererIndicador.enabled = false;
        }
    }
}