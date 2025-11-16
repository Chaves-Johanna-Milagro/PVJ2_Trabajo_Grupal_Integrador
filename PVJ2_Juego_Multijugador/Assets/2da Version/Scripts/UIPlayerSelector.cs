using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class UIPlayerSelector : MonoBehaviour
{
    private GameObject _uiPlayer1;
    private GameObject _uiPlayer2;

    private bool _isActiveUI = false;

    void Start()
    {
        _uiPlayer1 = transform.Find("UIPlayer1")?.gameObject;
        _uiPlayer2 = transform.Find("UIPlayer2")?.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isActiveUI) return;

        int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

        if (actorNumber == 1)
        {
            _uiPlayer1.SetActive(true);
            _uiPlayer2.SetActive(false);
        }
        else if (actorNumber == 2)
        {
            _uiPlayer1.SetActive(false);
            _uiPlayer2.SetActive(true);
        }

        _isActiveUI = true;
    }
}
