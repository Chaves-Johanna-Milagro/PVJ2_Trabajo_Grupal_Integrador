using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerScoreUI : MonoBehaviour, IScoreObserver // Clase observadora
{
    private TMP_Text _textScore;

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

        // Buscamos todos los subjects
        ScoreSubject[] subjects = FindObjectsOfType<ScoreSubject>();

        foreach (var s in subjects)
        {
            // Tomamos SOLO el Subject del player local
            PhotonView pv = s.GetComponent<PhotonView>();

            if (pv != null && pv.IsMine)
            {
                Debug.Log("[PlayerScoreUI] Registrado al Subject del player LOCAL");
                s.AddObserver(this);
                _isRegistered = true;
                break;
            }
        }
    }

    public void OnScoreChanged(int newScore)
    {
        Debug.Log("[PlayerScoreUI] OnScoreChanged recibido → nuevo score: " + newScore);

        _textScore.text = "Mi puntaje " + newScore.ToString();
    }
}
