
using System.Collections.Generic;
using System.Linq;
public class DataToSave {
    public int playerMaxHP;
    public int playerLevel;
    public int playerExp;
    public int playerGold;
    public int playerLuck;
    public List<int> playerAbilities;
    public List<int> items;
    public List<int> closet;
    public int itemLimit;

    public bool isClosetChecked;
    public int stageExplored;
    public DataToSave() {
        playerMaxHP = PlayerStatus.Instance.MaxHP;
        playerLevel = int.Parse(PlayerStatus.Instance.getStatus(PlayerStatus.LEVEL));
        playerExp = PlayerStatus.Instance.Exp;
        playerGold = int.Parse(PlayerStatus.Instance.getStatus(PlayerStatus.GOLD));
        playerLuck = int.Parse(PlayerStatus.Instance.getStatus(PlayerStatus.LUCK));
        playerAbilities = PlayerStatus.Instance.Abilities;
       
        items = Inventory.Instance.Items.Select(x => x.getProperty(IInventoryItem.INDEX)).ToList();
        closet = Inventory.Instance.Closet.Select(x => x.getProperty(IInventoryItem.INDEX)).ToList();
        itemLimit = Inventory.Instance.ItemLimit;

        isClosetChecked = GlobalStates.IsClosetChecked;
        stageExplored = GlobalStates.StageExplored;
    }
}
