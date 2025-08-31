using UnityEngine;
using TMPro; // TextMeshPro k�t�phanesini ekliyoruz

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject loginPanel;
    public GameObject registerPanel;

    [Header("Login Fields")]
    public TMP_InputField loginEmailInput;
    public TMP_InputField loginPasswordInput;

    [Header("Register Fields")]
    public TMP_InputField registerEmailInput;
    public TMP_InputField registerPhoneInput;
    public TMP_InputField registerPasswordInput;
    public TMP_InputField registerPasswordConfirmInput;

    // DatabaseManager script'ine referans
    private DatabaseManager databaseManager;

    void Start()
    {
        // Managers objesindeki DatabaseManager script'ini bul ve ata
        databaseManager = FindObjectOfType<DatabaseManager>();
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

    // Kay�t Ol butonuna t�kland���nda �al��acak
    public void OnRegisterButtonClicked()
    {
        // Alanlar�n bo� olup olmad���n� kontrol et
        if (registerEmailInput.text == "" || registerPhoneInput.text == "" || registerPasswordInput.text == "" || registerPasswordConfirmInput.text == "")
        {
            Debug.LogWarning("L�tfen t�m alanlar� doldurun.");
            return;
        }

        // �ifrelerin e�le�ip e�le�medi�ini kontrol et
        if (registerPasswordInput.text != registerPasswordConfirmInput.text)
        {
            Debug.LogWarning("�ifreler e�le�miyor.");
            return;
        }

        // Her �ey yolundaysa, DatabaseManager'daki kay�t fonksiyonunu �al��t�r
        StartCoroutine(databaseManager.RegisterUser(registerEmailInput.text, registerPhoneInput.text, registerPasswordInput.text));
    }

    // Giri� Yap butonuna t�kland���nda �al��acak
    public void OnLoginButtonClicked()
    {
        if (loginEmailInput.text == "" || loginPasswordInput.text == "")
        {
            Debug.LogWarning("L�tfen t�m alanlar� doldurun.");
            return;
        }

        StartCoroutine(databaseManager.LoginUser(loginEmailInput.text, loginPasswordInput.text));
    }
}