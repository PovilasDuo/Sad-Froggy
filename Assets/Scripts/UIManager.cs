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
    public TextMeshProUGUI finalTimeText;
    public TextMeshProUGUI finishScoreText;
    public TextMeshProUGUI finishTimeText;
    public Button pauseButton;
	public int score;
	public float startTime;
	public float playTime;

    public GameObject pausePanel;
	public GameObject gameOverPanel;
	public GameObject finishPanel;
	private bool textAnimations = false;

    void Start()
    {
        score = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
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
        playTime += Time.time - startTime;

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

    public void OpenShopPanel()
    {
        PlayerPrefs.SetInt("OpenShopPanel", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene("MenuScene");
    }

    /// <summary>
    /// Switches the scene to MainMenu scene
    /// </summary>
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MenuScene");
	}

	/// <summary>
	/// Finishes the game when the red door entered
	/// </summary>
	public void FinishGame()
	{
        Time.timeScale = 0;
        playerSettings.totalTadpoles += score;

		SetPlayTimeText(finishTimeText);
        finishScoreText.text = "Saved tadpoles: " + score;

        finishPanel.SetActive(true);
        pauseButton.gameObject.SetActive(false);
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
		playerSettings.totalTadpoles += score / 2; 
		SetPlayTimeText(finalTimeText);
        finalScoreText.text = "Saved tadpoles: " + score / 2;

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

	/// <summary>
	/// Shows how long was playing when game finishes
	/// </summary>
    private void SetPlayTimeText(TextMeshProUGUI textObject)
    {
        playTime += Time.time - startTime;
        int minutes = Mathf.FloorToInt(playTime / 60);
        int seconds = Mathf.FloorToInt(playTime % 60);

        string playingTimeString = string.Format("{0:00}:{1:00}", minutes, seconds);
        textObject.text = "Playing time: " + playingTimeString;
    }
}
