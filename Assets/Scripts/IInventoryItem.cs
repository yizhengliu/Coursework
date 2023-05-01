public class IInventoryItem
{
    private string name;
    private int atk;
    private int def;
    private int cooldown;
    private int property;
    private int value;


    private int index;
    private int currentCooldown = 0;

    public int CurrentCooldown{ get { return currentCooldown; } }
    public string Name { get { return name; } }
    public IInventoryItem(string[] attributes, int index)
    {
        name = attributes[0];
        atk = int.Parse(attributes[1]);
        def = int.Parse(attributes[2]);
        cooldown = int.Parse(attributes[3]);
        property = int.Parse(attributes[4]);
        value = int.Parse(attributes[5]);
        this.index = index;
    }

    public int getProperty(int type) 
    {
        switch (type)
        {
            case INDEX: return index;
            case ATK: return atk;
            case DEF: return def;
            case COOLDOWN: return cooldown;
            case PROPERTY: return property;
            default: return -1;
        }
    }
    public void used() 
    {
        currentCooldown = cooldown;
    }

    public void onCooldownReduced() 
    {
        if (--currentCooldown < 0)
            currentCooldown = 0;
    }
    public void resetCooldown() { currentCooldown = 0; }
    public const int INDEX = 0;
    public const int ATK = 1;
    public const int DEF = 2;
    public const int COOLDOWN = 4;
    public const int PROPERTY = 5;

    public const int PROPERTY_MAGIC = 1;
    public const int PROPERTY_PHYSIC = 0;
}
