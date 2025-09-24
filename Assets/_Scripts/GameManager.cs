using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game Settings")]
    public bool isDebugMode = true;
    public float gameVersion = 1.0f;
    
    [Header("Scene Management")]
    public string loginSceneName = "LoginScene";
    public string characterSelectSceneName = "CharacterSelectScene";
    public string gameWorldSceneName = "GameWorld";
    
    public enum GameState
    {
        Login,
        CharacterSelect,
        InGame,
        Loading
    }
    
    public GameState currentState { get; private set; }
    
    // Events
    public System.Action<GameState> OnGameStateChanged;
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeGame()
    {
        // Set initial game state
        currentState = GameState.Login;
        
        // Initialize other managers
        InitializeManagers();
        
        if (isDebugMode)
        {
            Debug.Log($"Game Manager initialized - Version {gameVersion}");
        }
    }
    
    private void InitializeManagers()
    {
        // Ensure other managers are initialized
        if (DatabaseManager.Instance == null)
        {
            GameObject dbManager = new GameObject("DatabaseManager");
            dbManager.AddComponent<DatabaseManager>();
        }
        
        if (UIManager.Instance == null)
        {
            GameObject uiManager = new GameObject("UIManager");
            uiManager.AddComponent<UIManager>();
        }
    }
    
    public void ChangeGameState(GameState newState)
    {
        if (currentState == newState) return;
        
        GameState previousState = currentState;
        currentState = newState;
        
        if (isDebugMode)
        {
            Debug.Log($"Game state changed from {previousState} to {newState}");
        }
        
        OnGameStateChanged?.Invoke(newState);
        HandleStateChange(newState);
    }
    
    private void HandleStateChange(GameState newState)
    {
        switch (newState)
        {
            case GameState.Login:
                LoadScene(loginSceneName);
                break;
            case GameState.CharacterSelect:
                LoadScene(characterSelectSceneName);
                break;
            case GameState.InGame:
                LoadScene(gameWorldSceneName);
                break;
        }
    }
    
    public void LoadScene(string sceneName)
    {
        if (isDebugMode)
        {
            Debug.Log($"Loading scene: {sceneName}");
        }
        
        currentState = GameState.Loading;
        SceneManager.LoadScene(sceneName);
    }
    
    public void QuitGame()
    {
        if (isDebugMode)
        {
            Debug.Log("Quitting game...");
        }
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        if (isDebugMode)
        {
            Debug.Log($"Game paused: {pauseStatus}");
        }
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        if (isDebugMode)
        {
            Debug.Log($"Game focus: {hasFocus}");
        }
    }
}