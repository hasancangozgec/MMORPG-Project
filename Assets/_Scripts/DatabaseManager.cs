using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;

public class DatabaseManager : MonoBehaviour
{
    private string apiUrl = "http://localhost/mmo_api/";

    // Bu script art�k UI elemanlar�na do�rudan referans vermeyecek.
    // Bu g�rev UIManager'a ait.

    public IEnumerator LoginUser(string loginIdentifier, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("login_identifier", loginIdentifier); // PHP'nin bekledi�i alan ad�
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post(apiUrl + "login.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Sunucu Cevab�: " + www.downloadHandler.text);

                LoginResponse response = JsonUtility.FromJson<LoginResponse>(www.downloadHandler.text);

                if (response != null && response.message == "Giri� ba�ar�l�")
                {
                    GameManager.Instance.LoggedInAccountId = response.account_id;
                    Debug.Log("Giri� ba�ar�l�! CharacterSelectScene y�kleniyor...");
                    SceneManager.LoadScene("CharacterSelectScene");
                }
                else
                {
                    Debug.LogWarning("Giri� yap�lamad�: " + (response != null ? response.message : "Ge�ersiz sunucu cevab�."));
                }
            }
            else
            {
                Debug.LogError("Giri� hatas�: " + www.error);
            }
        }
    }

    public IEnumerator RegisterUser(string username, string email, string phone, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username); // PHP'ye yeni alan� g�nder
        form.AddField("email", email);
        form.AddField("phone_number", phone);
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post(apiUrl + "register.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Sunucu Cevab�: " + www.downloadHandler.text);
                // Burada kay�t sonras� otomatik giri� veya giri� ekran�na y�nlendirme yap�labilir.
            }
            else
            {
                Debug.LogError("Kay�t hatas�: " + www.error);
            }
        }
    }

    [System.Serializable]
    private class LoginResponse
    {
        public string message;
        public int account_id;
    }
}