using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using ExitGames.Client.Photon;

public class BReady : MonoBehaviour
{
    private GameObject _blackImg;
    private Button _bReady;

    private string _readyText = "isReady";

    void Start()
    {
        _blackImg = transform.parent.transform.Find("BlackImg").gameObject; 

        _bReady = GetComponent<Button>();

        _bReady.onClick.AddListener(PlayerReady);
    }

    private void PlayerReady()
    {
        _blackImg.SetActive(true);
        _bReady.gameObject.SetActive(false);
        Debug.Log("Jugador listo!!!");

        Debug.Log($"[BReady] El jugador LOCAL presionó Ready → ActorNumber: {PhotonNetwork.LocalPlayer.ActorNumber}, Nick: {PhotonNetwork.LocalPlayer.NickName}");


        // Marcar jugador listo
        Hashtable data = new Hashtable();
        data[_readyText] = true;
        PhotonNetwork.LocalPlayer.SetCustomProperties(data);
    }
}
