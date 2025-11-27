using UnityEngine;
using Photon.Pun;

public class BallData : MonoBehaviourPun
{
    private string _ballType = "normal";

    // Establece el tipo de pelota. Solo el MasterClient puede hacer esto
    public void SetBallType(string type)
    {
        // Permitir que cualquiera lo establezca localmente si no hay photonView
        if (photonView == null)
        {
            _ballType = type;
            Debug.Log($"[BallData] Tipo establecido localmente: {type}");
            return;
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("[BallData] Solo el MasterClient puede establecer el tipo de pelota");
            return;
        }

        photonView.RPC("RPC_SetBallType", RpcTarget.AllBuffered, type);
    }

    [PunRPC]
    private void RPC_SetBallType(string type)
    {
        _ballType = type;
        Debug.Log($"[BallData] RPC - Tipo de pelota establecido: {type} (GameObject: {gameObject.name})");
    }

    // Obtiene el tipo de pelota
    public string GetBallType()
    {
        return _ballType;
    }

    // Para debugging en el inspector
    void OnGUI()
    {
        if (Debug.isDebugBuild && gameObject.activeInHierarchy)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            if (screenPos.z > 0)
            {
                GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y, 100, 20), $"Type: {_ballType}");
            }
        }
    }
}