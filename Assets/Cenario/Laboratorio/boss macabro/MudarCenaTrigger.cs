using UnityEngine;
using UnityEngine.SceneManagement;
 
public class MudarCenaTrigger : MonoBehaviour
{
    [Header("Configuração da Cena")]
    [Tooltip("Nome exato da cena que você quer carregar (como está na pasta do projeto)")]
    public string nomeDaProximaCena;
 
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
 