using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private Button _bPlay;
    private Button _bExit;
    private void Start()
    {
        _bPlay = transform.Find("BPlay").GetComponent<Button>();
        _bExit = transform.Find("BExit").GetComponent<Button>();

        _bPlay.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("SelectLevel");

            Debug.Log("[MainMenu] Cargando escena de seleccion de nivel...");
        });
        _bExit.onClick.AddListener(() =>
        {
            Application.Quit();

            Debug.Log("[MainMenu] Saliendo del juego...");
        });
    }

}
