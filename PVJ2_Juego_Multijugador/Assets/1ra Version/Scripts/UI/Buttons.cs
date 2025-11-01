using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    public void Menu()
    {
        SceneManager.LoadScene("Menu");
        Debug.Log("saliendo al menu...");
    }
    public void Level1()
    {
        //SceneManager.LoadScene("Level1");

        PhotonNetwork.LoadLevel("Level1");
        Debug.Log("entrando nivel 1...");

    }
    public void Level2()
    {
        //SceneManager.LoadScene("Level2");

        PhotonNetwork.LoadLevel("Level2");
        Debug.Log("entrando nivel 2...");

    }
    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("saliendo...");
    }

}
