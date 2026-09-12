using UnityEngine;
using System.Collections;

public class ControleMusica : MonoBehaviour
{
    public static ControleMusica Instancia;

    [Header("Configuração de Áudio")]
    public AudioSource fonteAudio;

    void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // Inicia a música da fase normalmente (do zero) com Fade In
    public void IniciarMusicaComFade(AudioClip clipInicial, float duracaoFade = 1.5f, float volumeMaximo = 1f)
    {
        if (fonteAudio == null || clipInicial == null) return;

        fonteAudio.Stop();
        fonteAudio.clip = clipInicial;

        // COMEÇA DO ZERO (NORMAL)
        fonteAudio.time = 0f;

        fonteAudio.volume = 0f;
        fonteAudio.Play();

        StartCoroutine(EfeitoFadeIn(duracaoFade, volumeMaximo));
    }

    // Troca a música atual por outra com Fade Out/In (Invocado pela Porta / Boss)
    public void TrocarMusica(AudioClip novaMusica, float duracaoFade = 1.5f, float volumeMaximo = 1f)
    {
        if (fonteAudio == null || novaMusica == null) return;
        if (fonteAudio.clip == novaMusica && fonteAudio.isPlaying) return;

        StartCoroutine(MudarMusicaComFade(novaMusica, duracaoFade, volumeMaximo));
    }

    private IEnumerator EfeitoFadeIn(float duracaoFade, float volumeMaximo)
    {
        float tempo = 0f;
        while (tempo < duracaoFade)
        {
            tempo += Time.deltaTime;
            fonteAudio.volume = Mathf.Lerp(0f, volumeMaximo, tempo / duracaoFade);
            yield return null;
        }
        fonteAudio.volume = volumeMaximo;
    }

    private IEnumerator MudarMusicaComFade(AudioClip novaMusica, float duracaoFade, float volumeMaximo)
    {
        float volumeInicial = fonteAudio.volume;
        if (volumeInicial <= 0.05f) volumeInicial = volumeMaximo;

        // 1. Fade Out
        float tempo = 0f;
        while (tempo < duracaoFade)
        {
            tempo += Time.deltaTime;
            fonteAudio.volume = Mathf.Lerp(volumeInicial, 0f, tempo / duracaoFade);
            yield return null;
        }

        // 2. Troca o clipe e dá Play do zero
        fonteAudio.Stop();
        fonteAudio.clip = novaMusica;
        fonteAudio.time = 0f;
        fonteAudio.volume = 0f;
        fonteAudio.Play();

        // 3. Fade In
        tempo = 0f;
        while (tempo < duracaoFade)
        {
            tempo += Time.deltaTime;
            fonteAudio.volume = Mathf.Lerp(0f, volumeMaximo, tempo / duracaoFade);
            yield return null;
        }

        fonteAudio.volume = volumeMaximo;
    }
}