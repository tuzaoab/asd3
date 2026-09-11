using UnityEngine;
using UnityEngine.UI;
using System.Collections; 

public class MutantBirdHealth : MonoBehaviour
{
    public int vidaMaxima = 1000; 
    private float vidaAtual;

    private MutantBirdAI bossAI;
    private bool jaEntrouNaFase2 = false;

    [Header("UI do Boss")]
    public Slider barraDeVidaUI; 

    [Header("SISTEMA DE SANGUE (IGUAL AO ZUMBI)")]
    [Tooltip("Arraste aqui o Prefab da explosão de sangue (GameObject)")]
    public GameObject bloodExplosion; 
    public GameObject[] bloodPrefabs;
    public float bloodSpawnRadius = 1.2f; 
    public float bloodYMin = -4f;
    public float bloodYMax = 4f;
    public int bloodAmount = 8; 
    public Vector2 bloodScaleMin = new Vector2(1.2f, 1.2f);
    public Vector2 bloodScaleMax = new Vector2(2.0f, 2.0f);
    public bool lockBloodRotation = false;

    private SpriteRenderer[] sprites;
    private Color[] originalColors;
    private Coroutine flashCoroutine;

    void Awake()
    {
        vidaAtual = vidaMaxima;
        bossAI = GetComponent<MutantBirdAI>();
        
        sprites = GetComponentsInChildren<SpriteRenderer>();
        originalColors = new Color[sprites.Length];
        for (int i = 0; i < sprites.Length; i++)
        {
            originalColors[i] = sprites[i].color;
        }
    }

    public void InicializarBarraDeVida(Slider barraDaCena)
    {
        barraDeVidaUI = barraDaCena;

        if (barraDeVidaUI != null) 
        {
            barraDeVidaUI.gameObject.SetActive(true);
            barraDeVidaUI.maxValue = vidaMaxima / 2; // 500
            barraDeVidaUI.value = vidaMaxima / 2;    // 500
        }
    }

    public void TomarDano(float dano)
    {
        if (bossAI == null || bossAI.currentState == MutantBirdAI.BossState.Dead) return;

        if (bossAI.currentState == MutantBirdAI.BossState.TentaculoAttack && bossAI.animator.GetBool("InvisivelDano"))
        {
            return; 
        }

        if (bossAI.currentState == MutantBirdAI.BossState.Defesa)
        {
            return; 
        }

        vidaAtual -= dano;
        AtualizarUI();

        // Spawna as partículas de sangue no hit usando GameObject
        if (bloodExplosion != null) 
        {
            Instantiate(bloodExplosion, transform.position, Quaternion.identity);
        }

        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(DamageFlash());

        // GATILHO DA FASE 2
        if (vidaAtual <= vidaMaxima / 2 && !jaEntrouNaFase2)
        {
            EntrarNaSegundaFase();
            return;
        }

        // MORTE REAL DO BOSS
        if (vidaAtual <= 0)
        {
            MorrerDeVerdade();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            TomarDano(50);
        }
    }

    void AtualizarUI()
    {
        if (barraDeVidaUI == null) return;

        if (!jaEntrouNaFase2)
        {
            barraDeVidaUI.value = vidaAtual - (vidaMaxima / 2);
        }
        else
        {
            barraDeVidaUI.value = vidaAtual;
        }
    }

    IEnumerator DamageFlash()
    {
        foreach (var s in sprites) if (s != null) s.color = Color.red;
        yield return new WaitForSeconds(0.12f);
        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i] != null) sprites[i].color = originalColors[i];
        }
    }

    void EntrarNaSegundaFase()
    {
        jaEntrouNaFase2 = true;
        
        // Garante que a vida atual interna fique exatamente no topo da metade da barra
        vidaAtual = vidaMaxima / 2;

        if (barraDeVidaUI != null)
        {
            barraDeVidaUI.maxValue = vidaMaxima / 2; 
            barraDeVidaUI.value = vidaMaxima / 2;    
        }

        bossAI.MudarParaFase2();
    }

void MorrerDeVerdade()
{
    // Avisa a IA para cancelar os ataques e limpar os indicadores pendentes
    if (bossAI != null)
    {
        bossAI.InterromperEMorrer();
        bossAI.currentState = MutantBirdAI.BossState.Dead; // Trava o estado como morto antes de destruir
    }

    // 1. Explosão final de sangue
    if (bloodExplosion != null) 
        Instantiate((GameObject)bloodExplosion, transform.position, Quaternion.identity);

    // 2. Poças de sangue no chão
    SpawnBloodOnGround();

    if (barraDeVidaUI != null) barraDeVidaUI.gameObject.SetActive(false);

    // ---> ALTERAÇÃO AQUI: Chama a função com o delay de 1 segundo!
    if (CreditTrigger.Instance != null)
    {
        CreditTrigger.Instance.AbrirCreditosComDelay(1.0f);
    }

    // 3. Deleta o Boss da cena definitivamente (os efeitos de sangue e poças continuam lá)
    Destroy(gameObject);
}

    private void SpawnBloodOnGround()
    {
        if (bloodPrefabs.Length == 0) return;

        for (int i = 0; i < bloodAmount; i++)
        {
            Vector2 offset = Random.insideUnitCircle * bloodSpawnRadius;
            Vector2 spawnPos = new Vector2(transform.position.x + offset.x, Mathf.Clamp(transform.position.y + offset.y, bloodYMin, bloodYMax));
            
            GameObject prefab = bloodPrefabs[Random.Range(0, bloodPrefabs.Length)];
            GameObject decal = Instantiate(prefab, spawnPos, Quaternion.identity);
            
            decal.transform.rotation = lockBloodRotation ? Quaternion.identity : Quaternion.Euler(0, 0, Random.Range(0f, 360f));
            decal.transform.localScale = new Vector3(Random.Range(bloodScaleMin.x, bloodScaleMax.x), Random.Range(bloodScaleMin.y, bloodScaleMax.y), 1);
        }
    }
}