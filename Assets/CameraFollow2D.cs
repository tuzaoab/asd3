using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    public Transform target;   // o personagem a seguir
    public float smoothSpeed = 0.125f;
    public Vector3 offset;     // ajuste de posição da câmera

    public float minY = -2f;   // limite inferior normal
    public float maxY = 2f;    // limite superior normal

    [Header("Configurações de Zoom")]
    public float defaultZoom = 5f;       // Zoom normal do jogo
    public float zoomSpeed = 2f;         // Velocidade da transição do zoom
    private float targetZoom;            // O zoom atual que a câmera deve buscar
    private Camera cam;                  // Referência ao componente de câmera

    [Header("Configurações do Boss")]
    private bool isBossRoom = false; // Controla se a câmera está no modo boss
    private Vector3 bossCameraPosition; // Posição central da arena (configurada no trigger)

    void Awake()
    {
        cam = GetComponent<Camera>();
        targetZoom = defaultZoom;
        if (cam != null) cam.orthographicSize = defaultZoom;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition;

            if (isBossRoom)
            {
                // MODO BOSS MODIFICADO:
                // O X fica travado no centro da arena (para mostrar o cenário do boss inteiro)
                // O Y continua seguindo o jogador, mas respeita os novos limites (minY e maxY) da arena!
                desiredPosition.x = bossCameraPosition.x;
                desiredPosition.y = Mathf.Clamp(target.position.y + offset.y, minY, maxY);
                desiredPosition.z = target.position.z + offset.z; 
            }
            else
            {
                // Comportamento normal: Segue o personagem em tudo
                desiredPosition = target.position + offset;
                desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
            }

            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }

        // Gerenciar Zoom suave
        if (cam != null)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);
        }
    }

    public void LockToBossRoom(Vector3 positionToLock, float bossZoom)
    {
        isBossRoom = true;
        bossCameraPosition = positionToLock;
        targetZoom = bossZoom; 
    }

    public void UnlockFromBossRoom()
    {
        isBossRoom = false;
        targetZoom = defaultZoom; 
    }

    public void MudarApenasOZoom(float novoZoom)
    {
        targetZoom = novoZoom;
    }

    // Usado pelo Trigger do Zoom para atualizar os limites verticais da câmera
    public void AlterarLimitesVerticais(float novoMinY, float novoMaxY)
    {
        minY = novoMinY;
        maxY = novoMaxY;
    }

    public bool EstaNaArenaDoBoss()
    {
        return isBossRoom;
    }
}