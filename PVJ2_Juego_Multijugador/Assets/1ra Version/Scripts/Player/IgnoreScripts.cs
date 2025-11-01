using Photon.Pun;
using UnityEngine;

public class IgnoreScripts : MonoBehaviour
{
    private MonoBehaviour[] _ignoreScripts;

    private PhotonView _photonView;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _photonView = GetComponent<PhotonView>();

        _ignoreScripts = new MonoBehaviour[]
        {
            GetComponent<MoveVertical>(),
            GetComponent<IgnoreScripts>()
        };

        if (!_photonView.IsMine)
        {
            foreach (var script in _ignoreScripts)
            {
                script.enabled = false;
            }
        }
    }
}
