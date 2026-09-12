using UnityEngine;
using UnityEngine.SceneManagement;

public class MudarCenaTrigger : MonoBehaviour
{
    [Header("Configuração da Cena")]
    [Tooltip("Nome exato da cena que você quer carregar (como está na pasta do projeto)")]
    public string nomeDaProximaCena;

    [Header("Música da Nova Fase")]
    public AudioClip musicaSegundaFase; // Arraste a música da nova cena aqui

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se quem encostou no trigger foi o Player
        if (!other.CompareTag("Player")) return;

        // Verifica se o nome da cena não está vazio
        if (string.IsNullOrEmpty(nomeDaProximaCena))
        {
            Debug.LogWarning("Você esqueceu de colocar o nome da próxima cena no Inspector!");
            return;
        }

        // Avisa qualquer MainMenu que exista na próxima cena pra NÃO mostrar o menu de novo
        MainMenu.AtivarFadeProximaCena();

        // Troca a música para a da nova fase com efeito de Fade
        if (ControleMusica.Instancia != null && musicaSegundaFase != null)
        {
            ControleMusica.Instancia.TrocarMusica(musicaSegundaFase);
        }

        if (FadeManager.Instance != null)
        {
            // Fade pra preto -> carrega a cena -> fade de volta
            FadeManager.Instance.CarregarCenaComFade(nomeDaProximaCena);
        }
        else
        {
            Debug.LogWarning("Nenhum FadeManager encontrado. Carregando a cena sem efeito de fade.");
            SceneManager.LoadScene(nomeDaProximaCena);
        }
    }
}