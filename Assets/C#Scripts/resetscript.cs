using UnityEngine;
using UnityEngine.SceneManagement;

public class SwipeReset : MonoBehaviour
{
    public void ResetScene()
    {
        // Reset static values
        SwipeStats.YesCount = 0;
        SwipeStats.NoCount = 0;

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

