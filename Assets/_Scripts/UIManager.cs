using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Paneller")]
    public GameObject loginPanel;
    public GameObject registerPanel;

    [Header("Manager Referanslar�")]
    public DatabaseManager databaseManager;

    [Header("UI Elemanlar�")]
    public TMP_InputField registerUsernameInput; // Yeni eklendi
    public TMP_InputField registerEmailInput;
    public TMP_InputField registerPhoneInput;
    public TMP_InputField registerPasswordInput;
    public TMP_InputField loginIdentifierInput; // Ad� de�i�ti
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
        // Gerekli kontroller yap�labilir (alanlar bo� mu vb.)
        StartCoroutine(databaseManager.LoginUser(loginIdentifierInput.text, loginPasswordInput.text));
    }

    public void OnRegisterButtonClicked()
    {
        // Gerekli kontroller yap�labilir
        StartCoroutine(databaseManager.RegisterUser(registerUsernameInput.text, registerEmailInput.text, registerPhoneInput.text, registerPasswordInput.text));
    }
}