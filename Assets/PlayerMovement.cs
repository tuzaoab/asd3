using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class PlayerMovement : MonoBehaviour
{
    public Character Character;
    public float walkSpeed = 3f;
    public float runSpeed = 5f;
    
    [Header("Limites de Movimentação")]
    public float minY = -2f, maxY = 2f;
    public float minX = -100f, maxX = 100f;

    public float dashSpeed = 12f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.4f;

    public ParticleSystem dashParticles;

    [Header("Configurações de Áudio")]
    public AudioSource audioSource;
    public AudioClip somPassos;
    public AudioClip somDash;
    [Range(0f, 1f)] public float volumeCaminhada = 0.4f;
    [Range(0f, 1f)] public float volumeCorrida = 0.7f;
    [Range(0f, 1f)] public float volumeDash = 0.8f;
    public float pitchCaminhada = 1.0f;
    public float pitchCorrida = 1.3f;

    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer playerSprite;
    Vector2 input;
    float currentSpeed;

    bool isDashing;
    float dashTime;
    float dashCooldownTimer;
    Vector2 dashDir;

    // NOVA VARIÁVEL: Controla o estado de liga/desliga da corrida
    private bool isSprinting = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerSprite = GetComponent<SpriteRenderer>();

        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        if (audioSource != null)
        {
            audioSource.clip = somPassos;
            audioSource.loop = true;
            audioSource.playOnAwake = false;
        }
    }

    void Update()
    {
        if (Character != null && Character.isGameOver)
        {
            PararSomDePassos();
            return;
        }

        dashCooldownTimer -= Time.deltaTime;

        if (!isDashing)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            // AJUSTE: Verifica se o jogador apertou o Shift para inverter o Toggle
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                isSprinting = !isSprinting;
            }

            bool moving = input.sqrMagnitude > 0.01f;

            // Se o jogador parar de andar, podemos resetar a corrida? 
            // Opcional: Se quiser que ele continue correndo assim que voltar a andar, mantenha como está.
            // Se quiser que a corrida desligue ao parar de se mover, descomente a linha abaixo:
            // if (!moving) isSprinting = false;

            currentSpeed = isSprinting ? runSpeed : walkSpeed;
            input = input.normalized * currentSpeed;

            animator.SetBool("IsRunning", isSprinting && moving);
            animator.SetBool("IsWalking", !isSprinting && moving);

            if (moving) GerenciarSomDePassos(isSprinting);
            else PararSomDePassos();

            HandleCharacterFlip();

            // MUDANÇA DOS BOTÕES DO DASH: Aceita Q, Botão Direito do Mouse (1) ou Alt (esquerdo ou direito)
            bool apertouBotaoDash =
                                   Input.GetMouseButtonDown(1) || 
                                   Input.GetKeyDown(KeyCode.LeftAlt) || 
                                   Input.GetKeyDown(KeyCode.RightAlt);

            if (apertouBotaoDash && moving && dashCooldownTimer <= 0)
            {
                ExecutarDash();
            }
        }
        else
        {
            dashTime -= Time.deltaTime;
            if (dashTime <= 0)
            {
                isDashing = false;
                Character.isInvincible = false;
                if (dashParticles != null) dashParticles.Stop();
            }
        }
    }

    void HandleCharacterFlip()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (mousePos.x > transform.position.x)
        {
            playerSprite.flipX = false;
        }
        else
        {
            playerSprite.flipX = true;
        }
    }

    void GerenciarSomDePassos(bool correndo)
    {
        if (audioSource == null || somPassos == null) return;
        if (!audioSource.isPlaying) audioSource.Play();
        audioSource.volume = correndo ? volumeCorrida : volumeCaminhada;
        audioSource.pitch = correndo ? pitchCorrida : pitchCaminhada;
    }

    void PararSomDePassos()
    {
        if (audioSource != null && audioSource.isPlaying) audioSource.Stop();
    }

    void ExecutarDash()
    {
        PararSomDePassos();
        if (audioSource != null && somDash != null)
            audioSource.PlayOneShot(somDash, volumeDash);

        dashCooldownTimer = dashCooldown;
        dashTime = dashDuration;
        isDashing = true;
        
        // Salvamos a direção baseado na velocidade e direção atuais
        dashDir = input.normalized;

        if ((dashDir.y > 0 && rb.position.y >= maxY) || (dashDir.y < 0 && rb.position.y <= minY))
            dashDir.y = 0;
        if ((dashDir.x > 0 && rb.position.x >= maxX) || (dashDir.x < 0 && rb.position.x <= minX))
            dashDir.x = 0;

        Character.isInvincible = true;

        if (dashParticles != null)
        {
            var vol = dashParticles.velocityOverLifetime;
            vol.enabled = true;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            vol.x = new ParticleSystem.MinMaxCurve(mousePos.x > transform.position.x ? -5f : 5f);
            dashParticles.Play();
        }
    }

    void FixedUpdate()
    {
        Vector2 velocity = isDashing ? dashDir * dashSpeed : input;
        Vector2 newPos = rb.position + velocity * Time.fixedDeltaTime;
        
        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
        
        rb.MovePosition(newPos);
    }

    public void AlterarLimitesDeMovimento(float novoMinX, float novoMaxX, float novoMinY, float novoMaxY)
    {
        minX = novoMinX;
        maxX = novoMaxX;
        minY = novoMinY;
        maxY = novoMaxY;
    }
}