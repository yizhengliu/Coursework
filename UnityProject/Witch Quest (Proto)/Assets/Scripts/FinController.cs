using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class FinController : MonoBehaviour
{
    [SerializeField]
    private Button load;
    private void Start()
    {
        if(load != null)
            load.interactable = File.Exists(Application.persistentDataPath +
            Path.AltDirectorySeparatorChar + "SaveData.json");
    }
    public void backToMainMenu() 
    {
        SceneManager.LoadScene("StartMenu");
    }

    public void quickLoad() 
    {
        JSONSaving.Instance.LoadGame();
        SceneManager.LoadScene("MainCity");
    }
}
