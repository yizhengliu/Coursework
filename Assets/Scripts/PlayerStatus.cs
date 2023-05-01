using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerStatus: MonoBehaviour
{
    public const int UNDEFINED = -1;
    public const int CURRENT_HP = 0;
    public const int MAX_HP = 1;
    public const int LEVEL = 2;
    public const int EXP = 3;
    public const int GOLD = 4;
    public const int LUCK = 5;

    public const int CONDITION_POISON = 0;
    public const int CONDITION_ILLUSORY = 1;

    public const int ABILITY_HYPERMETABOLISM = 0;
    public const int ABILITY_BERSERKER = 1;
    public const int ABILITY_NIRVANA = 2;
    public const int ABILITY_MISER = 3;

    public const int NO_BONUS = -1;
    public const int LUCK_PLUS = 0;
    public const int ABILITY_PLUS = 1;
    public const int INVENTORY_PLUS_PLUS = 2;
    public const int INVENTORY_PLUS = 3;

    private static PlayerStatus instance = null;
    private static int currentHP = 120;
    private static int maxHP = 120;
    private static int level = 1;
    private static int exp = 0;
    private static int gold = 0;
    private static int luck = 2; 
    private Dictionary<int, int> lvlBonuses = new Dictionary<int, int>();
    private int[] levelUpExpRequirement;
    private string[] descriptions;
    [SerializeField]
    private TextMeshProUGUI descriptionDisplay;

    private string message;
    public string Message { get { return message; } }

    public int MaxHP { get { return maxHP; } }
    private static List<int> abilities = new List<int>();
    private static Dictionary<int, int> conditions = new Dictionary<int, int>();
    public Dictionary<int, int> Conditions { get { return conditions; } }
    public List<int> Abilities { get { return abilities; } }

   
    public int Gold { get { return gold; } }

    private void Awake()
    {
        readExpRequirementInfo();
        readDescriptions();
        loadDescription();
        //Check if instance already exists
        if (instance == null)
            //if not, set instance to this
            instance = this;
        //If instance already exists and it's not this:
        else if (instance != this)
            // Then destroy this. This enforces the singleton pattern,
            // meaning there can only ever be one instance of a ScoreManager.
            Destroy(gameObject);
        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }
    public static PlayerStatus Instance
    {
        get
        {
            // test if the instance is null
            // if so, try to get it using FindObjectOfType
            if (instance == null)
                instance = FindObjectOfType<PlayerStatus>();

            // if the instance is null again
            // create a new game object
            // attached this class on it
            // set the instance to the new attached Singleton
            // call don't destroy on load
            
            if (instance == null)
            {
                GameObject gObj = new GameObject();
                gObj.name = "PlayerStatus";
                instance = gObj.AddComponent<PlayerStatus>();
                DontDestroyOnLoad(gObj);
            }
            return instance;
        }
    }
    public void levelUp() {
        switch (lvlBonuses[++level]) {
            case LUCK_PLUS: luck++; break;
            case ABILITY_PLUS:  break;
            case INVENTORY_PLUS_PLUS: Inventory.Instance.lvlUp(2); break;
            case INVENTORY_PLUS: Inventory.Instance.lvlUp(1); break;
        }
        maxHP += 40;
        currentHP = maxHP;
    }

    public void debuff(int condition) {
        if (condition == CONDITION_ILLUSORY)
            message = "Bewitched!\n" +
                "I may can't move properly\n";
        else
            message = "Poisoned!\n" +
                "I feel so painful\n";
        if (conditions.ContainsKey(condition))
            conditions[condition] = 7;
        else
            conditions.Add(condition, 7);
    }

    public bool updateStatesInDungeon()
    {
        if (Conditions.ContainsKey(CONDITION_ILLUSORY))
        {
            if (--Conditions[CONDITION_ILLUSORY] == 0)
                Conditions.Remove(CONDITION_ILLUSORY);
        }
        if (Conditions.ContainsKey(CONDITION_POISON))
        {
            int dmg = Mathf.RoundToInt(currentHP * Random.Range(0.03f, 0.05f));
            message = "poison!\n" +
                dmg + " damage!\n" +
                "current HP: " + currentHP + " => " + (currentHP - dmg);
            currentHP -= dmg;
            //Conditions[CONDITION_POISON]--;
            if (--Conditions[CONDITION_POISON] == 0)
                Conditions.Remove(CONDITION_POISON);
            return true;
        }
        return false;
    }

    public void flee() { gold -= Mathf.RoundToInt(0.05f * gold); }
    public void dropedInCliff() {
        int dmg = Mathf.RoundToInt(currentHP * Random.Range(0.1f, 0.3f));
        message = "fell off a cliff!\n" +
            dmg + " damage!\n" +
            "current HP: " + currentHP + " => " + (currentHP - dmg);  
        currentHP -= dmg;
    }

    public void rest() {
        string append;
        if (currentHP + Mathf.RoundToInt(0.25f * maxHP) > maxHP)
            append = "" + maxHP + " (maximum HP)";
        else
            append = "" + (currentHP + Mathf.RoundToInt(0.25f * maxHP));
        message = "Found a resting point!\n" +
            Mathf.RoundToInt(0.25f * maxHP) + "HP is revived \n" +
            "current HP: " + currentHP + " => " + append;
        currentHP += Mathf.RoundToInt(0.25f * maxHP);
        if (currentHP > maxHP)
            currentHP = maxHP;
    }

    public string getStatus(int type) {
        switch (type) 
        {
            case LUCK: return ("" + luck);
            case MAX_HP: return ("/ " + maxHP);
            case CURRENT_HP: return ("" + currentHP);
            case GOLD: return ("" + gold);
            case LEVEL: return ("" + level);
            case EXP: return ("" + exp);
            default: return ("" + UNDEFINED);
        }
    }

    private void readExpRequirementInfo()
    {
        TextAsset textAsset = Resources.Load("Character/LevelUpExpInfo") as TextAsset;
        string[] infos = textAsset.text.Split('\n');
        levelUpExpRequirement = new int[infos.Length];
        for (int i = 0; i < infos.Length; i++)
        {
            string[] info = infos[i].Split(' ');
            levelUpExpRequirement[i] = int.Parse(info[0]);
            lvlBonuses.Add(i + 2, int.Parse(info[1]));
        }
    }

    private void readDescriptions()
    {
        TextAsset textAsset = Resources.Load("Character/Description") as TextAsset;
        descriptions = textAsset.text.Split('\n');
    }

    public void loadDescription() 
    {
        descriptionDisplay.text = descriptions[GlobalStates.StageExplored];
    }

    public void monsterDefeated(int bounsGold, int bounsExp) 
    {
        gold += bounsGold;
        exp += bounsExp;
    }

    public int dmgedByMonster(int dmg) 
    {
        currentHP -= dmg;
        return currentHP;
    }

    public void bonusChestGold(int amount) 
    {
        gold += amount;
    }

}
