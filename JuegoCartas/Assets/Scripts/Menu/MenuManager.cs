using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuManager : MonoBehaviour
{
    // Script Utilizado para cambiar escenas y cerrar juego
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void CloseGame()
    {
        Application.Quit();
    }
    public void PlayGame()
    {
        SceneManager.LoadScene(2);
    }
    public void Edit()
    {
        SceneManager.LoadScene(3);
    }
}
