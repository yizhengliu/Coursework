using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System;
using TMPro;
using System.Linq;
using UnityEngine.UI;
public class DungeonManager : MonoBehaviour
{
    private bool isAnimationFinished = false;
    private static int counter = 2;
    private static int currentPos = 2;
    private string[] mapInfos;
    public event EventHandler<NextAreaInfoArgs> mapReaded;
    // public event EventHandler<string[]> mapReaded;
    public event EventHandler<NextAreaInfoArgs> newAreaMoved;

    public static event EventHandler<int> sceneOver;
    public event EventHandler<int> newAreaButtonPressed;
    public event EventHandler<string[]> randomRouteEntered;
    [SerializeField]
    private TextMeshProUGUI maxHPDisplay;
    [SerializeField]
    private TextMeshProUGUI currentHPDisplay;
    [SerializeField]
    private TextMeshProUGUI goldDisplay;
    [SerializeField]
    private TextMeshProUGUI levelDisplay;
    [SerializeField]
    private TextMeshProUGUI expDisplay;
    [SerializeField]
    private TextMeshProUGUI luckDisplay;
    [SerializeField]
    private GameObject backToCityButton;
    [SerializeField]
    private Animator HPIndicator;
    [SerializeField]
    private GameObject[] ConditionIndicators;
    [SerializeField]
    private TextMeshProUGUI[] ConditionTimers;
    [SerializeField]
    private Animator playerIllusory;
    [SerializeField]
    private TextMeshProUGUI description;

    private const int NOTHING = 0;
    private const int GOLD = 1;
    private const int ITEM = 2;
    private IInventoryItem tempBonusItem;
    private int bonusType = NOTHING;
    private void Awake()
    {
        Inventory.Instance.ItemRemoved += checkAvaiabilityChestPopUpButtons;

    }
    private void Start()
    {
        PlayerStatus.Instance.DescriptionDispay = description;
        PlayerStatus.Instance.loadDescription();
        readMapInfo();
        loadPlayerStats();
        checkPlayerHP();
        backToCityButton.SetActive(!GlobalStates.IsTurotial);
        UI.SetActive(GlobalStates.IsFirstTimeDungeon);
        TentInfo.SetActive(GlobalStates.IsFirstTimeDungeon);
    }

    public void onClickBackToCity() 
    {
        Inventory.Instance.refreshCooldown();
        GlobalStates.tutorialFinished();
        Inventory.Instance.ItemRemoved -= checkAvaiabilityChestPopUpButtons;
        sceneOver?.Invoke(this, 0);
        JSONSaving.Instance.SaveGame();
        SceneManager.LoadScene("MainCity");
    }

    private void loadPlayerStats()
    {
        maxHPDisplay.text = PlayerStatus.Instance.getStatus(PlayerStatus.MAX_HP);
        currentHPDisplay.text = PlayerStatus.Instance.getStatus(PlayerStatus.CURRENT_HP);
        goldDisplay.text = PlayerStatus.Instance.getStatus(PlayerStatus.GOLD);
        levelDisplay.text = PlayerStatus.Instance.getStatus(PlayerStatus.LEVEL);
        expDisplay.text = PlayerStatus.Instance.getStatus(PlayerStatus.EXP);
        luckDisplay.text = PlayerStatus.Instance.getStatus(PlayerStatus.LUCK);
        //check Player Conditions
        checkConditions(PlayerStatus.CONDITION_POISON);
        checkConditions(PlayerStatus.CONDITION_ILLUSORY);
        checkAbilities(PlayerStatus.ABILITY_HYPERMETABOLISM);
        checkAbilities(PlayerStatus.ABILITY_BERSERKER);
        checkAbilities(PlayerStatus.ABILITY_MISER);
        checkAbilities(PlayerStatus.ABILITY_NIRVANA);
        PlayerStatus.Instance.loadDescription();
    }

    [SerializeField]
    private GameObject[] conditionsInTent;
    [SerializeField]
    private GameObject[] abilitiesInTent;
    private void checkAbilities(int abilityType) 
    {
        abilitiesInTent[abilityType].SetActive(
            PlayerStatus.Instance.Abilities.Contains(abilityType));
    }

    private void checkConditions(int conditionType) 
    {
        if (PlayerStatus.Instance.Conditions.ContainsKey(conditionType))
        {
            ConditionIndicators[conditionType].SetActive(true);
            conditionsInTent[conditionType].SetActive(true);
            ConditionTimers[conditionType].text = "" + PlayerStatus.Instance.Conditions[conditionType];
            if (conditionType == PlayerStatus.CONDITION_ILLUSORY)
                playerIllusory.SetBool("IsIllusory", true);
        }
        else
        {
            conditionsInTent[conditionType].SetActive(false);
            ConditionIndicators[conditionType].SetActive(false);
            if (conditionType == PlayerStatus.CONDITION_ILLUSORY)
                playerIllusory.SetBool("IsIllusory", false);
        }
    }
    private void readMapInfo() 
    {
        TextAsset textAsset = Resources.Load("MapInfo/map" + GlobalStates.CurrentStage) as TextAsset;
        mapInfos = textAsset.text.Split('\n');
        newAreaButtonPressed?.Invoke(this, currentPos);
        mapReaded?.Invoke(this, new NextAreaInfoArgs(counter - 2, mapInfos));
    }
    private void updateConditions() 
    {
        if (PlayerStatus.Instance.updateStatesInDungeon())
            popUp(null);
    }
    public void MoveToNextArea(int pos) {
        int leftLimit = currentPos - 1 < 0 ? 0 : currentPos - 1;
        int rightLimit = currentPos + 1 > 4 ? 4 : currentPos + 1;
        if (PlayerStatus.Instance.Conditions.ContainsKey(PlayerStatus.CONDITION_ILLUSORY))
            pos = UnityEngine.Random.Range(leftLimit, rightLimit + 1);
        currentPos = pos;

        if (++counter == mapInfos.Length)
            counter = 0;
        updateConditions();
        StartCoroutine(updateUserStates(counter - 3, pos));
        loadPlayerStats();
        checkPlayerHP();
        if (int.Parse(PlayerStatus.Instance.getStatus(PlayerStatus.CURRENT_HP)) <= 0)
            SceneManager.LoadScene("GameOver");
    }
    private void checkPlayerHP() {
        if (int.Parse(PlayerStatus.Instance.getStatus(PlayerStatus.CURRENT_HP)) <
            PlayerStatus.Instance.MaxHP * 0.25)
            HPIndicator.SetBool("lowHP", true);
        else
            HPIndicator.SetBool("lowHP", false);
    }
    IEnumerator updateUserStates(int currentArea, int currentRoute) 
    {
        if (currentArea < 0)
            currentArea += mapInfos.Length;
        string info = mapInfos[currentArea].Split(' ')[currentRoute];
        if (info.Contains('r'))
        {
            info = randomRoll(info, false);
            randomRouteEntered?.Invoke(this,
                new string[] { "" + currentRoute,
                info});
            yield return new WaitUntil(()=>isAnimationFinished);
            isAnimationFinished = false;
        }
        caseCheck(info);
        newAreaButtonPressed?.Invoke(this, currentRoute);
        newAreaMoved?.Invoke(this, new NextAreaInfoArgs(currentArea, mapInfos));
        yield return null;
    }

    public void animationFinished() {
        isAnimationFinished = true;
    }

    private void caseCheck(string info)
    {
        string tempNum = string.Join("", info.ToCharArray().Where(Char.IsDigit));
        int index = -1;
        if (tempNum != "")
            index = int.Parse(tempNum);
        if (info.Contains('m'))
        {
            MonsterManager.Instance.setCurrentMonster(tempNum);
            SceneManager.LoadScene("Battle");
        }
        else if (info.Contains('c'))
        {
            //chest!! 
            string bonus = randomRoll(info, true);
            if (bonus != "g")
            {
                index = int.Parse(bonus);

                TextAsset textAsset = Resources.Load("Battle/Weapons/" + index) as TextAsset;
                string[] itemAttributes = textAsset.text.Split('\n');
                tempBonusItem = new IInventoryItem(itemAttributes, index);
                bonusType = ITEM;
            }
            else 
                bonusType = GOLD;

            popUp("Found a treasure chest!\n" +
                "Of course I would took it.");
        }
        else if (info.Contains('d'))
        {
            switch (index)
            {
                case SpriteHolder.DEBUFF_CLIFF_CONTENT:
                    PlayerStatus.Instance.dropedInCliff();
                    currentHPDisplay.text = PlayerStatus.Instance.getStatus(PlayerStatus.CURRENT_HP);
                    break;
                case SpriteHolder.DEBUFF_ILLUSORY_CONTENT:
                    PlayerStatus.Instance.debuff(PlayerStatus.CONDITION_ILLUSORY);
                    popUp(null);
                    break;
                case SpriteHolder.DEBUFF_POISON_CONTENT:
                    PlayerStatus.Instance.debuff(PlayerStatus.CONDITION_POISON);
                    popUp(null);
                    break;
            }
            popUp(null);
        }
        else if (info.Contains('b'))
        {
            switch (index)
            {
                case SpriteHolder.BUFF_PREPARE_CONTENT:
                    Inventory.Instance.refreshCooldown();
                    popUp("Found a legendary whetstone!\n" +
                        "My items are all ready!");
                    break;
                case SpriteHolder.BUFF_REST_CONTENT:
                    PlayerStatus.Instance.rest();
                    currentHPDisplay.text = PlayerStatus.Instance.getStatus(PlayerStatus.CURRENT_HP);
                    popUp(null);
                    break;
            }
        }
        else if (info.Contains('M'))
        {
            Inventory.Instance.refreshCooldown();
            GlobalStates.tutorialFinished();
            Inventory.Instance.ItemRemoved -= checkAvaiabilityChestPopUpButtons;
            sceneOver?.Invoke(this, 0);
            JSONSaving.Instance.SaveGame();
            SceneManager.LoadScene("MainCity");
        }
        else if (info.Contains('f')) 
        {
            SceneManager.LoadScene("FIN");
        }
    }
    private string randomRoll(string info, bool isChest) {
        int from = info.IndexOf('(') + 1; 
        int to;
        if (isChest)
            to = info.LastIndexOf(')');
        else
            to = info.IndexOf(')');
        Debug.Log(to); 
        string[] possibleInfos;
        if (isChest)
            possibleInfos = info.Substring(from, to - from).Split('_');
        else
            possibleInfos = info.Substring(from, to - from).Split(',');
        return possibleInfos[UnityEngine.Random
            .Range(0, possibleInfos.Length)];
    }
    [SerializeField]
    private GameObject UI;
    [SerializeField]
    private GameObject PopUp;
    [SerializeField]
    private TextMeshProUGUI popMessage;
    [SerializeField]
    private GameObject TentInfo;
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

    public void popUp(string pm) {
        UI.SetActive(true);
        PopUp.SetActive(true);
        if (pm != null)
            popMessage.text = pm;
        else
            popMessage.text = PlayerStatus.Instance.Message;
    }

    public static void newDungeon() {
        counter = 2;
    }

    public void tentClosed() { GlobalStates.firstTimeCloseTent(); }

    public void getBounsItem() 
    {
        switch (bonusType) 
        {
            case NOTHING: return;
            case GOLD:
                int goldGain = UnityEngine.Random.Range(
                    GlobalStates.CurrentStage * 100 + 20,
                     GlobalStates.CurrentStage * 100 + 80);
                popUp("Found some gold!\n" +
                    "CurrentGold " + PlayerStatus.Instance.getStatus(PlayerStatus.GOLD) +
                    " => " + (int.Parse(PlayerStatus.Instance.getStatus(PlayerStatus.GOLD)) + goldGain));
                PlayerStatus.Instance.bonusChestGold(goldGain);
                loadPlayerStats();
                bonusType = NOTHING;
                return;
            case ITEM:
                chestPopUp();
                bonusType = NOTHING; 
                return;
        }
    }
    [SerializeField]
    private GameObject ChestPopUp;

    [SerializeField]
    private Button Take;
    [SerializeField]
    private Button Closet;
    [SerializeField]
    private Button Drop;
    [SerializeField]
    private TextMeshProUGUI bonusItemDetail;
    [SerializeField]
    private Image itemSprite;
    public void chestPopUp()
    {
        Drop.interactable = !GlobalStates.IsTurotial;
        itemSprite.sprite = SpriteHolder.Instance.getItemSprite(
                    tempBonusItem.getProperty(IInventoryItem.INDEX));
        bonusItemDetail.text = tempBonusItem.Name + "\n" +
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
            ? "PHYSICAL" : "MAGICAL");
        UI.SetActive(true);
        ChestPopUp.SetActive(true);
        checkAvaiabilityChestPopUpButtons(null, null);
    }

    public void addNewItem() 
    {
        Inventory.Instance.addItem(tempBonusItem.getProperty(IInventoryItem.INDEX));
    }

    public void addToCloset() 
    {
        Inventory.Instance.addItemCloset(tempBonusItem.getProperty(IInventoryItem.INDEX));
    }

    public void checkAvaiabilityChestPopUpButtons(object sender, InventoryEventArgs args) 
    {
        if (!GlobalStates.IsClosetChecked || Inventory.Instance.Closet.Count == Inventory.CLOSET_LIMIT)
            Closet.interactable = false;
        else
            Closet.interactable = true;
        if (Inventory.Instance.isInventoryAddable())
            Take.interactable = true;
        else
            Take.interactable = false;
    }
}


