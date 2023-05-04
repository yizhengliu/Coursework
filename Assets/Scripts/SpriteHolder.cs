using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteHolder : MonoBehaviour
{
    private static SpriteHolder instance = null;
    [SerializeField]
    private Sprite[] contents;
    [SerializeField]
    private Sprite[] cardSprites;
    [SerializeField]
    private Sprite[] inventoryItemSprites;
    public const int CHEST = 0;
    public const int EMPTY = 1;
    public const int MONSTER = 2;
    public const int RANDOM = 3;
    public const int MAINCITY = 4;
    public const int DEBUFF = 5;
    public const int BLOCKED = 6;
    public const int BUFF = 7;
    public const int EVENT = 8;

    [SerializeField]
    private Sprite[] itemChipFrame;
    public static SpriteHolder Instance
    {
        get
        {  
            // test if the instance is null
            // if so, try to get it using FindObjectOfType
            if (instance == null)
                instance = FindObjectOfType<SpriteHolder>();

            // if the instance is null again
            // create a new game object
            // attached this class on it
            // set the instance to the new attached Singleton
            // call don't destroy on load
            if (instance == null)
            {
                GameObject gObj = new GameObject();
                gObj.name = "CardContentProvider";
                instance = gObj.AddComponent<SpriteHolder>();
                DontDestroyOnLoad(gObj);
            }
            return instance;
        }
    }
    private void Awake()
    {
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

    public Sprite getCardSprite(int index)
    {
        return cardSprites[index];
    }

    public Sprite getItemSprite(int index)
    {
        return inventoryItemSprites[index];
    }

    public Sprite getCardContent(int cardType, int index)
    {
        switch (cardType)
        {
            case CHEST:return contents[CHEST_CONTENT];
            case BLOCKED: return contents[BLOCKED_CONTENT];
            case MAINCITY: return contents[MAINCITY_CONTENT];
            case EMPTY: return contents[EMPTY_CONTENT];
            case RANDOM: return null;
            case EVENT: return contents[PRINCESS];
            default: return contents[index];
        }
    }

    public Color GetTheme(int cardType)
    {
        switch (cardType)
        {
            case CHEST:
            case BUFF: return BONUS_THEME;
            case MONSTER: return MONSTER_THEME;
            case DEBUFF: return DEBUFF_THEME;
            case BLOCKED: return BLOCK_THEME;
            case MAINCITY:
            case EVENT: return MAINCITY_THEME;
            default: return DEFAULT_THEME;
        }
    }


    private static Color MAINCITY_THEME = Color.green;
    private static Color BLOCK_THEME = Color.grey;
    private static Color BONUS_THEME = Color.yellow;
    private static Color DEFAULT_THEME = Color.blue;
    private static Color MONSTER_THEME = Color.red;
    private static Color DEBUFF_THEME = Color.magenta;
    //Map Content Info
    public const int CHEST_CONTENT = 0;
    public const int EMPTY_CONTENT = 1;
    public const int MAINCITY_CONTENT = 2;
    public const int BLOCKED_CONTENT = 3;


    public const int MONSTER_SLIME_CONTENT = 4;
    public const int MONSTER_VIPER_CONTENT = 5;
    public const int MONSTER_BAT_CONTENT = 6;
    public const int MONSTER_WORF_CONTENT = 7;
    public const int MONSTER_PIRANHA_CONTENT = 8;
    public const int MONSTER_WORM_CONTENT = 9;
    public const int MONSTER_CYCLOPS_CONTENT = 10;
    public const int MONSTER_LITTLE_DRAKE_CONTENT = 11;
    public const int MONSTER_CANNIBAL_CONTENT = 12;
    public const int MONSTER_SKELOTON_CONTENT = 13;
    public const int MONSTER_BOSS_VEGE_CONTENT = 14;
    public const int MONSTER_BOSS_SHADOW_CONTENT = 15;
    public const int MONSTER_BOSS_ELITE_CONTENT = 16;
    public const int MONSTER_BOSS_ARCHMAGE_CONTENT = 17;
    public const int MONSTER_BOSS_MONSTER_CONTENT = 18;

    public const int BUFF_PREPARE_CONTENT = 19;
    public const int BUFF_REST_CONTENT = 20;

    public const int DEBUFF_CLIFF_CONTENT = 21;
    public const int DEBUFF_POISON_CONTENT = 22;
    public const int DEBUFF_ILLUSORY_CONTENT = 23;

    public const int PRINCESS = 24;


    public const int SUPER_STICK = 0;
    public const int VERY_SHORT_BLADE = 1;
    public const int HIGH_SPEED_WHIP = 2;
    public const int MAGIC_SWORD = 3;
    public const int TH108= 4;
    public const int LONG_SWORD = 5;
    public const int HAMMER = 6;
    public const int SUPER_SWORD = 7;
    public const int QUATRA_SWORD = 8;
    public const int HUMAN_SHIELD = 9;
    public const int SMALL_SHIELD = 10;
    public const int LAINHARUT = 11;

    //public const int SuperStick = 0;
    //public const int SuperStick = 0;
    //public const int SuperStick = 0;
    //public const int SuperStick = 0;


    public const int UNAVAILABLE = 0;
    public const int NO_ITEM_ROUND = 1;
    public const int OCCUPIED_ONE_TIME = 2;
    public const int OCCUPIED_MULTI_TIME_ALREADY = 3;
    public const int OCCUPIED_MULTI_TIME_ON_COOLDOWN = 4;
    public const int NO_ITEM_SQUARE = 5;

    public Sprite getItemChipFrame(int index) 
    {
        return itemChipFrame[index];
    }
}
