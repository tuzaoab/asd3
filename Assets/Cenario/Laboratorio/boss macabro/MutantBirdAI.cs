using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class MutantBirdAI : MonoBehaviour
{
    public enum BossPhase { Fase1, Fase2 }
    public enum BossState { Idle, Walk, AsaAttack, GritoAttack, DashAttack, Defesa, TentaculoAttack, Dead }

    [Header("Estados Atuais")]
    public BossPhase currentPhase = BossPhase.Fase1;
    public BossState currentState = BossState.Idle;

    [Header("Referências")]
    public Transform player;
    public Animator animator;
    private SpriteRenderer sprite;
    private Rigidbody2D rb;

    [Header("Configurações de Movimento")]
    public float speed = 2f;
    public float dashSpeedPhase1 = 15f;
    public float dashSpeedPhase2 = 22f;
    [Tooltip("Tempo (em segundos) que o Boss fica parado carregando o dash antes de voar.")]
    public float tempoCarregandoDash = 0.6f;
    [Tooltip("Tempo (em segundos) que o Boss passa voando no Dash.")]
    public float tempoDeVooDash = 0.8f;

    [Header("Limites de Movimentação da Arena")]
    public float yMin = -4f;
    public float yMax = 4f;
    public float xMin = -10f;
    public float xMax = 4.5f;

    [Header("Prefabs e Indicadores Gerais")]
    public GameObject indicadorDash;       
    public GameObject pedraCaindoPrefab;   

    [Header("Sistema de Tentáculos (4 Prefabs)")]
    public GameObject indicadorVerticalPrefab;   
    public GameObject indicadorHorizontalPrefab; 
    public GameObject tentaculoVerticalPrefab;   
    public GameObject tentaculoHorizontalPrefab; 

    [Header("Customização dos Tentáculos (Inspector)")]
    public float tempoStartupTentaculo = 1.5f; 
    public float velocidadetentaculo = 6f;    
    public float tempoDeVidaTentaculo = 1.5f; 

    [Header("Posições de Spawn Customizadas")]
    public float spawnX_Direita = 5f;
    public float spawnX_Esquerda = -11f;
    public float spawnY_Cima = 5f;
    public float spawnY_Baixo = -5f;

    [Header("Configurações de Tempo Gerais")]
    public float tempoDefesa = 3f;          
    private float cooldownTimer;
    public float tempoEntreAtaques = 2.5f;

    [Header("Sistema de Adaptação (Pesos de Chance)")]
    public float pesoAsa = 25f;
    public float pesoGrito = 25f;
    public float pesoDash = 25f;
    public float pesoEspecial = 25f;

    private float pesoMinimo = 10f; 
    private float pesoMaximo = 40f; 
    private int indiceDoAtaqueAtual = -1;
    private bool causouDanoNesteAtaque = false;
    private bool estaAtacando = false;

    private Vector2 direcaoDoDashSalva;
    private bool executandoMovimentoDash = false;
    private GameObject indicadorInstanciado;
    private Coroutine failsafeCoroutine; 

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (player == null) player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (currentState == BossState.Dead || player == null) return;

        if (executandoMovimentoDash)
        {
            float velocidadeAtual = (currentPhase == BossPhase.Fase1) ? dashSpeedPhase1 : dashSpeedPhase2;
            rb.velocity = direcaoDoDashSalva * velocidadeAtual;

            Vector2 posTravada = rb.position;
            posTravada.x = Mathf.Clamp(posTravada.x, xMin, xMax);
            posTravada.y = Mathf.Clamp(posTravada.y, yMin, yMax);
            rb.position = posTravada;
            return; 
        }

        if (currentState == BossState.Defesa || currentState == BossState.TentaculoAttack)
        {
            rb.velocity = Vector2.zero;
            return; 
        }

        if (estaAtacando) return;

        cooldownTimer -= Time.deltaTime;
        GirarParaOPlayer();

        float distancia = Vector2.Distance(transform.position, player.position);

        if (cooldownTimer <= 0)
        {
            SortearAtaque(distancia);
        }
        else if (currentState == BossState.Idle && distancia > 3f)
        {
            animator.SetBool("IsWalking", true);
            Vector2 targetPos = new Vector2(player.position.x, player.position.y);
            Vector2 novaPosicao = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            
            novaPosicao.x = Mathf.Clamp(novaPosicao.x, xMin, xMax);
            novaPosicao.y = Mathf.Clamp(novaPosicao.y, yMin, yMax);
            
            transform.position = novaPosicao;
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }
    }

    void SortearAtaque(float distancia)
    {
        estaAtacando = true;
        animator.SetBool("IsWalking", false);
        causouDanoNesteAtaque = false; 

        if (failsafeCoroutine != null) StopCoroutine(failsafeCoroutine);

        float totalPesos = pesoAsa + pesoGrito + pesoDash + pesoEspecial;
        float valorSorteado = Random.Range(0f, totalPesos);

        if (currentPhase == BossPhase.Fase1)
        {
            if (valorSorteado < pesoAsa)
            {
                indiceDoAtaqueAtual = 0;
                currentState = BossState.AsaAttack;
                animator.SetTrigger("AsaFase1");
                failsafeCoroutine = StartCoroutine(SegurancaAntiTravamento(2.0f));
            }
            else if (valorSorteado < pesoAsa + pesoGrito)
            {
                indiceDoAtaqueAtual = 1;
                currentState = BossState.GritoAttack;
                animator.SetTrigger("Grito");
                failsafeCoroutine = StartCoroutine(SegurancaAntiTravamento(2.5f));
            }
            else if (valorSorteado < pesoAsa + pesoGrito + pesoDash)
            {
                indiceDoAtaqueAtual = 2;
                StartCoroutine(PrepararDashFase1());
            }
            else
            {
                indiceDoAtaqueAtual = 3;
                StartCoroutine(AtaqueDefesaFase1());
            }
        }
        else 
        {
            if (valorSorteado < pesoAsa)
            {
                indiceDoAtaqueAtual = 0;
                currentState = BossState.AsaAttack;
                animator.SetTrigger("AsaFase2");
                failsafeCoroutine = StartCoroutine(SegurancaAntiTravamento(2.0f));
            }
            else if (valorSorteado < pesoAsa + pesoGrito)
            {
                indiceDoAtaqueAtual = 1;
                StartCoroutine(AtaqueGritoFase2());
                failsafeCoroutine = StartCoroutine(SegurancaAntiTravamento(4.5f));
            }
            else if (valorSorteado < pesoAsa + pesoGrito + pesoDash)
            {
                indiceDoAtaqueAtual = 2;
                StartCoroutine(PrepararDashF2());
            }
            else
            {
                indiceDoAtaqueAtual = 4; 
                StartCoroutine(AtaqueTentaculo());
            }
        }
    }

    public void InterromperEMorrer()
    {
        StopAllCoroutines();
        estaAtacando = false;
        executandoMovimentoDash = false;
        
        if (rb != null) rb.velocity = Vector2.zero;

        if (indicadorInstanciado != null) Destroy(indicadorInstanciado);

        GameObject[] todosOsObjetos = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in todosOsObjetos)
        {
            if (obj != null && (obj.name.Contains("Indicador") || obj.name.Contains("indicador")))
            {
                if (obj.transform.parent == null && obj != gameObject) 
                {
                    Destroy(obj);
                }
            }
        }
    }

    IEnumerator PrepararDashFase1()
    {
        currentState = BossState.DashAttack;
        rb.velocity = Vector2.zero;
        
        direcaoDoDashSalva = (player.position - transform.position).normalized;
        CriarIndicadorDash();

        animator.SetTrigger("DashF1Start");
        failsafeCoroutine = StartCoroutine(SegurancaAntiTravamento(tempoCarregandoDash + tempoDeVooDash + 1f));

        yield return new WaitForSeconds(tempoCarregandoDash);

        animator.SetBool("DashF1Loop", true);
        if (indicadorInstanciado != null) Destroy(indicadorInstanciado);
        executandoMovimentoDash = true;

        yield return new WaitForSeconds(tempoDeVooDash);

        executandoMovimentoDash = false;
        animator.SetBool("DashF1Loop", false);
        FinalizarAtaqueMecanicamente();
    }

    IEnumerator PrepararDashF2()
    {
        currentState = BossState.DashAttack;
        rb.velocity = Vector2.zero;
        
        direcaoDoDashSalva = (player.position - transform.position).normalized;
        CriarIndicadorDash();

        animator.SetTrigger("DashF2Start");
        failsafeCoroutine = StartCoroutine(SegurancaAntiTravamento(tempoCarregandoDash + tempoDeVooDash + 1f));

        yield return new WaitForSeconds(tempoCarregandoDash);

        animator.SetBool("DashF2Loop", true);
        if (indicadorInstanciado != null) Destroy(indicadorInstanciado);
        executandoMovimentoDash = true;

        yield return new WaitForSeconds(tempoDeVooDash);

        executandoMovimentoDash = false;
        animator.SetBool("DashF2Loop", false);
        FinalizarAtaqueMecanicamente();
    }

    void CriarIndicadorDash()
    {
        if (indicadorDash != null)
        {
            indicadorInstanciado = Instantiate(indicadorDash, transform.position, Quaternion.identity);
            float angle = Mathf.Atan2(direcaoDoDashSalva.y, direcaoDoDashSalva.x) * Mathf.Rad2Deg;
            indicadorInstanciado.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    IEnumerator AtaqueDefesaFase1()
    {
        currentState = BossState.Defesa;
        rb.velocity = Vector2.zero; 

        animator.SetTrigger("DefesaF1Start");
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.25f); 

        animator.SetBool("DefesaF1Loop", true);
        
        float tempoPassado = 0f;
        while (tempoPassado < tempoDefesa)
        {
            tempoPassado += Time.deltaTime;
            yield return null;
        }

        animator.SetBool("DefesaF1Loop", false);
        FinalizarAtaqueMecanicamente();
    }

    IEnumerator AtaqueGritoFase2()
    {
        currentState = BossState.GritoAttack;
        animator.SetTrigger("GritoFase2"); 

        for (int i = 0; i < 5; i++)
        {
            float posXAleatoria = Random.Range(xMin, xMax);
            float alturaDeQueda = yMax + 4f; 

            Instantiate(pedraCaindoPrefab, new Vector3(posXAleatoria, alturaDeQueda, 0), Quaternion.identity);
            yield return new WaitForSeconds(0.2f); 
        }

        FinalizarAtaqueMecanicamente();
    }

    IEnumerator AtaqueTentaculo()
    {
        currentState = BossState.TentaculoAttack;
        rb.velocity = Vector2.zero; 

        animator.SetBool("Ataque_Tentaculo_F2_Loop", true); 
        failsafeCoroutine = StartCoroutine(SegurancaAntiTravamento(15f));

        for (int onda = 0; onda < 3; onda++)
        {
            int quantidadeDeTentaculos = Random.Range(1, 5);
            List<GameObject> indicadoresCriados = new List<GameObject>();
            List<Vector3> posicoesDeAtaque = new List<Vector3>();
            List<Vector2> direcoesMovimento = new List<Vector2>();
            List<bool> ehHorizontal = new List<bool>(); 

            for (int i = 0; i < quantidadeDeTentaculos; i++)
            {
                int direcaoBorda = Random.Range(0, 2); 
                Vector3 posSpawn = Vector3.zero;
                Vector2 dirMov = Vector2.zero;
                GameObject prefabIndicador = null;

                if (direcaoBorda == 0) 
                {
                    bool vemDeCima = Random.Range(0, 2) == 0;
                    float posXAleatoria = Random.Range(xMin, xMax);
                    float bordaY = vemDeCima ? spawnY_Cima : spawnY_Baixo;
                    posSpawn = new Vector3(posXAleatoria, bordaY, 0);
                    dirMov = vemDeCima ? Vector2.down : Vector2.up;
                    prefabIndicador = indicadorVerticalPrefab;
                    ehHorizontal.Add(false);
                }
                else 
                {
                    bool vemDaEsquerda = Random.Range(0, 2) == 0;
                    float posYAleatoria = Random.Range(yMin, yMax);
                    float bordaX = vemDaEsquerda ? spawnX_Esquerda : spawnX_Direita;
                    posSpawn = new Vector3(bordaX, posYAleatoria, 0);
                    dirMov = vemDaEsquerda ? Vector2.right : Vector2.left;
                    prefabIndicador = indicadorHorizontalPrefab;
                    ehHorizontal.Add(true);
                }

                if (prefabIndicador != null)
                {
                    GameObject linha = Instantiate(prefabIndicador, posSpawn, prefabIndicador.transform.rotation);
                    SpriteRenderer srLinha = linha.GetComponent<SpriteRenderer>();
                    if (srLinha != null)
                    {
                        if (direcaoBorda == 1 && dirMov == Vector2.left) srLinha.flipX = true;
                        if (direcaoBorda == 0 && dirMov == Vector2.down) srLinha.flipY = true;
                    }
                    indicadoresCriados.Add(linha);
                }

                posicoesDeAtaque.Add(posSpawn);
                direcoesMovimento.Add(dirMov);
            }

            yield return new WaitForSeconds(tempoStartupTentaculo);

            foreach (GameObject linha in indicadoresCriados)
            {
                if (linha != null) Destroy(linha);
            }

            for (int i = 0; i < posicoesDeAtaque.Count; i++)
            {
                GameObject prefabEscolhido = ehHorizontal[i] ? tentaculoHorizontalPrefab : tentaculoVerticalPrefab;

                if (prefabEscolhido != null)
                {
                    GameObject tentaculo = Instantiate(prefabEscolhido, posicoesDeAtaque[i], prefabEscolhido.transform.rotation);
                    SpriteRenderer srTentaculo = tentaculo.GetComponent<SpriteRenderer>();

                    if (ehHorizontal[i]) 
                    {
                        if (direcoesMovimento[i] == Vector2.left)
                        {
                            if (srTentaculo != null) srTentaculo.flipX = true;
                            else tentaculo.transform.Rotate(0, 180, 0);
                        }
                    }
                    else 
                    {
                        if (direcoesMovimento[i] == Vector2.down)
                        {
                            if (srTentaculo != null) srTentaculo.flipY = true;
                            else tentaculo.transform.Rotate(0, 0, 180);
                        }
                    }

                    GiantTentacle scriptTentaculo = tentaculo.GetComponent<GiantTentacle>();
                    if (scriptTentaculo != null)
                    {
                        scriptTentaculo.tempoAtaqueAtivo = tempoDeVidaTentaculo;
                    }
                }
            } // <-- ESSA CHAVE ESTAVA FALTANDO FECHAR O LOOP DO SPAWN!

            yield return new WaitForSeconds(tempoDeVidaTentaculo + 0.3f);
        }

        animator.SetBool("Ataque_Tentaculo_F2_Loop", false);
        FinalizarAtaqueMecanicamente();
    }

    public void Anim_DispararVelocidadeDash() { } 
    public void Anim_PararVelocidadeDash() { }    

    public void Anim_FinalizarQualquerAtaque()
    {
        if (currentState == BossState.TentaculoAttack || currentState == BossState.GritoAttack || currentState == BossState.Defesa || currentState == BossState.DashAttack) 
            return;

        FinalizarAtaqueMecanicamente();
    }

    private void FinalizarAtaqueMecanicamente()
    {
        if (failsafeCoroutine != null) StopCoroutine(failsafeCoroutine);

        animator.SetBool("Ataque_Tentaculo_F2_Loop", false);
        animator.SetBool("DefesaF1Loop", false);
        animator.SetBool("DashF1Loop", false);
        animator.SetBool("DashF2Loop", false);
        
        executandoMovimentoDash = false;
        if (rb != null) rb.velocity = Vector2.zero;

        ModificarChancesDoAtaque(); 
        currentState = BossState.Idle;
        cooldownTimer = tempoEntreAtaques;
        estaAtacando = false;
        indiceDoAtaqueAtual = -1;
    }

    IEnumerator SegurancaAntiTravamento(float tempoLimite)
    {
        yield return new WaitForSeconds(tempoLimite);

        if (estaAtacando && currentState != BossState.Dead)
        {
            Debug.LogWarning("⚠️");
            currentState = BossState.Idle; 
            FinalizarAtaqueMecanicamente();
        }
    }

    public void RegistrarAcertoNoPlayer() { causouDanoNesteAtaque = true; }

    void ModificarChancesDoAtaque()
    {
        if (indiceDoAtaqueAtual == 3) return;
        float mudanca = causouDanoNesteAtaque ? 5f : -5f;

        switch (indiceDoAtaqueAtual)
        {
            case 0: pesoAsa = Mathf.Clamp(pesoAsa + mudanca, pesoMinimo, pesoMaximo); break;
            case 1: pesoGrito = Mathf.Clamp(pesoGrito + mudanca, pesoMinimo, pesoMaximo); break;
            case 2: pesoDash = Mathf.Clamp(pesoDash + mudanca, pesoMinimo, pesoMaximo); break;
            case 4: pesoEspecial = Mathf.Clamp(pesoEspecial + mudanca, pesoMinimo, pesoMaximo); break;
        }
    }

    void GirarParaOPlayer()
    {
        if (player.position.x > transform.position.x) transform.localRotation = Quaternion.Euler(0, 0, 0); 
        else transform.localRotation = Quaternion.Euler(0, 180, 0); 
    }

    public void MudarParaFase2()
    {
        if (failsafeCoroutine != null) StopCoroutine(failsafeCoroutine);

        currentPhase = BossPhase.Fase2;
        currentState = BossState.Idle;
        estaAtacando = false;
        executandoMovimentoDash = false;
        if (rb != null) rb.velocity = Vector2.zero;
        
        pesoAsa = 25f; pesoGrito = 25f; pesoDash = 25f; pesoEspecial = 25f;

        animator.SetTrigger("ExplodirCabeca"); 
        tempoEntreAtaques = 1.5f; 
    }

    public void AtivarColliderAsa(int ativar) {
        Transform t = transform.Find("Area_Asa");
        if (t != null) t.gameObject.SetActive(ativar == 1);
    }

    public void AtivarColliderGrito(int ativar) {
        Transform t = transform.Find("Area_Grito");
        if (t != null) t.gameObject.SetActive(ativar == 1);
    }
}