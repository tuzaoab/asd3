using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EfeitoVinhetaTrigger : MonoBehaviour
{
    [Header("Configurações da Vinheta")]
    [Tooltip("Arraste direto o componente 'Image' da sua UI para cá")]
    public Image componenteImagemVinheta;

    [Range(0f, 1f)]
    [Tooltip("O quão escuro vai ficar (0 = transparente, 1 = preto total)")]
    public float intensidadeMaxima = 0.85f;

    [Tooltip("Velocidade com que o escuro aparece e desaparece")]
    public float velocidadeFade = 1.5f;

    private Coroutine coroutineAtual;

    void Start()
    {
        // Garante que a vinheta comece 100% invisível assim que a fase inicia
        if (componenteImagemVinheta != null)
        {
            Color corInicial = componenteImagemVinheta.color;
            corInicial.a = 0f;
            componenteImagemVinheta.color = corInicial;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (componenteImagemVinheta != null)
            {
                if (coroutineAtual != null) StopCoroutine(coroutineAtual);
                coroutineAtual = StartCoroutine(FazerFade(componenteImagemVinheta.color.a, intensidadeMaxima));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (componenteImagemVinheta != null)
            {
                if (coroutineAtual != null) StopCoroutine(coroutineAtual);
                coroutineAtual = StartCoroutine(FazerFade(componenteImagemVinheta.color.a, 0f));
            }
        }
    }

    private IEnumerator FazerFade(float alfaInicial, float alfaFinal)
    {
        float tempo = 0f;
        Color cor = componenteImagemVinheta.color;

        while (tempo < 1f)
        {
            tempo += Time.deltaTime * velocidadeFade;
            cor.a = Mathf.Lerp(alfaInicial, alfaFinal, tempo);
            componenteImagemVinheta.color = cor;
            yield return null;
        }

        cor.a = alfaFinal;
        componenteImagemVinheta.color = cor;
    }
}