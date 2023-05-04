using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;

public class StartMenuController : MonoBehaviour
{
    [SerializeField]
    private Button load;
    private void Start()
    {
        load.interactable = File.Exists(Application.persistentDataPath +
           Path.AltDirectorySeparatorChar + "SaveData.json");
    }
    public void OnClickNewGame()
    {
        JSONSaving.Instance.newGame();
        Inventory.Instance.addItem(SpriteHolder.SUPER_STICK);
        SceneManager.LoadScene("Dungeon");
    }

    public void OnClickLoad()
    {
        JSONSaving.Instance.newGame();
        JSONSaving.Instance.LoadGame();
        GlobalStates.tutorialFinished();
        SceneManager.LoadScene("MainCity");
    }
}
