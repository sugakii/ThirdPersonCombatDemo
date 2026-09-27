using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // 主菜单只负责场景入口，不持有 Gameplay 运行时状态。
    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    // Application.Quit 在 Editor 中不会关闭 Unity，只在独立 Build 中生效。
    public void QuitGame()
    {
        Application.Quit();
    }
}
