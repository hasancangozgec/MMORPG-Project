using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

[System.Serializable]
public class CharacterData
{
    public int character_id;
    public string character_name;
    public string race;
    public string @class;
    public int level;
    public string gender;
}

[System.Serializable]
public class CharacterList
{
    public List<CharacterData> characters;
}

public class CharacterSelectManager : MonoBehaviour
{
    private string apiUrl = "http://localhost/mmo_api/";
    private List<CharacterData> playerCharacters = new List<CharacterData>();

    [Header("UI Panelleri")]
    public GameObject characterListPanel;
    public GameObject characterCreatePanel;

    [Header("Karakter Slotlar�")]
    public TextMeshProUGUI[] characterSlotTexts;
    public GameObject newCharacterButton;

    [Header("Karakter Olu�turma Alanlar�")]
    public TMP_InputField nameInput;
    public TMP_Dropdown raceDropdown;
    public TMP_Dropdown classDropdown;
    public TMP_Dropdown genderDropdown;

    void Start()
    {
        characterCreatePanel.SetActive(false);
        characterListPanel.SetActive(true);

        if (GameManager.Instance == null)
        {
            Debug.LogError("HATA: GameManager bulunamad�! L�tfen LoginScene'den oyunu ba�lat�n.");
            return;
        }

        StartCoroutine(GetCharacters());
    }

    #region UI Fonksiyonlar�

    public void ShowCreateCharacterPanel()
    {
        characterListPanel.SetActive(false);
        characterCreatePanel.SetActive(true);
    }

    public void ShowCharacterListPanel()
    {
        characterCreatePanel.SetActive(false);
        characterListPanel.SetActive(true);
        StartCoroutine(GetCharacters());
    }

    private void UpdateCharacterSlotsUI()
    {
        for (int i = 0; i < characterSlotTexts.Length; i++)
        {
            characterSlotTexts[i].text = "[BO� SLOT]";
        }

        for (int i = 0; i < playerCharacters.Count; i++)
        {
            if (i < characterSlotTexts.Length)
            {
                CharacterData c = playerCharacters[i];
                characterSlotTexts[i].text = $"{c.character_name}\nIrk: {c.race} | S�n�f: {c.@class}\nSeviye: {c.level}";
            }
        }

        newCharacterButton.SetActive(playerCharacters.Count < 3);
    }

    #endregion

    #region Veritaban� ��lemleri (Coroutines)

    public void OnCreateCharacterClicked()
    {
        if (nameInput == null || raceDropdown == null || classDropdown == null || genderDropdown == null)
        {
            Debug.LogError("HATA: Karakter olu�turma aray�z alanlar� (InputField, Dropdown vb.) Inspector'da atanmam��!");
            return;
        }

        string charName = nameInput.text;
        string charRace = raceDropdown.options[raceDropdown.value].text;
        string charClass = classDropdown.options[classDropdown.value].text;
        string charGender = genderDropdown.options[genderDropdown.value].text;

        StartCoroutine(CreateCharacter(charName, charRace, charClass, charGender));
    }

    private IEnumerator GetCharacters()
    {
        WWWForm form = new WWWForm();
        form.AddField("account_id", GameManager.Instance.LoggedInAccountId);

        using (UnityWebRequest www = UnityWebRequest.Post(apiUrl + "get_characters.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                // YEN� KOD: Sunucudan gelen cevab�n bo� olup olmad���n� kontrol et
                if (string.IsNullOrEmpty(www.downloadHandler.text) || www.downloadHandler.text == "[]")
                {
                    // Hi� karakter yok, listeyi temizle ve aray�z� g�ncelle
                    playerCharacters = new List<CharacterData>();
                    UpdateCharacterSlotsUI();
                }
                else
                {
                    // Karakter(ler) var, JSON olarak ayr��t�r
                    string jsonResponse = "{\"characters\":" + www.downloadHandler.text + "}";
                    CharacterList list = JsonUtility.FromJson<CharacterList>(jsonResponse);
                    playerCharacters = list.characters;
                    UpdateCharacterSlotsUI();
                }
            }
            else
            {
                Debug.LogError("Karakterler al�namad�: " + www.error);
            }
        }
    }

    private IEnumerator CreateCharacter(string name, string race, string c_class, string gender)
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("HATA: GameManager bulunamad�! LoginScene'de do�ru kuruldu�undan emin olun.");
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("account_id", GameManager.Instance.LoggedInAccountId);
        form.AddField("name", name);
        form.AddField("race", race);
        form.AddField("class", c_class);
        form.AddField("gender", gender);

        using (UnityWebRequest www = UnityWebRequest.Post(apiUrl + "create_character.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Sunucu Cevab�: " + www.downloadHandler.text);
                if (www.downloadHandler.text.Contains("Success"))
                {
                    ShowCharacterListPanel();
                }
            }
            else
            {
                Debug.LogError("Karakter olu�turulamad�: " + www.error);
            }
        }
    }
    #endregion
}
