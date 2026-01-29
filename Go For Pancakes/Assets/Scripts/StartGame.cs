using UnityEngine;

public class StartGame : MonoBehaviour
{
    public void StartNewGame()
    {
        GameController.gameController.StartGame();
    }
}
