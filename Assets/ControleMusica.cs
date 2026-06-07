using UnityEngine;

public class ControleMusica : MonoBehaviour
{
    public AudioSource fonteAudio;

    void Awake()
    {
        // Isso impede que o objeto da música seja destruído ao mudar de cena
        DontDestroyOnLoad(gameObject);
    }

    public void TocarMusica()
    {
        if (!fonteAudio.isPlaying)
        {
            fonteAudio.Play();
        }
    }
}