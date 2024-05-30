using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private InterfaceSection startSection, menuSection, gameSettingsSection, looseSection, winSection;
    [SerializeField] private KeyCode startButton = KeyCode.Space;
    [SerializeField] private TMP_Text resultsText;
    [Space, SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buttonClip;
    [SerializeField] private AudioClip spaceBarClip;

    private InterfaceSection currentSection;

    private void Start()
    {
        GameManager.GameManagerState GMState = GameManager.instance.CurrentState();

        if (GMState == GameManager.GameManagerState.Win)
        {
            int collectables = GameManager.instance.CollectablesCount();
            double time = GameManager.instance.CurrentTimer();
            double minutes = Math.Floor(time / 60);
            double seconds = Math.Floor(time % 60);
            resultsText.text = String.Format("Score: {0}\nTime: {1:00}:{2:00}", collectables, minutes, seconds);
            OpenSection(winSection);
        }
        else if (GMState == GameManager.GameManagerState.Loose)
        {
            OpenSection(looseSection);
        }
        else
        {
            OpenSection(menuSection);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(startButton) && currentSection == startSection)
        {
            audioSource.PlayOneShot(spaceBarClip);
            OpenSection(menuSection);
        }
    }

    private void OpenSection(InterfaceSection nextSection)
    {
        if (currentSection != null)
        {
            currentSection.Active(false);
        }

        currentSection = nextSection;
        currentSection.Active(true);
    }
    
    public void OnPlayButton()
    {
        audioSource.PlayOneShot(buttonClip);
        OpenSection(gameSettingsSection);
    }

    public void OnQuitButton()
    {
        audioSource.PlayOneShot(buttonClip);
        GameManager.instance.QuitApplication();
    }
    
    public void OnBackMenuButton()
    {
        audioSource.PlayOneShot(buttonClip);
        OpenSection(menuSection);
    }

    public void OnDayButton()
    {
        audioSource.PlayOneShot(buttonClip);
        GameManager.instance.StartPlay(GameManager.MapSettings.Day);
    }
    
    public void OnDuskButton()
    {
        audioSource.PlayOneShot(buttonClip);
        GameManager.instance.StartPlay(GameManager.MapSettings.Dusk);
    }
    
    public void OnNightButton()
    {
        audioSource.PlayOneShot(buttonClip);
        GameManager.instance.StartPlay(GameManager.MapSettings.Night);
    }
}
