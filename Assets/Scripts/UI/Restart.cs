using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour {
    void RestartApplication()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
