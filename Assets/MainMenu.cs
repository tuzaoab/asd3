using UnityEngine;
using UnityEngine.SceneManagement;
 
public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject creditsPanel;
    public GameObject gameUI;
 
    [Header("Configurações de Debug / Spawn")]
    public Transform jogador; // Arraste o seu Player aqui
    public Transform pontoSpawnDebug; // Arraste um objeto vazio na posição do Debug
 
    // Variáveis de controle estáticas (mantêm valor ao carregar novas cenas)
    private static bool devePularMenuNoRefresh = false;
    private static bool usarSpawnDebugNoRefresh = false;
 
    /// <summary>
    /// Chamado pelo MudarCenaTrigger (ou por qualquer troca de cena) para avisar que
    /// a próxima cena NÃO deve mostrar o menu — só liberar o jogo direto.
    /// </summary>
    public static void AtivarFadeProximaCena()
    {
        devePularMenuNoRefresh = true;
        usarSpawnDebugNoRefresh = false;
    }
 
    void Awake()
    {
        // SE O JOGO VEIO DE UM REFRESH OU DE UMA TROCA DE CENA
        if (devePularMenuNoRefresh)
        {
            devePularMenuNoRefresh = false;
 
            Time.timeScale = 1f;
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (gameUI != null) gameUI.SetActive(true);
            if (creditsPanel != null) creditsPanel.SetActive(false);
 
            if (usarSpawnDebugNoRefresh && jogador != null && pontoSpawnDebug != null)
            {
                jogador.position = pontoSpawnDebug.position;
            }
            usarSpawnDebugNoRefresh = false;
 
            // O fade em si já foi iniciado por quem mandou carregar esta cena
            // (StartGame ou MudarCenaTrigger), através do FadeManager. Aqui só
            // deixamos o estado (menu escondido, jogo visível) pronto ANTES
            // da tela clarear, então o jogador nunca vê essa troca acontecer.
        }
        else
        {
            // COMEÇO NORMAL DO JOGO: Trava no menu
            FicarNoMenu();
        }
    }
 
    void FicarNoMenu()
    {
        Time.timeScale = 0f;
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (gameUI != null) gameUI.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
    }
 
    // BOTÃO 1: Começa o jogo do zero absoluto
    public void StartGame()
    {
        devePularMenuNoRefresh = true;
        usarSpawnDebugNoRefresh = false;
        Time.timeScale = 1f;
 
        RecarregarCenaAtualComFade();
    }
 
    // BOTÃO 2: Começa o jogo do zero no spawn de Debug
    public void StartGameDebug()
    {
        devePularMenuNoRefresh = true;
        usarSpawnDebugNoRefresh = true;
        Time.timeScale = 1f;
 
        RecarregarCenaAtualComFade();
    }
 
    // BOTÃO RESTART: Reinicia do zero limpando tudo
    public void RestartGame()
    {
        devePularMenuNoRefresh = true;
        usarSpawnDebugNoRefresh = false;
        Time.timeScale = 1f;
 
        RecarregarCenaAtualComFade();
    }
 
    private void RecarregarCenaAtualComFade()
    {
        int indiceCenaAtual = SceneManager.GetActiveScene().buildIndex;
 
        if (FadeManager.Instance != null)
            FadeManager.Instance.CarregarCenaComFade(indiceCenaAtual);
        else
            SceneManager.LoadScene(indiceCenaAtual);
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
 