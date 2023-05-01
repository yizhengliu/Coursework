using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ChipColor : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI info;
    private Image chip;
    private Color ATK = new Color(255f / 255f, 198f / 255f, 175f / 255f);
    private Color DEF = new Color(130f / 255f, 146f / 255f, 255f / 255f);

    private Color PHY_IMMUE = new Color(136f / 255f, 249f / 255f, 255f / 255f);
    private Color MAG_IMMUE = new Color(213f / 255f, 255f / 255f, 158f / 255f);
    private void Awake()
    {
        chip = GetComponent<Image>();
    }
    public void changeATKDEFColor(int bhv)
    {
        chip.color = bhv > 0 ? ATK : DEF;
        info.text = ATKDEFLvl(bhv);
    }
    public void changeImmueColor(int imu)
    {
        chip.color = imu == Monster.IMMUE_PHYSICAL ? PHY_IMMUE : MAG_IMMUE;
        info.text = imu == Monster.IMMUE_PHYSICAL ? "PHY" : "MAG";
    }
    private string ATKDEFLvl(int value) {
        int absoluteValue = Mathf.Abs(value);
        if (absoluteValue < 40)
            return "F";
        else if (absoluteValue < 80)
            return "E";
        else if (absoluteValue < 120)
            return "D";
        else if (absoluteValue < 190)
            return "C";
        else if (absoluteValue < 250)
            return "B";
        else if (absoluteValue < 340)
            return "A";
        else 
            return "S";
    } 
}
