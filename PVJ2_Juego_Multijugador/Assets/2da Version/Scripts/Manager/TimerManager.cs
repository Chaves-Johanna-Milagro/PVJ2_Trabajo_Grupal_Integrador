using Photon.Pun;
using TMPro;
using UnityEngine;

public class TimerManager : MonoBehaviourPun, IPlayerUI
{
    private TMP_Text _timerText;

    private float _time = 120f;   // tiempo inicial
    private bool _running = false;

    private void Start()
    {
        if (_timerText == null)
        {
            _timerText = GameObject.Find("TTimer").GetComponent<TMP_Text>();
        }

        _timerText.text = "00:00";
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (!_running) return;

        _time -= Time.deltaTime;

        if (_time <= 0f)
        {
            _time = 0f;
            _running = false;
        }

        // MasterClient manda el tiempo a todos
        photonView.RPC("RPC_UpdateTimer", RpcTarget.All, _time);
    }
    public void ActiveUI()
    {
        _running = true;
    }

    [PunRPC]
    private void RPC_UpdateTimer(float t)
    {
        _time = t;

        int minutes = Mathf.FloorToInt(t / 60f);
        int seconds = Mathf.FloorToInt(t % 60f);
        _timerText.text = $"{minutes:00}:{seconds:00}";
    }
}
