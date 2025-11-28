using Photon.Pun;
using TMPro;
using UnityEngine;

public class TimerManager : MonoBehaviourPun, IPlayerUI
{
    private TMP_Text _timerText;

    private float _time = 180f;   // tiempo inicial de 3 minutos
    private bool _running = false;

    private void Start()
    {
        if (_timerText == null)
        {
            _timerText = GameObject.Find("TTimer").GetComponent<TMP_Text>();
        }

        //_timerText.text = "00:00";
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
        // Cuando un jugador activa UI (incluye jugadores que entran tarde):
        // si no eres master, pedís el tiempo actual al master.
        if (!PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("RPC_RequestCurrentTime", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
        }
        else
        {
            // Master: simplemente manda el tiempo actual
            photonView.RPC("RPC_UpdateTimer", RpcTarget.All, _time);
        }

        _running = true;
    }

    public void DesactiveUI()
    {
        _time = 120f;

        _running = false;

        _timerText.text = "00:00";
    }

    [PunRPC]
    private void RPC_UpdateTimer(float t)
    {
        _time = t;

        int minutes = Mathf.FloorToInt(t / 60f);
        int seconds = Mathf.FloorToInt(t % 60f);
        _timerText.text = $"{minutes:00}:{seconds:00}";
    }

    // Cuando el jugador salga y vuelva a entrar tenga su timer actualizado
    [PunRPC]
    private void RPC_RequestCurrentTime(int actorID)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        // Master responde solo al jugador que pidió
        Photon.Realtime.Player target = PhotonNetwork.CurrentRoom.GetPlayer(actorID);

        if (target != null)
        {
            photonView.RPC("RPC_UpdateTimer", target, _time);
        }
    }
}
