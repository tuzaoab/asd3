using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject creditsPanel;
    public GameObject gameUI;

    [Header("Configurações de Fade")]
    public GameObject fadeImageObject;
    private Image imagemPreta;

    [Header("Configurações de Debug / Spawn")]
    public Transform jogador; // Arraste o seu Player aqui
    public Transform pontoSpawnDebug; // Arraste um objeto vazio na posição do Debug

    void Awake()
    {
        FicarNoMenu();

        if (fadeImageObject != null)
        {
            imagemPreta = fadeImageObject.GetComponent<Image>();
        }
    }

    void FicarNoMenu()
    {
        Time.timeScale = 0f;
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (gameUI != null) gameUI.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
        if (fadeImageObject != null) fadeImageObject.SetActive(false);
    }

    // BOTÃO 1: Começa o jogo normal
    public void StartGame()
    {
        StartCoroutine(SequenciaStart(false)); // false = não teleporta
    }

    // BOTÃO 2: Começa o jogo no spawn alternativo (Debug)
    public void StartGameDebug()
    {
        StartCoroutine(SequenciaStart(true)); // true = teleporta para o ponto debug
    }

    IEnumerator SequenciaStart(bool usarSpawnDebug)
    {
        // 1. Some com o menu na hora
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);

        // 2. Ativa a tela preta imediatamente
        if (fadeImageObject != null)
        {
            fadeImageObject.SetActive(true);
            if (imagemPreta != null)
            {
                imagemPreta.color = new Color(0, 0, 0, 1);
            }
        }

        // --- PULO DO GATO DO DEBUG ---
        // Se clicou no botão de Debug, move o jogador enquanto a tela ainda está 100% preta
        if (usarSpawnDebug && jogador != null && pontoSpawnDebug != null)
        {
            jogador.position = pontoSpawnDebug.position;
        }

        // 3. SEGURA A TELA PRETA POR 0.9 SEGUNDOS
        yield return new WaitForSecondsRealtime(0.9f);

        // 4. Entra no jogo
        Time.timeScale = 1f;
        if (gameUI != null) gameUI.SetActive(true);

        // 5. Faz o efeito de clarear a tela suavemente
        if (imagemPreta != null)
        {
            float alfa = 1f;
            while (alfa > 0f)
            {
                alfa -= Time.deltaTime * 2f;
                imagemPreta.color = new Color(0, 0, 0, alfa);
                yield return null;
            }
        }

        if (fadeImageObject != null) fadeImageObject.SetActive(false);
    }

    public void OpenCredits()
    {
        if (creditsPanel != null) creditsPanel.SetActive(true);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (gameUI != null) gameUI.SetActive(false);
    }

    public void CloseCredits()
    {
        if (creditsPanel != null) creditsPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (gameUI != null) gameUI.SetActive(false);
    }
}