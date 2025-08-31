using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.SceneManagement;

public class DatabaseManager : MonoBehaviour
{
    private string apiUrl = "http://localhost/mmo_api/";

    // Bu script artýk UI elemanlarýna doðrudan referans vermeyecek.
    // Bu görev UIManager'a ait.

    public IEnumerator LoginUser(string loginIdentifier, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("login_identifier", loginIdentifier); // PHP'nin beklediði alan adý
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post(apiUrl + "login.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Sunucu Cevabý: " + www.downloadHandler.text);

                LoginResponse response = JsonUtility.FromJson<LoginResponse>(www.downloadHandler.text);

                if (response != null && response.message == "Giriþ baþarýlý")
                {
                    GameManager.Instance.LoggedInAccountId = response.account_id;
                    Debug.Log("Giriþ baþarýlý! CharacterSelectScene yükleniyor...");
                    SceneManager.LoadScene("CharacterSelectScene");
                }
                else
                {
                    Debug.LogWarning("Giriþ yapýlamadý: " + (response != null ? response.message : "Geçersiz sunucu cevabý."));
                }
            }
            else
            {
                Debug.LogError("Giriþ hatasý: " + www.error);
            }
        }
    }

    public IEnumerator RegisterUser(string username, string email, string phone, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username); // PHP'ye yeni alaný gönder
        form.AddField("email", email);
        form.AddField("phone_number", phone);
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post(apiUrl + "register.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Sunucu Cevabý: " + www.downloadHandler.text);
                // Burada kayýt sonrasý otomatik giriþ veya giriþ ekranýna yönlendirme yapýlabilir.
            }
            else
            {
                Debug.LogError("Kayýt hatasý: " + www.error);
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