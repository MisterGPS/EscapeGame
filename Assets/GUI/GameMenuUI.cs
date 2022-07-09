using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenuUI : MonoBehaviour
{
    [SerializeField]
    private Button resumeButton;

    [SerializeField]
    private Button settingsButton;

    [SerializeField] 
    private Button mainMenuButton;

    public bool bOpen { get; private set; } = false;

    private void Start()
    {
        resumeButton.onClick.AddListener(HideUI);
        settingsButton.onClick.AddListener(OpenSettings);
        mainMenuButton.onClick.AddListener(ExitToMainMenu);
        
        HideUI();
    }

    public void ShowUI()
    {
        gameObject.SetActive(true);
        bOpen = true;
    }

    public void HideUI()
    {
        gameObject.SetActive(false);
        bOpen = false;
    }
    
    private void OpenSettings()
    {
        throw new NotImplementedException();
    }

    private void ExitToMainMenu()
    {
        GameManager.Instance.LoadMainMenu();
    }
}
