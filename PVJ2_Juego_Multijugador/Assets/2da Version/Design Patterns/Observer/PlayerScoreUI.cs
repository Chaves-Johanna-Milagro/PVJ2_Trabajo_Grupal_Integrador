using UnityEngine;
using TMPro;

public class PlayerScoreUI : MonoBehaviour, IScoreObserver // Clase observadora
{
    private TMP_Text _textScore;

    private ScoreSubject _subject;

    // Bandera para evitar bucles
    private bool _isRegistered = false;

    void Start()
    {
        _textScore = GetComponent<TMP_Text>();
    }

    void Update()
    {
        // Si ya está registrado → no hacer nada
        if (_isRegistered) return;

        // Si ya encontramos un Subject en escena → registrarlo
        if (_subject == null)
        {
            _subject = FindFirstObjectByType<ScoreSubject>();

            if (_subject != null)
            {
                Debug.Log("[PlayerScoreUI] ScoreSubject encontrado → registrando observer...");
                _subject.AddObserver(this);
                _isRegistered = true;
            }
        }
    }

    public void OnScoreChanged(int newScore)
    {
        Debug.Log("[PlayerScoreUI] OnScoreChanged recibido → nuevo score: " + newScore);

        _textScore.text = "Mi puntaje " + newScore.ToString();
    }
}
