using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI text;
    public int score;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		score = 0;
	}

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncreasePoints(int amount)
    {
        score += amount;
		text.text = score.ToString();
    }
}
