using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HighScorePanel : MonoBehaviour
{
    void Start()
    {
 
    }

    public void LoadMenu()
    {
        if (GameObject.Find("AlloyPowerup") != null)
        {
            GameObject[] alloyPowerups = GameObject.FindGameObjectsWithTag("AlloyPowerup");
            foreach (GameObject alloyPowerup in alloyPowerups)
            {
                Destroy(alloyPowerup);
            }
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
