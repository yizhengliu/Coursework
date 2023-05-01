using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinController : MonoBehaviour
{
    public void backToMainMenu() 
    {
        SceneManager.LoadScene("StartMenu");
    }

    public void quickLoad() 
    {
        SceneManager.LoadScene("Dungeon");
    }
}
