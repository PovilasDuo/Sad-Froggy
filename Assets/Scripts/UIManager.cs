using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	private GameManager gameManagerInstance;
    public PlayerSettings playerSettings;

    public TextMeshProUGUI keyText;
	public TextMeshProUGUI scoreText;
    public TextMeshProUGUI finalScoreText;
    public Button pauseButton;
	public int score;

    public GameObject pausePanel;
	public GameObject gameOverPanel;
	private bool textAnimations = false;

	void Start()
	{
		score = 0;
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

	/// <summary>
	/// Makes the key text appear or disappear with an animation
	/// </summary>
	/// <param name="enabled">Enables or dissables the text</param>
	public void KeyTextAppear(bool enabled)
	{
		keyText.gameObject.SetActive(enabled);
		PulseText(keyText, enabled);
	}

	/// <summary>
	/// Pauses the game
	/// </summary>
	public void PauseGame()
	{
		Time.timeScale = 0;
		pausePanel.SetActive(true);
		pauseButton.gameObject.SetActive(false);
	}

	/// <summary>
	/// Resumes the game
	/// </summary>
	public void ResumeGame()
	{
		Time.timeScale = 1;
		pausePanel.SetActive(false);
		pauseButton.gameObject.SetActive(true);
	}

	/// <summary>
	/// Restarts the level
	/// </summary>
	public void Restart()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		Time.timeScale = 1;
	}

	/// <summary>
	/// Switches the scene to MainMenu scene
	/// </summary>
	public void BackToMainMenu()
	{
		SceneManager.LoadScene("MenuScene");
	}

	/// <summary>
	/// Exits the game
	/// </summary>
	public void ExitGame()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
        UnityEngine.Application.Quit();
#endif
	}

	/// <summary>
	/// Makes a text appear in a pulse like animation
	/// </summary>
	/// <param name="text">Text to appear</param>
	/// <param name="enabled">Sets the bool variable</param>
	public void PulseText(TMP_Text text, bool enabled)
	{
		textAnimations = enabled;
		StartCoroutine(PulseAnimation(text));
	}

	public void ShowGameOver()
    {
		playerSettings.totalTadpoles += score;
		finalScoreText.text = "Collected tadpoles: " + scoreText.text;
        gameOverPanel.SetActive(true);
        pauseButton.gameObject.SetActive(false);
    }

	/// <summary>
	/// Makes a text appear in a pulse like animation
	/// </summary>
	/// <param name="text">Text to appear</param>
	/// <returns>Smooth animation after a period</returns>
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
