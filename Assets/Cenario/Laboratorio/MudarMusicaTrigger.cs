using UnityEngine;

public class MudarMusicaTrigger : MonoBehaviour
{
    [Header("Configuração de Áudio")]
    [Tooltip("Arraste aqui a música da próxima fase/área")]
    public AudioClip novaMusica;

    [Header("Configurações do Fade")]
    public float duracaoFade = 1.5f;
    [Range(0.1f, 1f)]
    public float volumeMaximo = 1f;

    private bool jaAcionou = false; // Variável corrigida corretamente

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se quem encostou foi o Player e se o gatilho já não foi acionado antes
        if (collision.CompareTag("Player") && !jaAcionou)
        {
            if (ControleMusica.Instancia != null && novaMusica != null)
            {
                jaAcionou = true;

                // Manda o ControleMusica fazer a transição com Fade
                ControleMusica.Instancia.TrocarMusica(novaMusica, duracaoFade, volumeMaximo);
            }
            else
            {
                if (ControleMusica.Instancia == null)
                    Debug.LogWarning("O objeto ControleMusica (GerenciadorAudio) não foi encontrado na cena!");
                if (novaMusica == null)
                    Debug.LogWarning("Você esqueceu de colocar o AudioClip no gatilho!");
            }
        }
    }
}