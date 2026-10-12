using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum GameState { Playing, Paused, Won, Lost }

public class GameManager : MonoBehaviour
{
    [SerializeField] private float timeLimit = 90f;
    [SerializeField] private PlayerBall player;

    [Header("Звук")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip collectClip;
    [SerializeField] private AudioClip winClip;
    [SerializeField] private AudioClip loseClip;

    public event Action<int, int> CrystalsChanged; // собрано, всего
    public event Action<float> TimeChanged;        // осталось, с
    public event Action<GameState, string> StateChanged;

    public GameState State { get; private set; } = GameState.Playing;

    private int total;
    private int collected;
    private float timeLeft;
    private InputAction pauseAction;

    void Awake()
    {
        pauseAction = InputSystem.actions.FindAction("Player/Pause");
    }

    void OnEnable() => Crystal.Collected += OnCrystalCollected;
    void OnDisable() => Crystal.Collected -= OnCrystalCollected;

    void Start()
    {
        Time.timeScale = 1f;
        total = FindObjectsByType<Crystal>(FindObjectsSortMode.None).Length;
        timeLeft = timeLimit;
        CrystalsChanged?.Invoke(collected, total);
        TimeChanged?.Invoke(timeLeft);
        StateChanged?.Invoke(State, string.Empty);
    }

    void Update()
    {
        if (pauseAction.WasPressedThisFrame())
            TogglePause();

        if (State != GameState.Playing) return;

        timeLeft -= Time.deltaTime;
        TimeChanged?.Invoke(Mathf.Max(timeLeft, 0f));
        if (timeLeft <= 0f)
            Lose("Время вышло!");
    }

    private void OnCrystalCollected(Crystal crystal)
    {
        if (State != GameState.Playing) return;
        collected++;
        CrystalsChanged?.Invoke(collected, total);
        PlaySfx(collectClip);

        if (collected >= total)
            Win();
    }

    public void PlayerDied(string reason) => Lose(reason);

    private void Win()
    {
        SetState(GameState.Won, "Все кристаллы собраны!");
        PlaySfx(winClip);
    }

    private void Lose(string reason)
    {
        if (State == GameState.Won || State == GameState.Lost) return;
        SetState(GameState.Lost, reason);
        PlaySfx(loseClip);
    }

    public void TogglePause()
    {
        if (State == GameState.Playing) SetState(GameState.Paused, "");
        else if (State == GameState.Paused) SetState(GameState.Playing, "");
    }

    private void SetState(GameState newState, string message)
    {
        State = newState;
        player.ControlEnabled = newState == GameState.Playing;
        Time.timeScale = newState == GameState.Paused ? 0f : 1f;
        StateChanged?.Invoke(newState, message);
    }

    private void PlaySfx(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.pitch = UnityEngine.Random.Range(0.95f, 1.1f);
        sfxSource.PlayOneShot(clip);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}