using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponSwitcher : MonoBehaviour
{
    [Header("Weapons")]
    public GameObject pistol;
    public GameObject shotgun;

    [Header("UI")]
    public Image pistolIndicator;   // Indicador da pistola
    public Image shotgunIndicator;  // Indicador da escopeta

    private GameObject activeWeapon;

    void Start()
    {
        activeWeapon = pistol;

        pistol.SetActive(true);
        shotgun.SetActive(false);

        UpdateIndicators();
        UpdateAmmoUI();
    }

    void Update()
    {
        // Teclas numéricas normais
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchWeapon(pistol);
            UpdateIndicators();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchWeapon(shotgun);
            UpdateIndicators();
        }

        // ==================== SISTEMA DE QUICK SWITCH ====================
        // Checa se apertou Q ou o Clique do Meio do Mouse (botão 2)
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetMouseButtonDown(2))
        {
            AlternarArmaAtual();
        }
    }

    // Função que faz o "Toggle" inteligente entre as duas armas
    void AlternarArmaAtual()
    {
        if (activeWeapon == pistol)
        {
            SwitchWeapon(shotgun);
        }
        else
        {
            SwitchWeapon(pistol);
        }

        UpdateIndicators();
    }

    void SwitchWeapon(GameObject newWeapon)
    {
        if (activeWeapon == newWeapon)
            return;

        activeWeapon.SetActive(false);
        newWeapon.SetActive(true);
        activeWeapon = newWeapon;

        UpdateAmmoUI();
    }

    void UpdateIndicators()
    {
        pistolIndicator.gameObject.SetActive(activeWeapon == pistol);
        shotgunIndicator.gameObject.SetActive(activeWeapon == shotgun);
    }

    void UpdateAmmoUI()
    {
        Gun gun = activeWeapon.GetComponent<Gun>();
        if (gun != null)
        {
            gun.UpdateAmmoUI();
            return;
        }

        Escopeta escopeta = activeWeapon.GetComponent<Escopeta>();
        if (escopeta != null)
        {
            escopeta.UpdateAmmoUI();
            return;
        }
    }
}