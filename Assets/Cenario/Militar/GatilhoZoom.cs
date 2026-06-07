using UnityEngine;
using System.Collections;

public class GatilhoZoom : MonoBehaviour
{
    private Camera cameraPrincipal;

    [Header("Configurações do Zoom")]
    public float tamanhoNormal = 5f;   // Tamanho padrão da sua câmera
    public float tamanhoZoomOut = 8f;  // Tamanho com zoom out (câmera maior)
    public float velocidadeZoom = 2f;  // Quão rápido o zoom acontece

    private float tamanhoAlvo;
    private bool jogoE2D;

    void Start()
    {
        // Pega a câmera principal do jogo automaticamente
        cameraPrincipal = Camera.main;

        if (cameraPrincipal != null)
        {
            // Descobre se a câmera está configurada como 2D (Orthographic) ou 3D (Perspective)
            jogoE2D = cameraPrincipal.orthographic;

            // Define o tamanho inicial baseado no tipo de câmera
            tamanhoAlvo = jogoE2D ? cameraPrincipal.orthographicSize : cameraPrincipal.fieldOfView;
            tamanhoNormal = tamanhoAlvo;
        }
    }

    void Update()
    {
        if (cameraPrincipal == null) return;

        // Faz a transição suave (Lerp) entre o tamanho atual e o tamanho alvo
        if (jogoE2D)
        {
            cameraPrincipal.orthographicSize = Mathf.Lerp(cameraPrincipal.orthographicSize, tamanhoAlvo, Time.deltaTime * velocidadeZoom);
        }
        else
        {
            cameraPrincipal.fieldOfView = Mathf.Lerp(cameraPrincipal.fieldOfView, tamanhoAlvo, Time.deltaTime * velocidadeZoom);
        }
    }

    // Detecta quando o Player ENTRA na área invisível
    private void OnTriggerEnter2D(Collider2D other) { VerificarEntrada(other.gameObject); }
    private void OnTriggerEnter(Collider other) { VerificarEntrada(other.gameObject); }

    // Detecta quando o Player SAI da área invisível
    private void OnTriggerExit2D(Collider2D other) { VerificarSaida(other.gameObject); }
    private void OnTriggerExit(Collider other) { VerificarSaida(other.gameObject); }

    void VerificarEntrada(GameObject objeto)
    {
        if (objeto.CompareTag("Player"))
        {
            tamanhoAlvo = tamanhoZoomOut; // Ativa o zoom out
        }
    }

    void VerificarSaida(GameObject objeto)
    {
        if (objeto.CompareTag("Player"))
        {
            tamanhoAlvo = tamanhoNormal; // Volta ao tamanho original
        }
    }
}