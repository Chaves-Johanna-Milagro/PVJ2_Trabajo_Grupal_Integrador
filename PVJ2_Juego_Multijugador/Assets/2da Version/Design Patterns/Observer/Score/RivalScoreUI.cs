using Photon.Pun;
using TMPro;
using UnityEngine;

public class RivalScoreUI : MonoBehaviour, IScoreObserver, IPlayerUI // Clase observadora
{
    private TMP_Text _textScore;

    // Guardamos referencia al subject del rival
    private ScoreSubject _currentSubject; 

    void Start()
    {
        _textScore = GetComponent<TMP_Text>();
    }


    // Se encarga de saber y obtener el componente ScoreSubject del jugador Rival
    private ScoreSubject FindRivalSubject()
    {
        // Buscar subjects en la escena
        ScoreSubject[] subjects = FindObjectsOfType<ScoreSubject>();

        foreach (var s in subjects)
        {
            PhotonView pv = s.GetComponent<PhotonView>();

            // Solo registrar al subject RIVAL
            if (pv != null && !pv.IsMine)
            {
                return s;
            }
        }

        return null;
    }


    // Metodos implementados de la interfaz IPlayerUI
    public void ActiveUI()
    {
        // Buscar el subject rival
        _currentSubject = FindRivalSubject();

        if (_currentSubject == null)
        {
            Debug.LogWarning("[RivalScoreUI] No se encontró Subject rival al activar UI...");
            return;
        }

        // Registrar el observer una sola vez
        _currentSubject.AddObserver(this);

        Debug.Log("[RivalScoreUI] UI activada y observer registrado...");
    }

    public void DesactiveUI()
    {
        if (_currentSubject == null) return;

        _currentSubject.RemoveObserver(this);
        _currentSubject = null;

        Debug.Log("[RivalScoreUI] UI desactivada y observer removido...");
    }


    // Metodo implementado de la interfaz IScoreObserver
    public void OnScoreChanged(int newScore)
    {
        _textScore.text = "Rival puntaje " + newScore;
    }
}
