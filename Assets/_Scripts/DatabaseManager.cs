using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class DatabaseManager : MonoBehaviour
{
    private string apiUrl = "http://localhost/mmo_api/";

    // Kayýt olma fonksiyonu
    public IEnumerator RegisterUser(string email, string phone, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("phone", phone);
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post(apiUrl + "register.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Bir hata oluþtu: " + www.error);
            }
            else
            {
                Debug.Log("Sunucu Cevabý: " + www.downloadHandler.text);
                // Burada sunucudan gelen cevaba göre iþlem yapabiliriz.
                // Örneðin: if (www.downloadHandler.text.Contains("Success")) { ... }
            }
        }
    }

    // Giriþ yapma fonksiyonu
    public IEnumerator LoginUser(string email, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post(apiUrl + "login.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Bir hata oluþtu: " + www.error);
            }
            else
            {
                Debug.Log("Sunucu Cevabý: " + www.downloadHandler.text);
            }
        }
    }
}