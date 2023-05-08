using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    private Monster currentMonster;
    private static MonsterManager instance;
    private int tempImmue;
    public int TempImmue { get { return tempImmue; } }
    private int currentMonsterIndex;
    public int MonsterIndex { get { return currentMonsterIndex; } }
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
    public static MonsterManager Instance
    {
        get
        {
            // test if the instance is null
            // if so, try to get it using FindObjectOfType
            if (instance == null)
                instance = FindObjectOfType<MonsterManager>();

            // if the instance is null again
            // create a new game object
            // attached this class on it
            // set the instance to the new attached Singleton
            // call don't destroy on load

            if (instance == null)
            {
                GameObject gObj = new GameObject();
                gObj.name = "MonsterManager";
                instance = gObj.AddComponent<MonsterManager>();
                DontDestroyOnLoad(gObj);
            }
            return instance;
        }
    }
    public void setCurrentMonster(string index) {
        TextAsset textAsset = Resources.Load("MapInfo/Monsters/" + index) as TextAsset;
        string[] monsterAttributes = textAsset.text.Split('\n');
        currentMonster = new Monster(monsterAttributes);
        currentMonsterIndex = int.Parse(index);
    }

    public void nextMonsterBehaviour() 
    {
        currentMonster.nextMonsterBehaviour();
    }

    public int getcurrentBehaviour()
    {
        return currentMonster.currentBehaviour();
    }

    public void refreshImmueInfo() {
        if (currentMonster.getImmue() == Monster.IMMUE_RANDOM)
            tempImmue = Random.Range(0, 2);
        else
            tempImmue = currentMonster.getImmue();
    }

    public void getHitByPlayer(int playerDmg) {
        currentMonster.getHitted(playerDmg);
    }

    public bool isDead() { return currentMonster.Next == -1; }

    public int getCurrentHP() { return currentMonster.getHP(); }

    public int loadNextLife()
    {   
        TextAsset textAsset = Resources.Load("MapInfo/Monsters/" + currentMonsterIndex +
            "-" + currentMonster.Next) as TextAsset;
        string[] monsterAttributes = textAsset.text.Split('\n');
        currentMonster = new Monster(monsterAttributes);
        return currentMonster.Next - 1;
    }

    public int getMaxHP() { return currentMonster.MaxHP; }

    public int getEXP() { return currentMonster.Exp; }
    public int getGold() { return currentMonster.Gold; }

    public int getDebuff() 
    {
        if (Random.Range(0, 100) < 33)
        {
            if (currentMonster.getDebuff() != Monster.DEBUFF_RANDOM)
                return currentMonster.getDebuff();
            else
                return Random.Range(0, 2);
        }
        return Monster.DEBUFF_NOTHING;
    }
}
