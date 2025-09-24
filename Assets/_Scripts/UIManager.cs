using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [Header("UI References")]
    public Canvas mainCanvas;
    public GraphicRaycaster graphicRaycaster;
    
    [Header("Login UI")]
    public GameObject loginPanel;
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public Button registerButton;
    public TextMeshProUGUI statusText;
    
    [Header("Character Select UI")]
    public GameObject characterSelectPanel;
    public Transform characterListParent;
    public Button createCharacterButton;
    public Button backToLoginButton;
    
    [Header("Loading UI")]
    public GameObject loadingPanel;
    public TextMeshProUGUI loadingText;
    public Slider loadingProgressBar;
    
    // UI State
    private bool isUILocked = false;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeUI();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // Subscribe to events
        if (DatabaseManager.Instance != null)
        {
            DatabaseManager.Instance.OnLoginResult += HandleLoginResult;
            DatabaseManager.Instance.OnRegistrationResult += HandleRegistrationResult;
            DatabaseManager.Instance.OnCharactersLoaded += HandleCharactersLoaded;
        }
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        }
    }
    
    private void InitializeUI()
    {
        // Find main canvas if not assigned
        if (mainCanvas == null)
        {
            mainCanvas = FindObjectOfType<Canvas>();
        }
        
        // Setup initial UI state
        ShowLoginUI();
        
        if (GameManager.Instance.isDebugMode)
        {
            Debug.Log("UI Manager initialized");
        }
    }
    
    // UI State Management
    public void ShowLoginUI()
    {
        HideAllPanels();
        if (loginPanel != null) loginPanel.SetActive(true);
        SetUILocked(false);
    }
    
    public void ShowCharacterSelectUI()
    {
        HideAllPanels();
        if (characterSelectPanel != null) characterSelectPanel.SetActive(true);
        SetUILocked(false);
    }
    
    public void ShowLoadingUI(string message = "Loading...")
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
            if (loadingText != null) loadingText.text = message;
        }
        SetUILocked(true);
    }
    
    public void HideLoadingUI()
    {
        if (loadingPanel != null) loadingPanel.SetActive(false);
        SetUILocked(false);
    }
    
    private void HideAllPanels()
    {
        if (loginPanel != null) loginPanel.SetActive(false);
        if (characterSelectPanel != null) characterSelectPanel.SetActive(false);
        if (loadingPanel != null) loadingPanel.SetActive(false);
    }
    
    public void SetUILocked(bool locked)
    {
        isUILocked = locked;
        
        if (graphicRaycaster != null)
        {
            graphicRaycaster.enabled = !locked;
        }
    }
    
    // Login UI Methods
    public void OnLoginButtonClicked()
    {
        if (isUILocked) return;
        
        string email = emailInput?.text ?? "";
        string password = passwordInput?.text ?? "";
        
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowStatusMessage("Please enter both email and password", Color.red);
            return;
        }
        
        ShowLoadingUI("Logging in...");
        DatabaseManager.Instance.LoginUser(email, password);
    }
    
    public void OnRegisterButtonClicked()
    {
        if (isUILocked) return;
        
        string email = emailInput?.text ?? "";
        string password = passwordInput?.text ?? "";
        
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowStatusMessage("Please enter both email and password", Color.red);
            return;
        }
        
        ShowLoadingUI("Creating account...");
        string username = email.Split('@')[0]; // Simple username from email
        DatabaseManager.Instance.RegisterUser(email, password, username);
    }
    
    // Character Select UI Methods
    public void OnCreateCharacterButtonClicked()
    {
        if (isUILocked) return;
        
        // TODO: Open character creation UI
        ShowStatusMessage("Character creation coming soon!", Color.yellow);
    }
    
    public void OnBackToLoginButtonClicked()
    {
        if (isUILocked) return;
        
        DatabaseManager.Instance.Logout();
        GameManager.Instance.ChangeGameState(GameManager.GameState.Login);
    }
    
    // Event Handlers
    private void HandleLoginResult(bool success)
    {
        HideLoadingUI();
        
        if (success)
        {
            ShowStatusMessage("Login successful!", Color.green);
            GameManager.Instance.ChangeGameState(GameManager.GameState.CharacterSelect);
        }
        else
        {
            ShowStatusMessage("Login failed. Please check your credentials.", Color.red);
        }
    }
    
    private void HandleRegistrationResult(bool success)
    {
        HideLoadingUI();
        
        if (success)
        {
            ShowStatusMessage("Registration successful! Please login.", Color.green);
        }
        else
        {
            ShowStatusMessage("Registration failed. Please try again.", Color.red);
        }
    }
    
    private void HandleCharactersLoaded(List<CharacterData> characters)
    {
        HideLoadingUI();
        
        if (GameManager.Instance.isDebugMode)
        {
            Debug.Log($"Loaded {characters.Count} characters");
        }
        
        // TODO: Populate character list UI
        UpdateCharacterList(characters);
    }
    
    private void HandleGameStateChanged(GameManager.GameState newState)
    {
        switch (newState)
        {
            case GameManager.GameState.Login:
                ShowLoginUI();
                break;
            case GameManager.GameState.CharacterSelect:
                ShowCharacterSelectUI();
                DatabaseManager.Instance.LoadPlayerCharacters();
                break;
            case GameManager.GameState.Loading:
                ShowLoadingUI();
                break;
        }
    }
    
    // Helper Methods
    public void ShowStatusMessage(string message, Color color)
    {
        if (statusText != null)
        {
            statusText.text = message;
            statusText.color = color;
            
            // Clear message after 3 seconds
            CancelInvoke(nameof(ClearStatusMessage));
            Invoke(nameof(ClearStatusMessage), 3f);
        }
        
        if (GameManager.Instance.isDebugMode)
        {
            Debug.Log($"Status: {message}");
        }
    }
    
    private void ClearStatusMessage()
    {
        if (statusText != null)
        {
            statusText.text = "";
        }
    }
    
    private void UpdateCharacterList(List<CharacterData> characters)
    {
        // TODO: Implement character list UI population
        if (characters.Count == 0)
        {
            ShowStatusMessage("No characters found. Create your first character!", Color.yellow);
        }
    }
    
    public void UpdateLoadingProgress(float progress, string message = "")
    {
        if (loadingProgressBar != null)
        {
            loadingProgressBar.value = progress;
        }
        
        if (!string.IsNullOrEmpty(message) && loadingText != null)
        {
            loadingText.text = message;
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (DatabaseManager.Instance != null)
        {
            DatabaseManager.Instance.OnLoginResult -= HandleLoginResult;
            DatabaseManager.Instance.OnRegistrationResult -= HandleRegistrationResult;
            DatabaseManager.Instance.OnCharactersLoaded -= HandleCharactersLoaded;
        }
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
        }
    }
}