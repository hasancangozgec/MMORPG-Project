using UnityEngine;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }
    
    [Header("Network Settings")]
    public bool isServer = false;
    public bool isClient = true;
    public string serverAddress = "localhost";
    public int serverPort = 7777;
    public int maxConnections = 100;
    
    [Header("Connection Status")]
    public bool isConnected = false;
    public float connectionTimeout = 10f;
    
    // Connected players
    private Dictionary<string, NetworkPlayer> connectedPlayers;
    
    // Events
    public System.Action OnConnectedToServer;
    public System.Action OnDisconnectedFromServer;
    public System.Action<NetworkPlayer> OnPlayerJoined;
    public System.Action<NetworkPlayer> OnPlayerLeft;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeNetwork();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializeNetwork()
    {
        connectedPlayers = new Dictionary<string, NetworkPlayer>();
        
        if (GameManager.Instance.isDebugMode)
        {
            Debug.Log("Network Manager initialized");
        }
    }
    
    private void Start()
    {
        // Auto-connect in development mode
        if (GameManager.Instance.isDebugMode && isClient)
        {
            // For now, simulate connection
            SimulateConnection();
        }
    }
    
    // Connection Methods
    public void ConnectToServer()
    {
        if (isConnected)
        {
            Debug.LogWarning("Already connected to server");
            return;
        }
        
        if (GameManager.Instance.isDebugMode)
        {
            Debug.Log($"Connecting to server: {serverAddress}:{serverPort}");
        }
        
        // TODO: Implement actual networking (Unity Netcode, Mirror, etc.)
        SimulateConnection();
    }
    
    public void DisconnectFromServer()
    {
        if (!isConnected)
        {
            Debug.LogWarning("Not connected to server");
            return;
        }
        
        if (GameManager.Instance.isDebugMode)
        {
            Debug.Log("Disconnecting from server");
        }
        
        isConnected = false;
        connectedPlayers.Clear();
        OnDisconnectedFromServer?.Invoke();
    }
    
    private void SimulateConnection()
    {
        // Simulate connection delay
        Invoke(nameof(CompleteConnection), 1f);
    }
    
    private void CompleteConnection()
    {
        isConnected = true;
        OnConnectedToServer?.Invoke();
        
        if (GameManager.Instance.isDebugMode)
        {
            Debug.Log("Connected to server (simulated)");
        }
    }
    
    // Player Management
    public void RegisterPlayer(NetworkPlayer player)
    {
        if (player == null || string.IsNullOrEmpty(player.playerId)) return;
        
        if (!connectedPlayers.ContainsKey(player.playerId))
        {
            connectedPlayers.Add(player.playerId, player);
            OnPlayerJoined?.Invoke(player);
            
            if (GameManager.Instance.isDebugMode)
            {
                Debug.Log($"Player joined: {player.playerName} ({player.playerId})");
            }
        }
    }
    
    public void UnregisterPlayer(string playerId)
    {
        if (connectedPlayers.TryGetValue(playerId, out NetworkPlayer player))
        {
            connectedPlayers.Remove(playerId);
            OnPlayerLeft?.Invoke(player);
            
            if (GameManager.Instance.isDebugMode)
            {
                Debug.Log($"Player left: {player.playerName} ({playerId})");
            }
        }
    }
    
    public NetworkPlayer GetPlayer(string playerId)
    {
        connectedPlayers.TryGetValue(playerId, out NetworkPlayer player);
        return player;
    }
    
    public List<NetworkPlayer> GetAllPlayers()
    {
        return new List<NetworkPlayer>(connectedPlayers.Values);
    }
    
    // Message Sending (placeholder for actual networking)
    public void SendPlayerPosition(Vector3 position, Quaternion rotation)
    {
        if (!isConnected) return;
        
        // TODO: Implement actual network message sending
        if (GameManager.Instance.isDebugMode)
        {
            //Debug.Log($"Sending position: {position}");
        }
    }
    
    public void SendChatMessage(string message)
    {
        if (!isConnected) return;
        
        if (GameManager.Instance.isDebugMode)
        {
            Debug.Log($"Sending chat message: {message}");
        }
        
        // TODO: Implement actual chat system
    }
    
    public void SendPlayerAction(string action, object data = null)
    {
        if (!isConnected) return;
        
        if (GameManager.Instance.isDebugMode)
        {
            Debug.Log($"Sending player action: {action}");
        }
        
        // TODO: Implement actual action system
    }
    
    // Server Methods (for future server implementation)
    public void StartServer()
    {
        if (GameManager.Instance.isDebugMode)
        {
            Debug.Log("Starting server...");
        }
        
        isServer = true;
        // TODO: Implement server startup
    }
    
    public void StopServer()
    {
        if (GameManager.Instance.isDebugMode)
        {
            Debug.Log("Stopping server...");
        }
        
        isServer = false;
        // TODO: Implement server shutdown
    }
    
    private void OnDestroy()
    {
        if (isConnected)
        {
            DisconnectFromServer();
        }
    }
}

// Network Player Data
[System.Serializable]
public class NetworkPlayer
{
    public string playerId;
    public string playerName;
    public Vector3 position;
    public Quaternion rotation;
    public CharacterData characterData;
    public float lastUpdateTime;
    
    public NetworkPlayer(string id, string name, CharacterData charData)
    {
        playerId = id;
        playerName = name;
        characterData = charData;
        position = Vector3.zero;
        rotation = Quaternion.identity;
        lastUpdateTime = Time.time;
    }
}