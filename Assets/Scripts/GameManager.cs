using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameManagerState { MainMenu, Playing, Win, Loose }
    
    public enum MapSettings { Day, Dusk, Night }
    
    public static GameManager instance;

    [SerializeField] private GameManagerState state;
    [Header("Game settings")]
    [SerializeField] private double timerDuration = 300.0f;
    public MapSettings mapSettings;
    [Header("Player current stats")]
    [SerializeField] private int collectables = 0;
    public float timerForEndTimer = 10.0f;

    // In-game variables.
    private double timer;
    private int collectablesCount = 1;
    private bool endTimerTrigger = false;
    private bool endTimerAnimTrigger = false;

    private void Awake()
    {
        // Singleton.
        
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("Created GameManager.");
        }
        else
        {
            // Cannot create multiple instance of GameManager.
            Destroy(gameObject);
        }
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void Update()
    {
        if (state == GameManagerState.Playing)
        {
            timer -= Time.deltaTime;
            
            if (FlightHUD.instance != null)
            {
                FlightHUD.instance.UpdateTimer(Math.Max(0.0, timer));
            }

            if (timer <= 0.0f)
            {
                PlayerLoose(1.0f);
            }

            if (endTimerTrigger != true && timer <= timerForEndTimer)
            {
                endTimerTrigger = true;
                FlightAudioManager.instance.EndCountDown();
            }

            if (endTimerAnimTrigger != true && timer <= timerForEndTimer+1.0f)
            {
                endTimerAnimTrigger = true;
                FlightHUD.instance.StartFinalCountdown();
            }
        }
    }

    public GameManagerState CurrentState()
    {
        return state;
    }

    public int CollectablesCount()
    {
        return collectables;
    }

    public double CurrentTimer()
    {
        return timerDuration - timer;
    }
    
    // Scene management events.
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
        {
            OnMainMenuSceneLoaded();
        }
        else if (scene.buildIndex == 1)
        {
            OnPlaySceneLoaded();
        }
    }

    private void OnPlaySceneLoaded()
    {
        state = GameManagerState.Playing;
        
        timer = timerDuration;
        collectables = 0;

        collectablesCount = FindObjectsOfType<Collectable>().Length;
        FlightHUD.instance.UpdateCollectables(collectables, collectablesCount);
    }

    private void OnMainMenuSceneLoaded()
    {
    }

    // UI events.
    
    public void StartPlay(MapSettings settings)
    {
        mapSettings = settings;
        SceneManager.LoadScene(1);
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
    
    // Player events.
    
    public void PlayerWin()
    {
        state = GameManagerState.Win;
        StartCoroutine(PlayerWinAnimation(1.5f));
    }

    public void PlayerLoose(float delay = 0.0f)
    {
        state = GameManagerState.Loose;
        StartCoroutine(PlayerLooseAnimation(delay));
    }
    
    private IEnumerator PlayerLooseAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        FlightHUD.instance.FadeOut();
        yield return new WaitForSeconds(2.0f);
        QuitFlightScene();
    }
    
    private IEnumerator PlayerWinAnimation(float delay)
    {
        FlightHUD.instance.StartVictoryMessage();
        yield return new WaitForSeconds(delay);
        FlightHUD.instance.FadeOut();
        yield return new WaitForSeconds(2.0f);
        QuitFlightScene();
    }

    public void QuitFlightScene()
    {
        if (state == GameManagerState.Playing)
            state = GameManagerState.MainMenu;
        
        SceneManager.LoadScene(0);
    }
    
    public void PlayerCollect()
    {
        if (state == GameManagerState.Playing)
        {
            collectables++;
            FlightAudioManager.instance.PlaneCollect();

            if (collectables >= collectablesCount)
            {
                PlayerWin();
            }
            else
            {
                FlightHUD.instance.UpdateCollectables(collectables, collectablesCount);
            }
        }
    }

    public void PlayerCrash()
    {
        if (state == GameManagerState.Playing)
        {
            FlightAudioManager.instance.PlaneExplosion();
            PlayerLoose(2.0f);
        }
    }

}
