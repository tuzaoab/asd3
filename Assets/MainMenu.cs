using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject creditsPanel;
    public GameObject gameUI;

    [Header("UI das Armas")]
    public GameObject hudPistolaMAG;

    [Header("Configurações de Áudio")]
    public AudioClip musicaPrimeiraFase;
    public AudioSource audioSourceMusicaMenu; // Arraste o AudioSource do objeto AudioMenu aqui (com Play on Awake DESMARCADO)

    [Header("Configurações de Debug / Spawn")]
    public Transform jogador;
    public Transform pontoSpawnDebug;

    public void AlternarMuteMusicaMenu()
    {
        if (audioSourceMusicaMenu != null)
        {
            // Inverte o estado atual de mute (se tá mutado, desmuta; se tá tocando, muta)
            audioSourceMusicaMenu.mute = !audioSourceMusicaMenu.mute;

            Debug.Log("Música do menu mutada? " + audioSourceMusicaMenu.mute);
        }
    }
    private static bool devePularMenuNoRefresh = false;
    private static bool usarSpawnDebugNoRefresh = false;
    private static bool musicaJaIniciada = false;

    public static void AtivarFadeProximaCena()
    {
        devePularMenuNoRefresh = true;
        usarSpawnDebugNoRefresh = false;
    }

    void Awake()
    {
        if (devePularMenuNoRefresh)
        {
            devePularMenuNoRefresh = false;

            Time.timeScale = 1f;
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (gameUI != null) gameUI.SetActive(true);
            if (creditsPanel != null) creditsPanel.SetActive(false);

            if (hudPistolaMAG != null) hudPistolaMAG.SetActive(true);

            // Garante que a música do menu fique totalmente desligada durante a gameplay
            if (audioSourceMusicaMenu != null)
            {
                audioSourceMusicaMenu.Stop();
            }

            // AGUARDA 3 SEGUNDOS PARA ATIVAR O CONTADOR APÓS O REFRESH DA CENA
            StartCoroutine(AtivarContadorComAtraso(3f));

            if (usarSpawnDebugNoRefresh && jogador != null && pontoSpawnDebug != null)
            {
                jogador.position = pontoSpawnDebug.position;
            }
            usarSpawnDebugNoRefresh = false;
        }
        else
        {
            FicarNoMenu();
        }
    }

    void FicarNoMenu()
    {
        Time.timeScale = 0f;
        musicaJaIniciada = false;
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (gameUI != null) gameUI.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);

        if (hudPistolaMAG != null) hudPistolaMAG.SetActive(false);

        // Toca a música do menu pulando direto para o segundo 110 (1:50)
        if (audioSourceMusicaMenu != null && !audioSourceMusicaMenu.isPlaying)
        {
            audioSourceMusicaMenu.Play();
            audioSourceMusicaMenu.time = 100f; // <--- SÓ O MENU COMEÇA AQUI
        }
    }

    public void StartGame()
    {
        ExecutarTransicaoJogo(false);
    }

    public void StartGameDebug()
    {
        ExecutarTransicaoJogo(true);
    }

    public void RestartGame()
    {
        ExecutarTransicaoJogo(false);
    }

    private void ExecutarTransicaoJogo(bool usarDebug)
    {
        devePularMenuNoRefresh = true;
        usarSpawnDebugNoRefresh = usarDebug;
        Time.timeScale = 1f;

        // Para a música do menu imediatamente ao clicar em jogar
        if (audioSourceMusicaMenu != null)
        {
            audioSourceMusicaMenu.Stop();
        }

        DispararMusicaInstantanea();
        RecarregarCenaAtualComFade();
    }

    private IEnumerator AtivarContadorComAtraso(float segundos)
    {
        yield return new WaitForSecondsRealtime(segundos);

        if (GerenciadorMissao.Instancia != null)
        {
            GerenciadorMissao.Instancia.IniciarMissao();
        }
    }

    private void DispararMusicaInstantanea()
    {
        if (!musicaJaIniciada && ControleMusica.Instancia != null && musicaPrimeiraFase != null)
        {
            musicaJaIniciada = true;
            ControleMusica.Instancia.IniciarMusicaComFade(musicaPrimeiraFase);
        }
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
        if (hudPistolaMAG != null) hudPistolaMAG.SetActive(false);
    }

    public void CloseCredits()
    {
        if (creditsPanel != null) creditsPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (gameUI != null) gameUI.SetActive(false);
        if (hudPistolaMAG != null) hudPistolaMAG.SetActive(false);
    }
}