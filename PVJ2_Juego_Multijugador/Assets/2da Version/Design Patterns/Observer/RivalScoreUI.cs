using Photon.Pun;
using TMPro;
using UnityEngine;

public class RivalScoreUI : MonoBehaviour, IScoreObserver // Clase observadora
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
        if (_isRegistered) return;

        ScoreSubject[] subjects = Object.FindObjectsOfType<ScoreSubject>();

        foreach (var s in subjects)
        {
            PhotonView pv = s.GetComponent<PhotonView>();

            // Registrarse SOLO al player que NO es el local
            if (pv != null && !pv.IsMine)
            {
                Debug.Log("[RivalScoreUI] Registrado al Subject del RIVAL");
                s.AddObserver(this);
                _isRegistered = true;
                break;
            }
        }
    }

    public void OnScoreChanged(int newScore)
    {
        _textScore.text = "Rival puntaje " + newScore;
    }
}
