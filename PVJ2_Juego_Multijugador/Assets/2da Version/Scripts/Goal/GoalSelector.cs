using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GoalSelector : MonoBehaviour
{
    private GameObject _goalP1;
    private GameObject _goalP2;

    private bool _isActive = false;

    void Start()
    {
        _goalP1 = transform.Find("GPlayer1")?.gameObject;
        _goalP2 = transform.Find("GPlayer2")?.gameObject;

        _goalP1.SetActive(false);
        _goalP2.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (_isActive) return;

        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

        // IMPAR → es Player 1
        if (actorNumber % 2 == 1)
        {
            _goalP1.SetActive(true);
            _goalP2.SetActive(false);
        }
        // PAR → es Player 2
        else
        {
            _goalP1.SetActive(false);
            _goalP2.SetActive(true);
        }

        _isActive = true;
    }
}
