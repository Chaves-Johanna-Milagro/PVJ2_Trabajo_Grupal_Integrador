using Photon.Pun;
using TMPro;
using UnityEngine;

public class RivalScoreUI : MonoBehaviour, IScoreObserver // Clase observadora
{
    private TMP_Text _textScore;

    // Bandera para evitar bucles
    private bool _isRegistered = false;

    // Guardamos referencia al subject del rival
    private ScoreSubject _currentSubject; 

    void Start()
    {
        _textScore = GetComponent<TMP_Text>();
    }
    void Update()
    {
        // Si estaba registrado pero el subject YA NO EXISTE → volver a registrarse
        if (_isRegistered && _currentSubject == null)
        {
            _isRegistered = false;
        }

        // Si ya está registrado → no hacer nada
        if (_isRegistered) return;

        // Buscar al nuevo subject del rival
        ScoreSubject[] subjects = FindObjectsOfType<ScoreSubject>();

        foreach (var s in subjects)
        {
            PhotonView pv = s.GetComponent<PhotonView>();

            if (pv != null && !pv.IsMine)
            {
                Debug.Log("[RivalScoreUI] Registrado al nuevo Subject del RIVAL");

                s.AddObserver(this);

                _currentSubject = s;    // Guardamos la referencia
                _isRegistered = true;   // Para no repetir

                break;
            }
        }
    }

    public void OnScoreChanged(int newScore)
    {
        _textScore.text = "Rival puntaje " + newScore;
    }
}
