using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField]
    private Button newGameButton;
    
    [SerializeField]
    private Button resumeButton;

    [SerializeField]
    private Button settingsButton;
    
    [SerializeField]
    private Button creditsButton;

    [SerializeField] 
    private Button exitButton;

    private void Start()
    {
        newGameButton.onClick.AddListener(NewGame);
        resumeButton.onClick.AddListener(Resume);
        settingsButton.onClick.AddListener(OpenSettings);
        creditsButton.onClick.AddListener(Credits);
        exitButton.onClick.AddListener(ExitGame);
    }

    private void NewGame()
    {
        GameManager.Instance.NewGame();
    }
    
    private void Resume()
    {
        GameManager.Instance.LoadLevel();
    }

    private void OpenSettings()
    {
        throw new NotImplementedException();
    }

    private void Credits()
    {
        throw new NotImplementedException();
    }

    private void ExitGame()
    {
        GameManager.Instance.ExitGame();
    }
}
