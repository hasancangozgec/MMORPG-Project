using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Paneller")]
    public GameObject loginPanel;
    public GameObject registerPanel;

    [Header("Manager Referanslarý")]
    public DatabaseManager databaseManager;

    [Header("UI Elemanlarý")]
    public TMP_InputField registerUsernameInput; // Yeni eklendi
    public TMP_InputField registerEmailInput;
    public TMP_InputField registerPhoneInput;
    public TMP_InputField registerPasswordInput;
    public TMP_InputField loginIdentifierInput; // Adý deðiþti
    public TMP_InputField loginPasswordInput;

    void Start()
    {
        ShowLoginPanel();
    }

    public void ShowLoginPanel()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
    }

    public void ShowRegisterPanel()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
    }

    public void OnLoginButtonClicked()
    {
        // Gerekli kontroller yapýlabilir (alanlar boþ mu vb.)
        StartCoroutine(databaseManager.LoginUser(loginIdentifierInput.text, loginPasswordInput.text));
    }

    public void OnRegisterButtonClicked()
    {
        // Gerekli kontroller yapýlabilir
        StartCoroutine(databaseManager.RegisterUser(registerUsernameInput.text, registerEmailInput.text, registerPhoneInput.text, registerPasswordInput.text));
    }
}