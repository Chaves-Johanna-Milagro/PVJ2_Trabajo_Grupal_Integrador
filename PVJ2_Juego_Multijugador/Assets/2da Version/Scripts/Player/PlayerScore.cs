using UnityEngine;
using Photon.Pun;
public class PlayerScore : MonoBehaviourPun // Componente del jugador
{
    private ScoreSubject _scoreSubject;

    private int _score = 1;

    void Start()
    {
        // Buscar el ScoreSubject en el mismo objeto del jugador
        _scoreSubject = GetComponent<ScoreSubject>();

        if (_scoreSubject == null)
        {
            Debug.LogWarning("[PlayerScore] No se encontró ScoreSubject en el jugador...");
        }
    }

    void Update()
    {
        // Solo el jugador dueño puede sumar puntos
        if (!photonView.IsMine) return;

        // Prueba: sumar 1 punto al apretar K
        if (Input.GetKeyDown(KeyCode.K))
        {
             if (_scoreSubject != null)
             {
                    Debug.Log("[PlayerScore] Tecla K presionada → sumando 1 punto...");
                    _scoreSubject.AddScore(_score);
             }
             else
             {
                    Debug.LogWarning("[PlayerScore] No hay ScoreSubject para sumar puntuación...");
             }
        }
    }

    
}
