using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreRight : MonoBehaviour
{
    private TMP_Text _scoreText;
    private int _score = 0;
    void Start()
    {
        _scoreText = GetComponent<TMP_Text>();
        _scoreText.text = "Jugador 2";
    }

    public void IncreasePoint()
    {
        _score++;

        _scoreText.text = "Jugador 2\n" + _score.ToString();

    }
    public void WinLevel()
    {
        if (_score >= 11)
        {
            SceneManager.LoadScene("Victory");

            Debug.Log("Jugador 2 ganó");
        }

    }
}
