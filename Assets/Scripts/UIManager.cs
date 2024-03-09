using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
	private GameManager gameManagerInstance;

	public TextMeshProUGUI keyText;
	public TextMeshProUGUI scoreText;
	public Button pauseButton;
    public int score;

	public GameObject pausePanel;

    void Start()
    {
		score = 0;
	}


    void Update()
    {
        
    }

	/// <summary>
	/// Increases the game score by the desired amount
	/// </summary>
	/// <param name="amount">The desired amount</param>
	public void IncreasePoints(int amount)
    {
        score += amount;
		scoreText.text = score.ToString();
		GameObject.Find("GameManager").GetComponent<GameManager>().tadPoleCount--;

	}

	public void KeyTextAppear()
	{
		keyText.gameObject.SetActive(true);
	}

	///TO DO
	public void DecreasePoints(int amount) { }


		public void PauseGame()
	{
		Time.timeScale = 0;
			pausePanel.SetActive(true);
			pauseButton.gameObject.SetActive(false);
	}

		public void ResumeGame()
	{
		Time.timeScale = 1;
			pausePanel.SetActive(false);
			pauseButton.gameObject.SetActive(true);
	}


		public void Restart()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void BackToMainMenu()
	{
		SceneManager.LoadScene("MenuScene");
	}

	public void ExitGame()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
	}
}
