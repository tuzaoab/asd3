using UnityEngine;
using TMPro;

public class GerenciadorMissao : MonoBehaviour
{
    public static GerenciadorMissao Instancia;

    [Header("Configuração da UI")]
    public TextMeshProUGUI textoContador; // Arraste o seu TextoContadorZumbis aqui

    [Header("Metas da Fase")]
    public int zumbisNecessarios = 50;
    private int zumbisMortosAtual = 0;

    void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Garante que o texto começa desativado ao abrir o jogo no menu
        if (textoContador != null)
        {
            textoContador.gameObject.SetActive(false);
        }
    }

    // Chamado pelo MainMenu quando você clica em Start
    public void IniciarMissao()
    {
        if (textoContador != null)
        {
            textoContador.gameObject.SetActive(true); // Ativa o texto na tela do jogo
        }
        AtualizarTextoUI();
    }

    public void RegistrarMorteZumbi()
    {
        if (zumbisMortosAtual < zumbisNecessarios)
        {
            zumbisMortosAtual++;
            AtualizarTextoUI();

            if (zumbisMortosAtual >= zumbisNecessarios)
            {
                CompletarMissao(); // Corrigido aqui (adicionado o "r")
            }
        }
    }

    private void AtualizarTextoUI()
    {
        if (textoContador != null)
        {
            textoContador.text = zumbisMortosAtual + "/" + zumbisNecessarios;
        }
    }

    private void CompletarMissao()
    {
        Debug.Log("Missão concluída! Meta de zumbis atingida.");
    }
}