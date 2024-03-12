using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class UIManager : MonoBehaviour
{
	private GameManager gameManagerInstance;

	public TextMeshProUGUI keyText;
	public TextMeshProUGUI scoreText;
	public Button pauseButton;
	public int score;

	public GameObject pausePanel;
	public bool textAnimations = false;

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
		PulseText(keyText);
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
		Time.timeScale = 1;
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

	public void PulseText(TMP_Text text)
	{
		textAnimations = true;
		StartCoroutine(PulseAnimation(text));
	}

	private IEnumerator PulseAnimation(TMP_Text text)
	{
		while (textAnimations)
		{
			float lerpTime = Mathf.PingPong(Time.time, 1);
			float newSize = Mathf.Lerp(0.9f, 1.2f, lerpTime);

			// Set the text size
			text.transform.localScale = new Vector3(newSize, newSize, 1);

			yield return null;
		}
	}
}
