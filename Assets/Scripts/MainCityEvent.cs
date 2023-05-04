using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class MainCityEvent : MonoBehaviour
{
    [SerializeField]
    private GameObject[] abilities;
    [SerializeField]
    private GameObject[] conditions;
    [SerializeField]
    private TextMeshProUGUI description;

    [SerializeField]
    private Button[] Stages;
    private bool isAbilityGain = false;

    private void Start()
    {
        PlayerStatus.Instance.DescriptionDispay = description;
        loadDescription();
        setStages();
        checkPlayerStatus();
    }
    private void checkPlayerStatus() 
    {
        checkConditions(PlayerStatus.CONDITION_POISON);
        checkConditions(PlayerStatus.CONDITION_ILLUSORY);
        checkAbilities(PlayerStatus.ABILITY_HYPERMETABOLISM);
        checkAbilities(PlayerStatus.ABILITY_BERSERKER);
        checkAbilities(PlayerStatus.ABILITY_NIRVANA);
        checkAbilities(PlayerStatus.ABILITY_MISER);
    }
    private void setStages() 
    {
        for (int i = 0; i < Stages.Length; i++)
            Stages[i].interactable = (i <= GlobalStates.StageExplored);
    }
    private void checkAbilities(int abilityType)
    {
        abilities[abilityType].SetActive(
            PlayerStatus.Instance.Abilities.Contains(abilityType));
    }

    private void checkConditions(int conditionType)
    {
        conditions[conditionType].SetActive(
            PlayerStatus.Instance.Conditions.ContainsKey(conditionType));
        
    }
    public void loadDescription() 
    {
        PlayerStatus.Instance.loadDescription();
    }

    public void goToDungeon(int stageNum) 
    {
        GlobalStates.CurrentStage = stageNum;
        DungeonManager.newDungeon();
        SceneManager.LoadScene("Dungeon");
    }
    //level up
    [SerializeField]
    private GameObject UI;
    [SerializeField]
    private GameObject PopUp;
    [SerializeField]
    private TextMeshProUGUI popMessage;
    public void onClickGuild() 
    {
        if (PlayerStatus.Instance.Exp < PlayerStatus.Instance.getExpRequirement())
            popUp("Unfortunate!\n" +
                "Failed to level up\n" +
                (PlayerStatus.Instance.getExpRequirement() - PlayerStatus.Instance.Exp) +
                " more exp is needed");
        else 
        {
            switch (PlayerStatus.Instance.levelUp()) 
            {
                case PlayerStatus.NO_BONUS:
                    popUp("Level: " + (int.Parse(PlayerStatus.Instance.getStatus(PlayerStatus.LEVEL)) - 1) +
                        " => " + PlayerStatus.Instance.getStatus(PlayerStatus.LEVEL) + "\n" +
                        "MaxHp: " + (PlayerStatus.Instance.MaxHP - 40) +
                        " => " + PlayerStatus.Instance.MaxHP + "\n" +
                        "Luck: " + PlayerStatus.Instance.getStatus(PlayerStatus.LUCK) +
                        " => " + PlayerStatus.Instance.getStatus(PlayerStatus.LUCK) + "\n" +
                        "Chip Number: " + Inventory.Instance.ItemLimit +
                        " => " + Inventory.Instance.ItemLimit);
                    break;
                case PlayerStatus.LUCK_PLUS:
                    popUp("Level: " + (int.Parse(PlayerStatus.Instance.getStatus(PlayerStatus.LEVEL)) - 1) +
                        " => " + PlayerStatus.Instance.getStatus(PlayerStatus.LEVEL) + "\n" +
                        "MaxHp: " + (PlayerStatus.Instance.MaxHP - 40) +
                        " => " + PlayerStatus.Instance.MaxHP + "\n" +
                        "Luck: " + (int.Parse(PlayerStatus.Instance.getStatus(PlayerStatus.LUCK)) - 1) +
                        " => " + PlayerStatus.Instance.getStatus(PlayerStatus.LUCK) + "\n" +
                        "Chip Number: " + Inventory.Instance.ItemLimit +
                        " => " + Inventory.Instance.ItemLimit);
                    break;
                case PlayerStatus.INVENTORY_PLUS_PLUS:
                    popUp("Level: " + (int.Parse(PlayerStatus.Instance.getStatus(PlayerStatus.LEVEL)) - 1) +
                        " => " + PlayerStatus.Instance.getStatus(PlayerStatus.LEVEL) + "\n" +
                        "MaxHp: " + (PlayerStatus.Instance.MaxHP - 40) +
                        " => " + PlayerStatus.Instance.MaxHP + "\n" +
                        "Luck: " + PlayerStatus.Instance.getStatus(PlayerStatus.LUCK) +
                        " => " + PlayerStatus.Instance.getStatus(PlayerStatus.LUCK) + "\n" +
                        "Chip Number: " + (Inventory.Instance.ItemLimit - 2) +
                        " => " + Inventory.Instance.ItemLimit);
                    break;
                case PlayerStatus.INVENTORY_PLUS:
                    popUp("Level: " + (int.Parse(PlayerStatus.Instance.getStatus(PlayerStatus.LEVEL)) - 1) +
                        " => " + PlayerStatus.Instance.getStatus(PlayerStatus.LEVEL) + "\n" +
                        "MaxHp: " + (PlayerStatus.Instance.MaxHP - 40) +
                        " => " + PlayerStatus.Instance.MaxHP + "\n" +
                        "Luck: " + PlayerStatus.Instance.getStatus(PlayerStatus.LUCK) +
                        " => " + PlayerStatus.Instance.getStatus(PlayerStatus.LUCK) + "\n" +
                        "Chip Number: " + (Inventory.Instance.ItemLimit - 1) +
                        " => " + Inventory.Instance.ItemLimit);
                    break;
                case PlayerStatus.ABILITY_PLUS:
                    popUp("Level: " + (int.Parse(PlayerStatus.Instance.getStatus(PlayerStatus.LEVEL)) - 1) +
                        " => " + PlayerStatus.Instance.getStatus(PlayerStatus.LEVEL) + "\n" +
                        "MaxHp: " + (PlayerStatus.Instance.MaxHP - 40) +
                        " => " + PlayerStatus.Instance.MaxHP + "\n" +
                        "Luck: " + PlayerStatus.Instance.getStatus(PlayerStatus.LUCK) +
                        " => " + PlayerStatus.Instance.getStatus(PlayerStatus.LUCK) + "\n" +
                        "Chip Number: " + Inventory.Instance.ItemLimit +
                        " => " + Inventory.Instance.ItemLimit + "\n" +
                        "You can have an ability now!");
                    isAbilityGain = true;
                    break;
                case PlayerStatus.LEVEL_LIMIT:
                    popUp("You have reached level limit, no level ups anymore!");
                    break;
            }
        }

    }

    [SerializeField]
    private Button[] AbilityButtons;
    [SerializeField]
    private GameObject AbilityPopUp;
    public void getAbility()
    {//missing
      
        if (isAbilityGain)
        {
            UI.SetActive(true);
            AbilityPopUp.SetActive(true); 
            for (int i = 0; i < AbilityButtons.Length; i++)
                AbilityButtons[i].interactable = (!PlayerStatus.Instance.Abilities.Contains(i));
            isAbilityGain = false;
        }
    }
    public void informStates(int type) 
    {
        PlayerStatus.Instance.gainAbility(type);
        checkPlayerStatus();
    }
    public void popUp(string pm)
    {
        UI.SetActive(true);
        PopUp.SetActive(true);
        if (pm != null)
            popMessage.text = pm;
        else
            popMessage.text = PlayerStatus.Instance.Message;
    }

    public void onClickFreeInn() 
    {
        popUp("You had a good rest!\n Your HP is back to " + PlayerStatus.Instance.MaxHP +" 'Full' and conditions are all removed!");
        PlayerStatus.Instance.restInInn();
        checkPlayerStatus();
    }

    [SerializeField]
    private GameObject DetailPop;
    [SerializeField]
    private TextMeshProUGUI detailMessage;
    public void popDetailUp(string pm)
    {
        UI.SetActive(true);
        DetailPop.SetActive(true);
        detailMessage.text = pm;
    }

    [SerializeField]
    private Button buyToInventory;
    [SerializeField]
    private Button buyToCloset;

    [SerializeField]
    private Image itemToBuy;
    [SerializeField]
    private TextMeshProUGUI itemDetail;
    private IInventoryItem tempBonusItem;
    public void confirmItem(int index) 
    {
        TextAsset textAsset = Resources.Load("Battle/Weapons/" + index) as TextAsset;
        string[] itemAttributes = textAsset.text.Split('\n');
        tempBonusItem = new IInventoryItem(itemAttributes, index);

        itemToBuy.sprite = SpriteHolder.Instance.getItemSprite(
                    tempBonusItem.getProperty(IInventoryItem.INDEX));
        itemDetail.text = tempBonusItem.Name + "\n" +
            (tempBonusItem.getProperty(IInventoryItem.ATK) == 0 ? "" : (
            "ATK = " + tempBonusItem.getProperty(IInventoryItem.ATK) + "\n"))
             +
             (tempBonusItem.getProperty(IInventoryItem.DEF) == 0 ? "" : (
            "DEF = " + tempBonusItem.getProperty(IInventoryItem.DEF) + "\n"))
             +
             (tempBonusItem.getProperty(IInventoryItem.COOLDOWN) == 0 ? "Cooldown = immediate\n" : (
            "Cooldown = " + tempBonusItem.getProperty(IInventoryItem.COOLDOWN) + " turns\n"))
            +
            "PROPERTY = " +
            ((tempBonusItem.getProperty(IInventoryItem.PROPERTY) == IInventoryItem.PROPERTY_PHYSIC)
            ? "PHYSICAL" : "MAGICAL") + "\n" +
            "Your gold: " + PlayerStatus.Instance.getStatus(PlayerStatus.GOLD) + ", Weapon value: " +
            tempBonusItem.getProperty(IInventoryItem.VALUE) + "\n" +
            (Inventory.Instance.Items.Count == Inventory.Instance.ItemLimit ? "Your inventory is full\n" : "") +
            (Inventory.Instance.Closet.Count == Inventory.CLOSET_LIMIT ? "Your closet is full":"");
        if (int.Parse(PlayerStatus.Instance.getStatus(PlayerStatus.GOLD)) >=
            tempBonusItem.getProperty(IInventoryItem.VALUE))
        {
            buyToInventory.interactable = !(Inventory.Instance.Items.Count == Inventory.Instance.ItemLimit);
            buyToCloset.interactable = !(Inventory.Instance.Closet.Count == Inventory.CLOSET_LIMIT);
        }
        else 
        {
            buyToInventory.interactable = false;
            buyToCloset.interactable = false;
        }
    }

    public void buyItem() 
    {
        Inventory.Instance.addItem(tempBonusItem.getProperty(IInventoryItem.INDEX));
    }
    public void buyItemToCloset() 
    {
        Inventory.Instance.addItemCloset(tempBonusItem.getProperty(IInventoryItem.INDEX));
    }
}
