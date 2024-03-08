using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
      
    public void PlayGame()
    {
		SceneManager.LoadScene("MainScene");
	}
    

}