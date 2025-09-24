using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Base Stats")]
    public int level = 1;
    public int experience = 0;
    public int experienceToNextLevel = 100;
    
    [Header("Health & Mana")]
    public int maxHealth = 100;
    public int currentHealth = 100;
    public int maxMana = 50;
    public int currentMana = 50;
    public float healthRegenRate = 1f;
    public float manaRegenRate = 2f;
    
    [Header("Primary Stats")]
    public int strength = 10;
    public int agility = 10;
    public int intelligence = 10;
    public int vitality = 10;
    
    [Header("Secondary Stats")]
    public int attackPower = 10;
    public int defense = 5;
    public int criticalChance = 5;
    public int criticalDamage = 150;
    
    // Character data reference
    private CharacterData characterData;
    
    // Regeneration timers
    private float healthRegenTimer;
    private float manaRegenTimer;
    
    // Events
    public System.Action<int, int> OnHealthChanged;
    public System.Action<int, int> OnManaChanged;
    public System.Action<int> OnLevelUp;
    public System.Action<int, int> OnExperienceChanged;
    public System.Action OnStatsChanged;
    
    private void Start()
    {
        InitializeStats();
    }
    
    private void Update()
    {
        HandleRegeneration();
    }
    
    private void InitializeStats()
    {
        // Calculate secondary stats from primary stats
        RecalculateStats();
        
        // Initialize health and mana to max
        currentHealth = maxHealth;
        currentMana = maxMana;
        
        if (GameManager.Instance.isDebugMode)
        {
            Debug.Log($"Player stats initialized - Level {level}");
        }
    }
    
    public void LoadFromCharacterData(CharacterData data)
    {
        if (data == null) return;
        
        characterData = data;
        
        level = data.level;
        experience = data.experience;
        maxHealth = data.health;
        currentHealth = maxHealth;
        maxMana = data.mana;
        currentMana = maxMana;
        strength = data.strength;
        agility = data.agility;
        intelligence = data.intelligence;
        
        RecalculateStats();
        
        if (GameManager.Instance.isDebugMode)
        {
            Debug.Log($"Loaded stats for character: {data.characterName}");
        }
    }
    
    private void RecalculateStats()
    {
        // Calculate max health and mana based on stats
        maxHealth = 100 + (vitality * 10) + (level * 5);
        maxMana = 50 + (intelligence * 5) + (level * 2);
        
        // Calculate attack power
        attackPower = strength + (level * 2);
        
        // Calculate defense
        defense = vitality + (level * 1);
        
        // Calculate critical chance (agility based)
        criticalChance = 5 + (agility / 2);
        
        // Ensure current values don't exceed max
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        currentMana = Mathf.Min(currentMana, maxMana);
        
        // Calculate experience needed for next level
        experienceToNextLevel = level * 100 + (level * level * 10);
        
        OnStatsChanged?.Invoke();
    }
    
    private void HandleRegeneration()
    {
        // Health regeneration
        if (currentHealth < maxHealth)
        {
            healthRegenTimer += Time.deltaTime;
            if (healthRegenTimer >= 1f / healthRegenRate)
            {
                ModifyHealth(1);
                healthRegenTimer = 0f;
            }
        }
        
        // Mana regeneration
        if (currentMana < maxMana)
        {
            manaRegenTimer += Time.deltaTime;
            if (manaRegenTimer >= 1f / manaRegenRate)
            {
                ModifyMana(1);
                manaRegenTimer = 0f;
            }
        }
    }
    
    // Health & Mana Management
    public void ModifyHealth(int amount)
    {
        int previousHealth = currentHealth;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        
        if (currentHealth != previousHealth)
        {
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            
            if (currentHealth <= 0)
            {
                HandleDeath();
            }
        }
    }
    
    public void ModifyMana(int amount)
    {
        int previousMana = currentMana;
        currentMana = Mathf.Clamp(currentMana + amount, 0, maxMana);
        
        if (currentMana != previousMana)
        {
            OnManaChanged?.Invoke(currentMana, maxMana);
        }
    }
    
    public void SetHealthPercentage(float percentage)
    {
        int newHealth = Mathf.RoundToInt(maxHealth * Mathf.Clamp01(percentage));
        ModifyHealth(newHealth - currentHealth);
    }
    
    public void SetManaPercentage(float percentage)
    {
        int newMana = Mathf.RoundToInt(maxMana * Mathf.Clamp01(percentage));
        ModifyMana(newMana - currentMana);
    }
    
    // Experience & Leveling
    public void AddExperience(int amount)
    {
        if (amount <= 0) return;
        
        experience += amount;
        OnExperienceChanged?.Invoke(experience, experienceToNextLevel);
        
        // Check for level up
        while (experience >= experienceToNextLevel)
        {
            LevelUp();
        }
        
        if (GameManager.Instance.isDebugMode)
        {
            Debug.Log($"Gained {amount} experience. Total: {experience}");
        }
    }
    
    private void LevelUp()
    {
        experience -= experienceToNextLevel;
        level++;
        
        // Increase stats on level up
        strength += GetStatIncreaseForClass(characterData?.characterClass ?? CharacterClass.Warrior, "strength");
        agility += GetStatIncreaseForClass(characterData?.characterClass ?? CharacterClass.Warrior, "agility");
        intelligence += GetStatIncreaseForClass(characterData?.characterClass ?? CharacterClass.Warrior, "intelligence");
        vitality += 2; // Everyone gets vitality
        
        RecalculateStats();
        
        // Restore health and mana on level up
        currentHealth = maxHealth;
        currentMana = maxMana;
        
        OnLevelUp?.Invoke(level);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        OnManaChanged?.Invoke(currentMana, maxMana);
        
        if (GameManager.Instance.isDebugMode)
        {
            Debug.Log($"Level up! Now level {level}");
        }
    }
    
    private int GetStatIncreaseForClass(CharacterClass characterClass, string statName)
    {
        switch (characterClass)
        {
            case CharacterClass.Warrior:
                return statName == "strength" ? 3 : (statName == "vitality" ? 2 : 1);
            case CharacterClass.Mage:
                return statName == "intelligence" ? 3 : (statName == "mana" ? 2 : 1);
            case CharacterClass.Archer:
                return statName == "agility" ? 3 : (statName == "strength" ? 2 : 1);
            case CharacterClass.Rogue:
                return statName == "agility" ? 2 : (statName == "strength" ? 2 : 1);
            default:
                return 1;
        }
    }
    
    // Combat Methods
    public int CalculateDamage()
    {
        int baseDamage = attackPower;
        
        // Check for critical hit
        if (Random.Range(0, 100) < criticalChance)
        {
            baseDamage = Mathf.RoundToInt(baseDamage * (criticalDamage / 100f));
            
            if (GameManager.Instance.isDebugMode)
            {
                Debug.Log("Critical hit!");
            }
        }
        
        return baseDamage;
    }
    
    public void TakeDamage(int damage)
    {
        // Apply defense
        int actualDamage = Mathf.Max(1, damage - defense);
        ModifyHealth(-actualDamage);
        
        if (GameManager.Instance.isDebugMode)
        {
            Debug.Log($"Took {actualDamage} damage (reduced from {damage} by {defense} defense)");
        }
    }
    
    private void HandleDeath()
    {
        if (GameManager.Instance.isDebugMode)
        {
            Debug.Log("Player died!");
        }
        
        // TODO: Implement death mechanics
        // For now, respawn with 25% health
        Invoke(nameof(Respawn), 3f);
    }
    
    private void Respawn()
    {
        currentHealth = maxHealth / 4;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        if (GameManager.Instance.isDebugMode)
        {
            Debug.Log("Player respawned");
        }
    }
    
    // Getters
    public float GetHealthPercentage() => (float)currentHealth / maxHealth;
    public float GetManaPercentage() => (float)currentMana / maxMana;
    public float GetExperiencePercentage() => (float)experience / experienceToNextLevel;
    
    public bool IsAlive() => currentHealth > 0;
    public bool HasMana(int amount) => currentMana >= amount;
    
    // Save current stats back to character data
    public void SaveToCharacterData()
    {
        if (characterData != null)
        {
            characterData.level = level;
            characterData.experience = experience;
            characterData.health = maxHealth;
            characterData.mana = maxMana;
            characterData.strength = strength;
            characterData.agility = agility;
            characterData.intelligence = intelligence;
        }
    }
}