using UnityEngine;

public class BossRoomTrigger : MonoBehaviour
{
    [Header("Configurações da Câmera")]
    public CameraFollow2D cameraScript; 
    public Transform cameraBossPosition;
    public float zoomDaArena = 8f; 

    [Header("Novos Limites da CÂMERA na Arena")]
    public float cameraMinY = -3f; 
    public float cameraMaxY = 3f;  

    [Header("Novos Limites Laterais (X) na Arena do Player")]
    public float arenaMinX = 50f;  
    public float arenaMaxX = 80f;  

    [Header("Novos Limites Verticais (Y) na Arena do Player")]
    public float arenaMinY = -4f; 
    public float arenaMaxY = 4f;  

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. CONFIGURAÇÕES DA CÂMERA
            if (cameraScript != null)
            {
                cameraScript.LockToBossRoom(cameraBossPosition.position, zoomDaArena);
                cameraScript.AlterarLimitesVerticais(cameraMinY, cameraMaxY);
            }

            // 2. CONFIGURAÇÕES DO JOGADOR
            PlayerMovement playerMov = other.GetComponent<PlayerMovement>();
            if (playerMov != null)
            {
                playerMov.AlterarLimitesDeMovimento(arenaMinX, arenaMaxX, arenaMinY, arenaMaxY);
            }

            // Desativa apenas o colisor deste gatilho da câmera para não rodar novamente
            GetComponent<Collider2D>().enabled = false;
        }
    }
}