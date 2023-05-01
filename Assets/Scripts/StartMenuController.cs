using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    public void OnClickNewGame() {
        Inventory.Instance.addItem(SpriteHolder.SUPER_STICK);
        SceneManager.LoadScene("Dungeon");
    }

    public void OnClickLoad()
    {
        GlobalStates.tutorialFinished();
        SceneManager.LoadScene("MainCity");
    }
}
