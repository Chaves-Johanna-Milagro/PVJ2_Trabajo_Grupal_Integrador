using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerScoreUI : MonoBehaviour, IScoreObserver // Clase observadora
{
    private TMP_Text _textScore;

    // Bandera para evitar bucles
    private bool _isRegistered = false;

    // Guardamos referencia al subject local
    private ScoreSubject _currentSubject;  

    void Start()
    {
        _textScore = GetComponent<TMP_Text>();
    }

    void Update()
    {
        // Si estaba registrado pero el subject YA NO EXISTE → volver a buscar
        if (_isRegistered && _currentSubject == null)
        {
            _isRegistered = false;
        }

        // Si ya está registrado → no hacer nada
        if (_isRegistered) return;

        // Buscar subjects en la escena
        ScoreSubject[] subjects = FindObjectsOfType<ScoreSubject>();

        foreach (var s in subjects)
        {
            PhotonView pv = s.GetComponent<PhotonView>();

            // Solo registrar al subject LOCAL
            if (pv != null && pv.IsMine)
            {
                Debug.Log("[PlayerScoreUI] Registrado al Subject LOCAL");

                s.AddObserver(this);

                _currentSubject = s;     // guardamos referencia
                _isRegistered = true;    // no repetir

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
