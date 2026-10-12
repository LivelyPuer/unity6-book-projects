using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private GameStart game;
    [SerializeField] private PlayerInputReader playerInput;
    private InputAction pause;

    void Start()
    {
        pause = InputSystem.actions.FindAction("Player/Pause");
        panel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (pause.WasPressedThisFrame()) Toggle();
    }

    public void Toggle()
    {
        bool paused = !panel.activeSelf;
        panel.SetActive(paused);
        Time.timeScale = paused ? 0f : 1f;
        playerInput.enabled = !paused;   // клик по кнопке меню — не удар
        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = paused;
    }

    public void NewGame() => game.NewGame();

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
