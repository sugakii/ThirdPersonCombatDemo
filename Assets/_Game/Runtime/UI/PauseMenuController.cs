using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField]
    private PlayerInputReader inputReader;

    [SerializeField]
    private GameObject pausePanel;

    [SerializeField]
    private CameraController cameraController;

    [SerializeField]
    private PlayerCombat playerCombat;

    [SerializeField]
    private SkillController skillController;

    [SerializeField]
    private GameFlowController gameFlowController;

    private bool isPaused;

    // timeScale 冻结依赖缩放时间的玩法；同时禁用输入消费者，避免暂停时缓存攻击或转动镜头。
    public void PauseGame()
    {
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        cameraController.enabled = false;
        playerCombat.enabled = false;
        skillController.enabled = false;
        isPaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cameraController.enabled = true;
        playerCombat.enabled = true;
        skillController.enabled = true;
        isPaused = false;
    }

    public void RestartGame()
    {
        // 切换场景前先恢复时间，避免新场景继承暂停状态。
        Time.timeScale = 1f;
        SceneManager.LoadScene("SampleScene");
    }

    public void ReturnToMainMenu()
    {
        // 主菜单需要可见且未锁定的鼠标，因此在离开 Gameplay 前统一恢复。
        Time.timeScale = 1f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene("MainMenu");
    }

    private void ClearPauseStateForGameEnd()
    {
        // 胜负流程接管游戏后关闭暂停层，防止两个终局界面同时存在。
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        isPaused = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Awake()
    {
        pausePanel.SetActive(false);
        isPaused = false;
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // Victory/GameOver 后不再响应 Pause；若结果在暂停期间产生，先清除暂停状态。
        if(!gameFlowController.IsPlaying)
        {
            if(isPaused)
            {
                ClearPauseStateForGameEnd();
            }

            return;
        }

        if(!inputReader.PausePressed)
        {
            return;
        }

        if(isPaused)
        {
            ResumeGame();
            return;
        }

        PauseGame();
    }
}
