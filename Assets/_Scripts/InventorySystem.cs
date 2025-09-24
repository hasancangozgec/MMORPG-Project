using UnityEngine;
using System.Collections.Generic;

public class InventorySystem : MonoBehaviour
{
    [Header("Inventory Settings")]
    public int maxSlots = 30;
    public int maxStackSize = 99;
    
    // Inventory data
    private List<InventorySlot> inventorySlots;
    private Dictionary<string, ItemData> itemDatabase;
    
    // Events
    public System.Action<List<InventorySlot>> OnInventoryUpdated;
    public System.Action<ItemData, int> OnItemAdded;
    public System.Action<ItemData, int> OnItemRemoved;
    public System.Action<ItemData> OnItemUsed;
    
    private void Awake()
    {
        InitializeInventory();
    }
    
    private void InitializeInventory()
    {
        inventorySlots = new List<InventorySlot>();
        itemDatabase = new Dictionary<string, ItemData>();
        
        // Initialize empty slots
        for (int i = 0; i < maxSlots; i++)
        {
            inventorySlots.Add(new InventorySlot());
        }
        
        // Load item database
        LoadItemDatabase();
        
        if (GameManager.Instance.isDebugMode)
        {
            Debug.Log($"Inventory initialized with {maxSlots} slots");
        }
    }
    
    private void LoadItemDatabase()
    {
        // Create sample items for development
        CreateSampleItems();
    }
    
    private void CreateSampleItems()
    {
        // Health Potion
        ItemData healthPotion = new ItemData
        {
            itemId = "health_potion",
            itemName = "Health Potion",
            description = "Restores 50 HP",
            itemType = ItemType.Consumable,
            rarity = ItemRarity.Common,
            maxStackSize = 10,
            value = 25,
            iconPath = "Icons/HealthPotion"
        };
        itemDatabase.Add(healthPotion.itemId, healthPotion);
        
        // Mana Potion
        ItemData manaPotion = new ItemData
        {
            itemId = "mana_potion",
            itemName = "Mana Potion",
            description = "Restores 30 MP",
            itemType = ItemType.Consumable,
            rarity = ItemRarity.Common,
            maxStackSize = 10,
            value = 20,
            iconPath = "Icons/ManaPotion"
        };
        itemDatabase.Add(manaPotion.itemId, manaPotion);
        
        // Iron Sword
        ItemData ironSword = new ItemData
        {
            itemId = "iron_sword",
            itemName = "Iron Sword",
            description = "A sturdy iron sword. +10 Attack",
            itemType = ItemType.Weapon,
            rarity = ItemRarity.Common,
            maxStackSize = 1,
            value = 100,
            iconPath = "Icons/IronSword",
            attackPower = 10
        };
        itemDatabase.Add(ironSword.itemId, ironSword);
        
        // Leather Armor
        ItemData leatherArmor = new ItemData
        {
            itemId = "leather_armor",
            itemName = "Leather Armor",
            description = "Basic leather armor. +5 Defense",
            itemType = ItemType.Armor,
            rarity = ItemRarity.Common,
            maxStackSize = 1,
            value = 75,
            iconPath = "Icons/LeatherArmor",
            defensePower = 5
        };
        itemDatabase.Add(leatherArmor.itemId, leatherArmor);
    }
    
    // Inventory Management
    public bool AddItem(string itemId, int quantity = 1)
    {
        if (!itemDatabase.TryGetValue(itemId, out ItemData itemData))
        {
            Debug.LogError($"Item not found in database: {itemId}");
            return false;
        }
        
        return AddItem(itemData, quantity);
    }
    
    public bool AddItem(ItemData itemData, int quantity = 1)
    {
        if (itemData == null || quantity <= 0) return false;
        
        int remainingQuantity = quantity;
        
        // Try to stack with existing items first
        if (itemData.maxStackSize > 1)
        {
            for (int i = 0; i < inventorySlots.Count && remainingQuantity > 0; i++)
            {
                InventorySlot slot = inventorySlots[i];
                if (slot.itemData != null && slot.itemData.itemId == itemData.itemId)
                {
                    int canAdd = Mathf.Min(remainingQuantity, itemData.maxStackSize - slot.quantity);
                    if (canAdd > 0)
                    {
                        slot.quantity += canAdd;
                        remainingQuantity -= canAdd;
                    }
                }
            }
        }
        
        // Add to empty slots
        while (remainingQuantity > 0)
        {
            int emptySlotIndex = FindEmptySlot();
            if (emptySlotIndex == -1)
            {
                Debug.LogWarning("Inventory is full!");
                break;
            }
            
            int addQuantity = Mathf.Min(remainingQuantity, itemData.maxStackSize);
            inventorySlots[emptySlotIndex].itemData = itemData;
            inventorySlots[emptySlotIndex].quantity = addQuantity;
            remainingQuantity -= addQuantity;
        }
        
        bool fullyAdded = remainingQuantity == 0;
        if (fullyAdded)
        {
            OnItemAdded?.Invoke(itemData, quantity);
            OnInventoryUpdated?.Invoke(inventorySlots);
            
            if (GameManager.Instance.isDebugMode)
            {
                Debug.Log($"Added {quantity}x {itemData.itemName} to inventory");
            }
        }
        
        return fullyAdded;
    }
    
    public bool RemoveItem(string itemId, int quantity = 1)
    {
        if (!itemDatabase.TryGetValue(itemId, out ItemData itemData))
        {
            return false;
        }
        
        return RemoveItem(itemData, quantity);
    }
    
    public bool RemoveItem(ItemData itemData, int quantity = 1)
    {
        if (itemData == null || quantity <= 0) return false;
        
        int remainingToRemove = quantity;
        
        for (int i = inventorySlots.Count - 1; i >= 0 && remainingToRemove > 0; i--)
        {
            InventorySlot slot = inventorySlots[i];
            if (slot.itemData != null && slot.itemData.itemId == itemData.itemId)
            {
                int removeQuantity = Mathf.Min(remainingToRemove, slot.quantity);
                slot.quantity -= removeQuantity;
                remainingToRemove -= removeQuantity;
                
                if (slot.quantity <= 0)
                {
                    slot.itemData = null;
                    slot.quantity = 0;
                }
            }
        }
        
        bool fullyRemoved = remainingToRemove == 0;
        if (fullyRemoved)
        {
            OnItemRemoved?.Invoke(itemData, quantity);
            OnInventoryUpdated?.Invoke(inventorySlots);
            
            if (GameManager.Instance.isDebugMode)
            {
                Debug.Log($"Removed {quantity}x {itemData.itemName} from inventory");
            }
        }
        
        return fullyRemoved;
    }
    
    public bool UseItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= inventorySlots.Count) return false;
        
        InventorySlot slot = inventorySlots[slotIndex];
        if (slot.itemData == null || slot.quantity <= 0) return false;
        
        ItemData itemData = slot.itemData;
        
        // Handle item usage based on type
        bool itemUsed = false;
        switch (itemData.itemType)
        {
            case ItemType.Consumable:
                itemUsed = UseConsumableItem(itemData);
                break;
            case ItemType.Weapon:
            case ItemType.Armor:
                itemUsed = EquipItem(itemData);
                break;
        }
        
        if (itemUsed)
        {
            // Remove one item from inventory for consumables
            if (itemData.itemType == ItemType.Consumable)
            {
                RemoveItem(itemData, 1);
            }
            
            OnItemUsed?.Invoke(itemData);
        }
        
        return itemUsed;
    }
    
    private bool UseConsumableItem(ItemData itemData)
    {
        // TODO: Implement consumable effects
        if (GameManager.Instance.isDebugMode)
        {
            Debug.Log($"Used consumable: {itemData.itemName}");
        }
        
        return true;
    }
    
    private bool EquipItem(ItemData itemData)
    {
        // TODO: Implement equipment system
        if (GameManager.Instance.isDebugMode)
        {
            Debug.Log($"Equipped: {itemData.itemName}");
        }
        
        return true;
    }
    
    // Utility Methods
    private int FindEmptySlot()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].itemData == null)
            {
                return i;
            }
        }
        return -1;
    }
    
    public int GetItemCount(string itemId)
    {
        int count = 0;
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.itemData != null && slot.itemData.itemId == itemId)
            {
                count += slot.quantity;
            }
        }
        return count;
    }
    
    public bool HasItem(string itemId, int quantity = 1)
    {
        return GetItemCount(itemId) >= quantity;
    }
    
    public List<InventorySlot> GetInventorySlots()
    {
        return inventorySlots;
    }
    
    public ItemData GetItemData(string itemId)
    {
        itemDatabase.TryGetValue(itemId, out ItemData itemData);
        return itemData;
    }
    
    // Development helper
    public void AddSampleItems()
    {
        if (!GameManager.Instance.isDebugMode) return;
        
        AddItem("health_potion", 5);
        AddItem("mana_potion", 3);
        AddItem("iron_sword", 1);
        AddItem("leather_armor", 1);
        
        Debug.Log("Added sample items to inventory");
    }
}

// Data Classes
[System.Serializable]
public class InventorySlot
{
    public ItemData itemData;
    public int quantity;
    
    public bool IsEmpty => itemData == null || quantity <= 0;
}

[System.Serializable]
public class ItemData
{
    public string itemId;
    public string itemName;
    public string description;
    public ItemType itemType;
    public ItemRarity rarity;
    public int maxStackSize = 1;
    public int value;
    public string iconPath;
    
    // Combat stats
    public int attackPower;
    public int defensePower;
    public int healthBonus;
    public int manaBonus;
    
    // Requirements
    public int levelRequirement = 1;
    public CharacterClass classRequirement = CharacterClass.Warrior;
}

public enum ItemType
{
    Consumable,
    Weapon,
    Armor,
    Accessory,
    Material,
    Quest
}

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}