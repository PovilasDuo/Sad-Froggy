using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	private GameManager gameManagerInstance;

	public TextMeshProUGUI keyText;
	public TextMeshProUGUI scoreText;
    public int score;

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
}
