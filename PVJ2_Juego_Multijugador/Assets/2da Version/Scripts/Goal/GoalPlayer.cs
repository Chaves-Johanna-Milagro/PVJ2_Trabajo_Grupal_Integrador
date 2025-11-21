using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GoalPlayer : MonoBehaviourPun
{
    private ScoreSubject _myScore;

    private int _score = 1;

    // Busca el ScoreSubject del jugador LOCAL
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _myScore = FindLocalSubject();

        if (collision.tag == "Ball")
        {
            _myScore.AddScore(_score);
            Debug.Log("[GoalPlayer] punto anotado...");
        }
    }
}
