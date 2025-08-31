using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton pattern: Bu script'e her yerden kolayca eriþmemizi saðlar
    public static GameManager Instance { get; private set; }

    public int LoggedInAccountId { get; set; }
    public string LoggedInAccountEmail { get; set; }

    private void Awake()
    {
        // Sadece bir tane GameManager olmasýný saðlýyoruz
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Bu objenin sahne geçiþlerinde silinmemesini saðlar
        }
        else
        {
            Destroy(gameObject);
        }
    }
}