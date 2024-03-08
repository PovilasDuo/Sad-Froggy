using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject SettingsPanel;
    public GameObject MainPanel;
 	public Slider gameVolumeSlider;

    void Start()
	{
		gameVolumeSlider.value = AudioListener.volume;
		gameVolumeSlider.onValueChanged.AddListener(delegate { GameVolumeChange(); });
	}

    public void PlayGame()
    {
		SceneManager.LoadScene("MainScene");
	}
    
        public void ShowSettingsPanel()
	{
     
            SettingsPanel.SetActive(true);
            MainPanel.SetActive(false);
	}

		public void ExitGame()
	{
        #if UNITY_EDITOR
		    UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
	}

		public void Back()
	{
		MainPanel.SetActive(true);
		if (SettingsPanel.active)
		{
			SettingsPanel.SetActive(false);
		}
	}

	public void GameVolumeChange()
	{
		AudioListener.volume = gameVolumeSlider.value;
	}   
}