using UnityEngine;
using TMPro; // TextMeshPro kütüphanesini ekliyoruz

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

    // Kayýt Ol butonuna týklandýðýnda çalýþacak
    public void OnRegisterButtonClicked()
    {
        // Alanlarýn boþ olup olmadýðýný kontrol et
        if (registerEmailInput.text == "" || registerPhoneInput.text == "" || registerPasswordInput.text == "" || registerPasswordConfirmInput.text == "")
        {
            Debug.LogWarning("Lütfen tüm alanlarý doldurun.");
            return;
        }

        // Þifrelerin eþleþip eþleþmediðini kontrol et
        if (registerPasswordInput.text != registerPasswordConfirmInput.text)
        {
            Debug.LogWarning("Þifreler eþleþmiyor.");
            return;
        }

        // Her þey yolundaysa, DatabaseManager'daki kayýt fonksiyonunu çalýþtýr
        StartCoroutine(databaseManager.RegisterUser(registerEmailInput.text, registerPhoneInput.text, registerPasswordInput.text));
    }

    // Giriþ Yap butonuna týklandýðýnda çalýþacak
    public void OnLoginButtonClicked()
    {
        if (loginEmailInput.text == "" || loginPasswordInput.text == "")
        {
            Debug.LogWarning("Lütfen tüm alanlarý doldurun.");
            return;
        }

        StartCoroutine(databaseManager.LoginUser(loginEmailInput.text, loginPasswordInput.text));
    }
}