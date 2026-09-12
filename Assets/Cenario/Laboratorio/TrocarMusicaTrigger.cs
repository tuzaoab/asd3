using UnityEngine;

public class TrocarMusicaTrigger : MonoBehaviour
{
    [Header("Configurações da Nova Música")]
    public AudioClip novaMusica;

    [Header("Configurações do Fade")]
    public float duracaoFade = 1.5f;
    [Range(0.1f, 1f)]
    public float volumeMaximo = 1f;

    private bool jaTrocou = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !jaTrocou)
        {
            if (ControleMusica.Instancia != null)
            {
                jaTrocou = true;
                ControleMusica.Instancia.TrocarMusica(novaMusica, duracaoFade, volumeMaximo);
            }
            else
            {
                Debug.LogError("ControleMusica não foi encontrado na cena!");
            }
        }
    }
}