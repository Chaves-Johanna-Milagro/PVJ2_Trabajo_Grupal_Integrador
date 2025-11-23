using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerScoreUI : MonoBehaviour, IScoreObserver, IPlayerUI // Clase observadora
{
    private TMP_Text _textScore;

    // Guardamos referencia al subject local
    private ScoreSubject _currentSubject;  

    void Start()
    {
        _textScore = GetComponent<TMP_Text>();
    }


    // Se encarga de saber y obtener el componente ScoreSubject del jugador LOCAL
    private ScoreSubject FindLocalSubject()
    {
        // Buscar subjects en la escena
        ScoreSubject[] subjects = FindObjectsOfType<ScoreSubject>();

        foreach (var s in subjects)
        {
            PhotonView pv = s.GetComponent<PhotonView>();

            // Solo registrar al subject LOCAL
            if (pv != null && pv.IsMine)
            {
                return s;
            }
        }

        return null;
    }


    // Metodos implementados de la interfaz IPlayerUI
    public void ActiveUI()
    {
        // Buscar el subject local
        _currentSubject = FindLocalSubject();

        if (_currentSubject == null)
        {
            Debug.LogWarning("[PlayerScoreUI] No se encontró Subject local al activar UI...");
            return;
        }

        // Registrar el observer una sola vez
        _currentSubject.AddObserver(this);

        Debug.Log("[PlayerScoreUI] UI activada y observer registrado...");
    }

    public void DesactiveUI()
    {
        if (_currentSubject == null) return;

        _currentSubject.ResetScore();

        _currentSubject.RemoveObserver(this);

        _currentSubject = null;

        Debug.Log("[PlayerScoreUI] UI desactivada y observer removido...");
    }


    // Metodo implementado de la interfaz IScoreObserver
    public void OnScoreChanged(int newScore)
    {
        Debug.Log("[PlayerScoreUI] OnScoreChanged recibido → nuevo score: " + newScore);

        _textScore.text = "My " + newScore.ToString();
    }


}
