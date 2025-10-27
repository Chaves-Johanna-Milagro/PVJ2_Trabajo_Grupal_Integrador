using TMPro;
using UnityEngine;

public class ScoreLeft : MonoBehaviour
{
    private TMP_Text _scoreText;
    private int _score = 0;
    void Start()
    {
        _scoreText = GetComponent<TMP_Text>();
    }

    public void IncreasePoint()
    {
        _score++;

        _scoreText.text = _score.ToString();
    }
}
