using UnityEngine;

public class CameraZoomTrigger : MonoBehaviour
{
    [Header("Configurações da Câmera")]
    public CameraFollow2D cameraScript; 

    [Header("Configuração de Zoom")]
    public float novoZoom = 7f; 
    public bool resetarAoSair = true; 

    [Header("Novos Limites Verticais (Y) da CÂMERA")]
    public float cameraNovoMinY = -5f; 
    public float cameraNovoMaxY = 5f;  

    [Header("Novos Limites Verticais (Y) do JOGADOR")]
    public float playerNovoMinY = -4f; 
    public float playerNovoMaxY = 4f;  

    private float backupCameraMinY;
    private float backupCameraMaxY;
    private float backupPlayerMinY;
    private float backupPlayerMaxY;

    // Variável para sabermos se este gatilho específico chegou a aplicar o zoom antes do boss
    private bool aplicouZoomNestaEntrada = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && cameraScript != null)
        {
            // BLOQUEIO: Se a câmera já estiver travada no boss, ignore este gatilho completamente!
            if (cameraScript.EstaNaArenaDoBoss()) return;

            aplicouZoomNestaEntrada = true;

            // 1. CONFIGURAÇÕES DA CÂMERA
            backupCameraMinY = cameraScript.minY;
            backupCameraMaxY = cameraScript.maxY;

            cameraScript.MudarApenasOZoom(novoZoom);
            cameraScript.AlterarLimitesVerticais(cameraNovoMinY, cameraNovoMaxY);

            // 2. CONFIGURAÇÕES DO JOGADOR
            PlayerMovement playerMov = other.GetComponent<PlayerMovement>();
            if (playerMov != null)
            {
                backupPlayerMinY = playerMov.minY;
                backupPlayerMaxY = playerMov.maxY;

                playerMov.AlterarLimitesDeMovimento(playerMov.minX, playerMov.maxX, playerNovoMinY, playerNovoMaxY);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && resetarAoSair && cameraScript != null)
        {
            // BLOQUEIO: Se o boss foi ativado enquanto o player estava aqui dentro, 
            // não resete nada, senão vai quebrar o zoom e limites do boss!
            if (cameraScript.EstaNaArenaDoBoss()) return;

            // Só reseta se esse gatilho de fato alterou as coisas ao entrar
            if (!aplicouZoomNestaEntrada) return;

            // 1. RESTAURAR CÂMERA
            cameraScript.UnlockFromBossRoom(); 
            cameraScript.AlterarLimitesVerticais(backupCameraMinY, backupCameraMaxY); 

            // 2. RESTAURAR JOGADOR
            PlayerMovement playerMov = other.GetComponent<PlayerMovement>();
            if (playerMov != null)
            {
                playerMov.AlterarLimitesDeMovimento(playerMov.minX, playerMov.maxX, backupPlayerMinY, backupPlayerMaxY);
            }

            aplicouZoomNestaEntrada = false;
        }
    }
}