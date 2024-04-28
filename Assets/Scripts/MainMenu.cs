using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject SettingsPanel;
	public GameObject InstructionsPanel;
	public GameObject ShopPanel;
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
		Time.timeScale = 1;
	}
    
    public void ShowSettingsPanel()
	{

        SettingsPanel.SetActive(true);
        MainPanel.SetActive(false);
	}

	public void ShowInstructionsPanel()
	{
        InstructionsPanel.SetActive(true);
        MainPanel.SetActive(false);
	}

	public void ShowShopPanel()
    {
        ShopPanel.SetActive(true);
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
		if (SettingsPanel.activeSelf)
		{
			SettingsPanel.SetActive(false);
		}
		if (InstructionsPanel.activeSelf)
		{
			InstructionsPanel.SetActive(false);
		}
		if (ShopPanel.activeSelf)
		{
			ShopPanel.SetActive(false);
		}
	}

	public void GameVolumeChange()
	{
		AudioListener.volume = gameVolumeSlider.value;
	}   
}