using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using ExitGames.Client.Photon;

public class BReady : MonoBehaviour
{
    private Button _bReady;

    private string _readyText = "isReady";

    void Start()
    {
        _bReady = GetComponent<Button>();

        _bReady.onClick.AddListener(PlayerReady);
    }

    private void PlayerReady()
    {
        _bReady.gameObject.SetActive(false);
        Debug.Log("Jugador listo!!!");

        Debug.Log($"[BReady] El jugador LOCAL presionó Ready → ActorNumber: {PhotonNetwork.LocalPlayer.ActorNumber}, Nick: {PhotonNetwork.LocalPlayer.NickName}");


        // Marcar jugador listo
        Hashtable data = new Hashtable();
        data[_readyText] = true;
        PhotonNetwork.LocalPlayer.SetCustomProperties(data);
    }

    private void OnDestroy()
    {
        // Resetear el marcador
        Hashtable data = new Hashtable();
        data[_readyText] = false;
        PhotonNetwork.LocalPlayer.SetCustomProperties(data);
    }
}
