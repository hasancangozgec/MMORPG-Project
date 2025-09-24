using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CharacterSelectManager : MonoBehaviour
{
    [Header("Character Display")]
    public Transform characterDisplayPoint;
    public Camera characterCamera;
    
    [Header("Character List UI")]
    public Transform characterListParent;
    public GameObject characterSlotPrefab;
    
    [Header("Character Info Panel")]
    public GameObject characterInfoPanel;
    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI characterClassText;
    public TextMeshProUGUI characterLevelText;
    public TextMeshProUGUI characterStatsText;
    
    [Header("Action Buttons")]
    public Button selectCharacterButton;
    public Button deleteCharacterButton;
    public Button createNewCharacterButton;
    
    [Header("Character Creation Panel")]
    public GameObject characterCreationPanel;
    public TMP_InputField characterNameInput;
    public TMP_Dropdown characterClassDropdown;
    public Button confirmCreateButton;
    public Button cancelCreateButton;
    
    // Current selection
    private CharacterData selectedCharacter;
    private List<CharacterData> availableCharacters;
    private List<GameObject> characterSlots;
    
    private void Start()
    {
        InitializeCharacterSelect();
        SetupUI();
    }
    
    private void InitializeCharacterSelect()
    {
        availableCharacters = new List<CharacterData>();
        characterSlots = new List<GameObject>();
        
        // Subscribe to database events
        if (DatabaseManager.Instance != null)
        {
            DatabaseManager.Instance.OnCharactersLoaded += OnCharactersLoaded;
            DatabaseManager.Instance.OnCharacterCreated += OnCharacterCreated;
        }
        
        // Setup character creation dropdown
        SetupCharacterClassDropdown();
        
        if (GameManager.Instance.isDebugMode)
        {
            Debug.Log("Character Select Manager initialized");
        }
    }
    
    private void SetupUI()
    {
        // Setup button listeners
        if (selectCharacterButton != null)
            selectCharacterButton.onClick.AddListener(OnSelectCharacterClicked);
        
        if (deleteCharacterButton != null)
            deleteCharacterButton.onClick.AddListener(OnDeleteCharacterClicked);
        
        if (createNewCharacterButton != null)
            createNewCharacterButton.onClick.AddListener(OnCreateNewCharacterClicked);
        
        if (confirmCreateButton != null)
            confirmCreateButton.onClick.AddListener(OnConfirmCreateCharacterClicked);
        
        if (cancelCreateButton != null)
            cancelCreateButton.onClick.AddListener(OnCancelCreateCharacterClicked);
        
        // Initially hide character info and creation panels
        if (characterInfoPanel != null) characterInfoPanel.SetActive(false);
        if (characterCreationPanel != null) characterCreationPanel.SetActive(false);
        
        // Disable action buttons initially
        if (selectCharacterButton != null) selectCharacterButton.interactable = false;
        if (deleteCharacterButton != null) deleteCharacterButton.interactable = false;
    }
    
    private void SetupCharacterClassDropdown()
    {
        if (characterClassDropdown != null)
        {
            characterClassDropdown.ClearOptions();
            
            List<string> classOptions = new List<string>();
            foreach (CharacterClass characterClass in System.Enum.GetValues(typeof(CharacterClass)))
            {
                classOptions.Add(characterClass.ToString());
            }
            
            characterClassDropdown.AddOptions(classOptions);
        }
    }
    
    // Event Handlers
    private void OnCharactersLoaded(List<CharacterData> characters)
    {
        availableCharacters = characters;
        UpdateCharacterList();
    }
    
    private void OnCharacterCreated(bool success)
    {
        if (success)
        {
            UIManager.Instance.ShowStatusMessage("Character created successfully!", Color.green);
            HideCharacterCreationPanel();
            
            // Reload characters
            DatabaseManager.Instance.LoadPlayerCharacters();
        }
        else
        {
            UIManager.Instance.ShowStatusMessage("Failed to create character", Color.red);
        }
    }
    
    // Character List Management
    private void UpdateCharacterList()
    {
        // Clear existing slots
        ClearCharacterSlots();
        
        // Create new slots for each character
        foreach (CharacterData character in availableCharacters)
        {
            CreateCharacterSlot(character);
        }
    }
    
    private void CreateCharacterSlot(CharacterData character)
    {
        if (characterSlotPrefab == null || characterListParent == null) return;
        
        GameObject slot = Instantiate(characterSlotPrefab, characterListParent);
        characterSlots.Add(slot);
        
        // Setup slot UI (assuming the prefab has these components)
        TextMeshProUGUI nameText = slot.transform.Find("CharacterName")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI classText = slot.transform.Find("CharacterClass")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI levelText = slot.transform.Find("CharacterLevel")?.GetComponent<TextMeshProUGUI>();
        Button slotButton = slot.GetComponent<Button>();
        
        if (nameText != null) nameText.text = character.characterName;
        if (classText != null) classText.text = character.characterClass.ToString();
        if (levelText != null) levelText.text = $"Level {character.level}";
        
        if (slotButton != null)
        {
            slotButton.onClick.AddListener(() => SelectCharacter(character));
        }
    }
    
    private void ClearCharacterSlots()
    {
        foreach (GameObject slot in characterSlots)
        {
            if (slot != null) Destroy(slot);
        }
        characterSlots.Clear();
    }
    
    // Character Selection
    public void SelectCharacter(CharacterData character)
    {
        selectedCharacter = character;
        UpdateCharacterInfo();
        
        // Enable action buttons
        if (selectCharacterButton != null) selectCharacterButton.interactable = true;
        if (deleteCharacterButton != null) deleteCharacterButton.interactable = true;
        
        if (GameManager.Instance.isDebugMode)
        {
            Debug.Log($"Selected character: {character.characterName}");
        }
    }
    
    private void UpdateCharacterInfo()
    {
        if (selectedCharacter == null) return;
        
        if (characterInfoPanel != null) characterInfoPanel.SetActive(true);
        
        if (characterNameText != null) 
            characterNameText.text = selectedCharacter.characterName;
        
        if (characterClassText != null) 
            characterClassText.text = selectedCharacter.characterClass.ToString();
        
        if (characterLevelText != null) 
            characterLevelText.text = $"Level {selectedCharacter.level}";
        
        if (characterStatsText != null)
        {
            characterStatsText.text = $"Health: {selectedCharacter.health}\n" +
                                    $"Mana: {selectedCharacter.mana}\n" +
                                    $"Strength: {selectedCharacter.strength}\n" +
                                    $"Agility: {selectedCharacter.agility}\n" +
                                    $"Intelligence: {selectedCharacter.intelligence}";
        }
    }
    
    // Button Handlers
    private void OnSelectCharacterClicked()
    {
        if (selectedCharacter == null) return;
        
        UIManager.Instance.ShowLoadingUI("Entering game world...");
        
        // TODO: Load game world with selected character
        // For now, just show a message
        UIManager.Instance.ShowStatusMessage($"Entering game with {selectedCharacter.characterName}!", Color.green);
        
        // Simulate loading delay
        Invoke(nameof(EnterGameWorld), 2f);
    }
    
    private void EnterGameWorld()
    {
        // TODO: Implement actual game world loading
        GameManager.Instance.ChangeGameState(GameManager.GameState.InGame);
    }
    
    private void OnDeleteCharacterClicked()
    {
        if (selectedCharacter == null) return;
        
        // TODO: Implement character deletion with confirmation dialog
        UIManager.Instance.ShowStatusMessage("Character deletion coming soon!", Color.yellow);
    }
    
    private void OnCreateNewCharacterClicked()
    {
        ShowCharacterCreationPanel();
    }
    
    private void OnConfirmCreateCharacterClicked()
    {
        string characterName = characterNameInput?.text ?? "";
        
        if (string.IsNullOrEmpty(characterName))
        {
            UIManager.Instance.ShowStatusMessage("Please enter a character name", Color.red);
            return;
        }
        
        CharacterClass selectedClass = (CharacterClass)characterClassDropdown.value;
        
        CharacterData newCharacter = new CharacterData
        {
            characterName = characterName,
            characterClass = selectedClass,
            level = 1,
            experience = 0,
            health = GetBaseHealth(selectedClass),
            mana = GetBaseMana(selectedClass),
            strength = GetBaseStrength(selectedClass),
            agility = GetBaseAgility(selectedClass),
            intelligence = GetBaseIntelligence(selectedClass),
            position = Vector3.zero
        };
        
        UIManager.Instance.ShowLoadingUI("Creating character...");
        DatabaseManager.Instance.CreateCharacter(newCharacter);
    }
    
    private void OnCancelCreateCharacterClicked()
    {
        HideCharacterCreationPanel();
    }
    
    // Character Creation Panel
    private void ShowCharacterCreationPanel()
    {
        if (characterCreationPanel != null)
        {
            characterCreationPanel.SetActive(true);
            
            // Clear input fields
            if (characterNameInput != null) characterNameInput.text = "";
            if (characterClassDropdown != null) characterClassDropdown.value = 0;
        }
    }
    
    private void HideCharacterCreationPanel()
    {
        if (characterCreationPanel != null)
        {
            characterCreationPanel.SetActive(false);
        }
    }
    
    // Character Stats Calculation
    private int GetBaseHealth(CharacterClass characterClass)
    {
        switch (characterClass)
        {
            case CharacterClass.Warrior: return 120;
            case CharacterClass.Mage: return 80;
            case CharacterClass.Archer: return 100;
            case CharacterClass.Rogue: return 90;
            default: return 100;
        }
    }
    
    private int GetBaseMana(CharacterClass characterClass)
    {
        switch (characterClass)
        {
            case CharacterClass.Warrior: return 30;
            case CharacterClass.Mage: return 100;
            case CharacterClass.Archer: return 50;
            case CharacterClass.Rogue: return 60;
            default: return 50;
        }
    }
    
    private int GetBaseStrength(CharacterClass characterClass)
    {
        switch (characterClass)
        {
            case CharacterClass.Warrior: return 15;
            case CharacterClass.Mage: return 5;
            case CharacterClass.Archer: return 10;
            case CharacterClass.Rogue: return 8;
            default: return 10;
        }
    }
    
    private int GetBaseAgility(CharacterClass characterClass)
    {
        switch (characterClass)
        {
            case CharacterClass.Warrior: return 8;
            case CharacterClass.Mage: return 6;
            case CharacterClass.Archer: return 15;
            case CharacterClass.Rogue: return 12;
            default: return 10;
        }
    }
    
    private int GetBaseIntelligence(CharacterClass characterClass)
    {
        switch (characterClass)
        {
            case CharacterClass.Warrior: return 5;
            case CharacterClass.Mage: return 15;
            case CharacterClass.Archer: return 8;
            case CharacterClass.Rogue: return 10;
            default: return 10;
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (DatabaseManager.Instance != null)
        {
            DatabaseManager.Instance.OnCharactersLoaded -= OnCharactersLoaded;
            DatabaseManager.Instance.OnCharacterCreated -= OnCharacterCreated;
        }
    }
}