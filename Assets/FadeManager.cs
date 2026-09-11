using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controla o efeito de fade (tela preta) em QUALQUER troca de cena.
///
/// A imagem preta é criada pelo PRÓPRIO script em tempo de execução, como filha
/// deste GameObject — assim ela sempre sobrevive ao DontDestroyOnLoad junto com
/// o script, não importa como a cena esteja organizada. Isso evita o erro
/// "MissingReferenceException" que acontecia quando a Image ficava numa cena
/// normal (e era destruída) enquanto só o script persistia.
///
/// Como usar:
/// 1. Crie um GameObject vazio na PRIMEIRA cena do jogo (a do menu), chamado "FadeManager".
/// 2. Adicione este script nele. NÃO precisa criar Canvas nem Image na mão.
/// 3. Não repita esse objeto nas outras cenas — ele persiste sozinho.
/// 4. Se você tinha um Canvas/Image antigo ligado à versão anterior do script,
///    pode apagar ele da cena — não é mais usado.
/// </summary>
public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance { get; private set; }

    [Header("Configuração de Fade")]
    [SerializeField] private float velocidadeFade = 2f;
    [SerializeField] private Color corFade = Color.black;

    private Image imagemFade;

    private void Awake()
    {
        // Garante que só existe UM FadeManager, mesmo se ele acabar em mais de uma cena
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CriarImagemFade();
    }

    private void CriarImagemFade()
    {
        GameObject canvasGO = new GameObject("FadeCanvas");
        canvasGO.transform.SetParent(transform, false);

        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999; // sempre por cima de tudo, inclusive do menu

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasGO.AddComponent<GraphicRaycaster>();

        GameObject imagemGO = new GameObject("FadeImage");
        imagemGO.transform.SetParent(canvasGO.transform, false);

        imagemFade = imagemGO.AddComponent<Image>();
        imagemFade.raycastTarget = true; // bloqueia cliques enquanto a transição acontece

        RectTransform rect = imagemFade.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        imagemFade.color = new Color(corFade.r, corFade.g, corFade.b, 0f);
        imagemGO.SetActive(false);
    }

    /// <summary>Escurece a tela, carrega a cena pelo NOME e depois clareia de volta.</summary>
    public void CarregarCenaComFade(string nomeCena)
    {
        StartCoroutine(RotinaCarregarCena(() => SceneManager.LoadScene(nomeCena)));
    }

    /// <summary>Escurece a tela, carrega a cena pelo BUILD INDEX e depois clareia de volta.</summary>
    public void CarregarCenaComFade(int buildIndex)
    {
        StartCoroutine(RotinaCarregarCena(() => SceneManager.LoadScene(buildIndex)));
    }

    /// <summary>Só clareia a tela (força preto e depois vai para transparente).</summary>
    public void FadeIn()
    {
        StartCoroutine(RotinaFadeInForcado());
    }

    private IEnumerator RotinaFadeInForcado()
    {
        imagemFade.gameObject.SetActive(true);
        imagemFade.color = new Color(corFade.r, corFade.g, corFade.b, 1f);
        yield return FadeParaTransparente();
    }

    private IEnumerator RotinaCarregarCena(Action carregarCena)
    {
        // 1) Escurece a tela ANTES de trocar de cena
        yield return FadeParaPreto();

        // 2) Carrega a nova cena
        carregarCena?.Invoke();

        // 3) Espera a nova cena renderizar o primeiro frame
        yield return new WaitForEndOfFrame();

        // 4) Clareia a tela na cena nova
        yield return FadeParaTransparente();
    }

    private IEnumerator FadeParaPreto()
    {
        imagemFade.gameObject.SetActive(true);
        float alfa = imagemFade.color.a;

        while (alfa < 1f)
        {
            alfa += Time.unscaledDeltaTime * velocidadeFade;
            imagemFade.color = new Color(corFade.r, corFade.g, corFade.b, Mathf.Clamp01(alfa));
            yield return null;
        }

        imagemFade.color = new Color(corFade.r, corFade.g, corFade.b, 1f);
    }

    private IEnumerator FadeParaTransparente()
    {
        imagemFade.gameObject.SetActive(true);
        float alfa = imagemFade.color.a;

        while (alfa > 0f)
        {
            alfa -= Time.unscaledDeltaTime * velocidadeFade;
            imagemFade.color = new Color(corFade.r, corFade.g, corFade.b, Mathf.Clamp01(alfa));
            yield return null;
        }

        imagemFade.color = new Color(corFade.r, corFade.g, corFade.b, 0f);
        imagemFade.gameObject.SetActive(false);
    }
}