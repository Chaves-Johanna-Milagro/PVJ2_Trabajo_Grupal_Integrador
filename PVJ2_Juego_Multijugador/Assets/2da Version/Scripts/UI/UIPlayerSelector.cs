using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class UIPlayerSelector : MonoBehaviour, IPlayerUI
{
    private GameObject _uiPlayer1;
    private GameObject _uiPlayer2;

    private bool _isActiveUI = false;

    void Start()
    {
        _uiPlayer1 = transform.Find("UIPlayer1")?.gameObject;
        _uiPlayer2 = transform.Find("UIPlayer2")?.gameObject;

        _uiPlayer1.SetActive(false);
        _uiPlayer2.SetActive(false);
    }
    public void ActiveUI()
    {
        if (_isActiveUI) return;

        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

        // IMPAR → es Player 1
        if (actorNumber % 2 == 1)
        {
            _uiPlayer1.SetActive(true);
            _uiPlayer2.SetActive(false);
        }
        // PAR → es Player 2
        else
        {
            _uiPlayer1.SetActive(false);
            _uiPlayer2.SetActive(true);
        }

        _isActiveUI = true;
    }
}
