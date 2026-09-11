using UnityEngine;

public class ControleMusica : MonoBehaviour
{
    // Variável estática que guarda qual é o controle de música oficial ativo
    public static ControleMusica Instancia;

    public AudioSource fonteAudio;

    void Awake()
    {
        // ==================== TRAVA ANTI-DUPLICAÇÃO ====================
        if (Instancia == null)
        {
            // Se for o primeiro, ele vira o oficial e fica salvo na memória
            Instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Se a cena resetou e um NOVO ControleMusica nasceu, mas já existe o oficial...
            // Esse novo se destrói imediatamente e impede a música de duplicar!
            Destroy(gameObject);
            return;
        }
        // ===============================================================
    }

    public void TocarMusica()
    {
        if (fonteAudio != null && !fonteAudio.isPlaying)
        {
            fonteAudio.Play();
        }
    }
}