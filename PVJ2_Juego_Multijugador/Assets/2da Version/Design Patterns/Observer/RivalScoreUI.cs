using UnityEngine;
using TMPro;

public class RivalScoreUI : MonoBehaviour, IScoreObserver // Clase observadora
{
    private TMP_Text _textScore;
    void Start()
    {
        _textScore = GetComponent<TMP_Text>();
    }

    public void OnScoreChanged(int newScore)
    {
        _textScore.text = "Rival puntaje " + newScore.ToString();
    }
}
