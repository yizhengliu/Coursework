using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class BattleSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject ATK_DEFChip;
    [SerializeField]
    private GameObject ImmueChip;
    [SerializeField]
    private Image Enemy;
    [SerializeField]
    private TextMeshProUGUI maxHPDisplay;
    [SerializeField]
    private TextMeshProUGUI currentHPDisplay;
    [SerializeField]
    private GameObject Flee;
    [SerializeField]
    private TextMeshProUGUI popUpMessage;

    [SerializeField]
    private Animator playerLowHP;
    [SerializeField]
    private Animator enemyLowHP;
    [SerializeField]
    private Animator DebuffIndicator;
    [SerializeField]
    private GameObject UI;

    [SerializeField]
    private GameObject Tutorial;
    [SerializeField]
    private GameObject[] DebuffIndicators;
    [SerializeField]
    private TextMeshProUGUI[] DebuffRounds;
    private bool isFlee = false;
    private Dictionary<int, int> battleConditions = new Dictionary<int, int>();
    private int nextRoundDebuff = -1;
    [SerializeField]
    private TextMeshProUGUI RoundInfo;
    private static bool[] selectedChips;
    public static bool[] SelectedChips { get { return selectedChips; } }
    void Start()
    {
        selectedChips = new bool[Inventory.Instance.Items.Count];
        Enemy.sprite = SpriteHolder.Instance.getCardContent(SpriteHolder.MONSTER,
           MonsterManager.Instance.MonsterIndex);
        Enemy.SetNativeSize();
        updateChips();
        loadPlayerStats();
        Tutorial.SetActive(GlobalStates.NumOfBattle == 0);
        Flee.SetActive(!GlobalStates.IsTurotial);
        checkPlayerHP();
    }
    private void loadPlayerStats()
    {
        maxHPDisplay.text = PlayerStatus.Instance.getStatus(PlayerStatus.MAX_HP);
        currentHPDisplay.text = PlayerStatus.Instance.getStatus(PlayerStatus.CURRENT_HP);
        checkConditions(Monster.DEBUFF_ATK_DOWN);
        checkConditions(Monster.DEBUFF_DEF_DOWN);
    }
    private void checkConditions(int conditionType)
    {
        if (battleConditions.ContainsKey(conditionType))
        {
            DebuffIndicators[conditionType].SetActive(true);
            DebuffRounds[conditionType].text = "" + battleConditions[conditionType];
        }
        else
            DebuffIndicators[conditionType].SetActive(false);
    }

    private void updateChips() {
        //get ATK / DEF info
        int currentBhv = MonsterManager.Instance.getcurrentBehaviour();
        if (currentBhv == 0)
            ATK_DEFChip.SetActive(false);
        else
        {
            ATK_DEFChip.SetActive(true);
            ATK_DEFChip.SendMessage("changeATKDEFColor", currentBhv);
        }
        //get Immue info
        MonsterManager.Instance.refreshImmueInfo();
        if (MonsterManager.Instance.TempImmue == Monster.IMMUE_NOTHING)
            ImmueChip.SetActive(false);
        else
        {
            ImmueChip.SetActive(true);
            ImmueChip.SendMessage("changeImmueColor", MonsterManager.Instance.TempImmue);
        }
    }

    public void onClickBattleStart() {

        //attack enemy
        float critChangce = 
            UnityEngine.Random.Range(0,100) < 
            int.Parse(PlayerStatus.Instance.getStatus(PlayerStatus.LUCK)) ? 
            1.75f : 1f;

        //Check debuff
        float atk_ratio = 1f;
        float def_ratio = 1f;
        if (battleConditions.ContainsKey(Monster.DEBUFF_ATK_DOWN)) 
        {
            atk_ratio = 0.9f;
            if (--battleConditions[Monster.DEBUFF_ATK_DOWN] == 0)
                battleConditions.Remove(Monster.DEBUFF_ATK_DOWN);
        }
        if (battleConditions.ContainsKey(Monster.DEBUFF_DEF_DOWN))
        {
            def_ratio = 1.1f;
            if (--battleConditions[Monster.DEBUFF_DEF_DOWN] == 0)
                battleConditions.Remove(Monster.DEBUFF_DEF_DOWN);
        }

        //if type is right
        if (MonsterManager.Instance.TempImmue == Monster.IMMUE_NOTHING
            || ((!(MonsterManager.Instance.TempImmue == Monster.IMMUE_NOTHING))
            && !(MonsterManager.Instance.TempImmue ==
            Inventory.Instance.isSelectedItemsPHY(selectedChips))))
        {
            int dmg = Mathf.RoundToInt(
                Inventory.Instance.getSelectedItemsATK(selectedChips) * critChangce * atk_ratio);
            //if enemy defence
            if (MonsterManager.Instance.getcurrentBehaviour() < 0)
                dmg += MonsterManager.Instance.getcurrentBehaviour();
            if (dmg < 0)
                dmg = 0;
            MonsterManager.Instance.getHitByPlayer(dmg);
            RoundInfo.text = "You have dealt " + dmg + " damage.\n";
        }
        else
            RoundInfo.text = "Enemy blocked your attack due to the immue type.\n";

        //player get hit by enemy
        if (MonsterManager.Instance.getcurrentBehaviour() > 0)
        {
            int dmgFromEnemy = Mathf.RoundToInt(
                MonsterManager.Instance.getcurrentBehaviour() * def_ratio);
            if (PlayerStatus.Instance.dmgedByMonster(dmgFromEnemy) <= 0)
            {
                battleEnd?.Invoke(this, -1);
                SceneManager.LoadScene("GameOver");
            }
            if (dmgFromEnemy != 0)
                RoundInfo.text += "Enemy have dealt " + dmgFromEnemy + " to you.\n";
        }

        //if monster had buff, add it to the player
        addBuff(Monster.DEBUFF_ATK_DOWN);
        addBuff(Monster.DEBUFF_DEF_DOWN);
        if (nextRoundDebuff != Monster.DEBUFF_NOTHING)
            RoundInfo.text += "You have been debuffed on " + (
                nextRoundDebuff == Monster.DEBUFF_ATK_DOWN ? "attack." : "defence.");
        //update monster states and load on to the screen
        MonsterManager.Instance.nextMonsterBehaviour();
        updateChips();
        loadPlayerStats();

        //check if monster is dead
        if (MonsterManager.Instance.getCurrentHP() <= 0)
        {
            if (MonsterManager.Instance.isDead())
            {
                battleEnd?.Invoke(this, -1);
                StartCoroutine(enemyDead());
            }
            else
                MonsterManager.Instance.loadNextLife();
        }

        checkPlayerHP();
        checkDebuff();
        checkMonsterHP();
        onClickUnSelectAll();
        battleStarted?.Invoke(this, null);
    }
    IEnumerator enemyDead() 
    {
        enemyLowHP.SetBool("IsDead", true);
        yield return new WaitUntil(isDead); 
        if (MonsterManager.Instance.MonsterIndex > 13)
            GlobalStates.stageCleared();
        GlobalStates.battleFinished();
        popUp();
    }
    private bool isDead() { return Enemy.color.a == 0; }
    private void addBuff(int type) 
    {
        if (nextRoundDebuff == type)
        {
            if (battleConditions.ContainsKey(type))
                battleConditions[type] = 5;
            else
                battleConditions.Add(type, 5);
        }
    }
    public void onClickFlee() 
    {
        isFlee = true;
        popUpMessage.text = "Confirm\n" +
            "Do you want to throw away " + Mathf.RoundToInt(0.05f * PlayerStatus.Instance.Gold)
            + " to the enemy" + "and use this time to flee back to the city?";
    }
    private void checkPlayerHP()
    {
        if (int.Parse(PlayerStatus.Instance.getStatus(PlayerStatus.CURRENT_HP)) <
            PlayerStatus.Instance.MaxHP * 0.25)
            playerLowHP.SetBool("LowHP", true);
        else
            playerLowHP.SetBool("LowHP", false);
    }
    private void checkMonsterHP()
    {
        if (MonsterManager.Instance.getCurrentHP() <
            MonsterManager.Instance.getMaxHP() * 0.2)
            enemyLowHP.SetBool("lowHP", true);
        else
            enemyLowHP.SetBool("lowHP", false);
    }

    private void checkDebuff()
    {
        nextRoundDebuff = MonsterManager.Instance.getDebuff();
        if (nextRoundDebuff >= 0)
            DebuffIndicator.SetBool("IsDebuff", true);
        else
            DebuffIndicator.SetBool("IsDebuff", false);
    }

    private void popUp() {
        int currentExp = int.Parse(PlayerStatus.Instance.getStatus(PlayerStatus.EXP));
        int currentGold = int.Parse(PlayerStatus.Instance.getStatus(PlayerStatus.GOLD));
        popUpMessage.text = "Victory!\n" +
            (MonsterManager.Instance.getEXP()) + " EXP gained, current EXP = " +
            currentExp + " => " + (currentExp + MonsterManager.Instance.getEXP()) + "\n" +
            (MonsterManager.Instance.getGold()) + " Gold gained, current gold = " +
            currentGold + " => " + (currentGold + MonsterManager.Instance.getGold());
        UI.SetActive(true);
    }

    public void changeScene() {
        if (isFlee)
        {
            battleEnd?.Invoke(this, -1);
            PlayerStatus.Instance.flee();
            JSONSaving.Instance.SaveGame();
            SceneManager.LoadScene("MainCity");
        }
        else
        {
            PlayerStatus.Instance.monsterDefeated(MonsterManager.Instance.getGold(),
                MonsterManager.Instance.getEXP());
            SceneManager.LoadScene("Dungeon");
        }
    }

    public void onClickSelectAll()
    {
        for (int i = 0; i < selectedChips.Length; i++)
            if (Inventory.Instance.Items[i].CurrentCooldown == 0)
                selectedChips[i] = true;
    }

    public void onClickUnSelectAll()
    {
        for (int i = 0; i < selectedChips.Length; i++)
            selectedChips[i] = false;
    }

    public void onSelectItem(int index) 
    {
        int indexInArray = index +
            (Inventory.ITEMS_PER_PAGE * Inventory.Instance.CurrentPage);
        if (selectedChips[indexInArray])
            selectedChips[indexInArray] = false;
        else
            selectedChips[indexInArray] = true;
    }
    public static event EventHandler<int> battleEnd;
    public static event EventHandler<InventoryEventArgs> battleStarted;
}
