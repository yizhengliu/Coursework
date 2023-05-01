public class Monster
{
    private int maxHP;
    private int HP;
    private int[] pattern;
    private int debuff;
    private int immue;
    private int exp;
    private int gold;

    private int next = -1;
    public int Next { get { return next; } }

    private int counter = 0;
    public int MaxHP { get { return maxHP; } }
    public int Gold { get { return gold; } }
    public int Exp { get { return exp; } }
    public Monster(string[] attributes) {
        string[] HPInfo = attributes[0].Split(',');
        HP = int.Parse(HPInfo[0]);
        maxHP = HP;
        if (HPInfo.Length != 1) next = int.Parse(HPInfo[1]);

        string[] patternInfo = attributes[1].Split(',');
        pattern = new int[patternInfo.Length];
        for (int i = 0; i < patternInfo.Length; i++)
            pattern[i] = int.Parse(patternInfo[i]);

        debuff = int.Parse(attributes[2]);
        immue = int.Parse(attributes[3]);
        exp = int.Parse(attributes[4]);
        gold = int.Parse(attributes[5]);
    }

    public void nextMonsterBehaviour()
    {
        if (++counter == pattern.Length)
            counter = 0;
    }

    public int currentBehaviour()
    {
        return pattern[counter];
    }

    public void getHitted(int dmg) 
    {
        HP -= dmg;
    }

    public int getHP() 
    {
        return HP;
    }

    public int getDebuff() 
    {
        return debuff;
    }

    public int getImmue() { return immue; }

    public const int DEBUFF_NOTHING = -1;
    public const int DEBUFF_DEF_DOWN = 0;
    public const int DEBUFF_ATK_DOWN = 1;
    public const int DEBUFF_RANDOM = 2;

    public const int IMMUE_NOTHING = -1;
    public const int IMMUE_PHYSICAL = 0;
    public const int IMMUE_MAGICAL = 1;
    public const int IMMUE_RANDOM = 2;
}
