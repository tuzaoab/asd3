using UnityEngine;
using System.Collections; // IMPORTANTE: Necessário para usar as Coroutines (IEnumerator)

public class CreditTrigger : MonoBehaviour
{
    public static CreditTrigger Instance;

    public GameObject creditsPanel;
    public GameObject gameUI;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (creditsPanel != null) creditsPanel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Character ch = other.GetComponent<Character>();
        if (ch != null)
        {
            // Se passar pelo trigger físico, abre normal (ou com delay se preferir)
            OpenCredits();
        }
    }

    // Método que o Boss vai chamar! Ele inicia a contagem regressiva em segundo plano
    public void AbrirCreditosComDelay(float tempoEspera)
    {
        StartCoroutine(EsperarParaAbrir(tempoEspera));
    }

    private IEnumerator EsperarParaAbrir(float tempoEspera)
    {
        // Espera 1 segundo com o tempo do jogo correndo normalmente (efeitos e partículas rodam)
        yield return new WaitForSeconds(tempoEspera);
        
        // Abre os créditos e pausa o jogo
        OpenCredits();
    }

    public void OpenCredits()
    {
        if (creditsPanel != null) creditsPanel.SetActive(true);
        if (gameUI != null) gameUI.SetActive(false);
        
        Time.timeScale = 0f;
    }
}