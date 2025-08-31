using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton pattern: Bu script'e her yerden kolayca eri�memizi sa�lar
    public static GameManager Instance { get; private set; }

    public int LoggedInAccountId { get; set; }
    public string LoggedInAccountEmail { get; set; }

    private void Awake()
    {
        // Sadece bir tane GameManager olmas�n� sa�l�yoruz
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Bu objenin sahne ge�i�lerinde silinmemesini sa�lar
        }
        else
        {
            Destroy(gameObject);
        }
    }
}