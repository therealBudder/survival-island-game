using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour {
    
    private bool buttonPressed = false;

    public void PlayGame() {

        if (!buttonPressed) {
            StartCoroutine(SetActiveAndUnload("MenuScene", "SampleScene"));
            buttonPressed = true;
        }
        
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void SetSeed(string seed) {

        MainController.Instance.seed = int.Parse(seed);

    }

    public void MainMenu() {

        StartCoroutine(SetActiveAndUnload("SampleScene", "MenuScene"));
        buttonPressed = true;

    }

    IEnumerator SetActiveAndUnload(string currentScene, string nextScene) {

        yield return SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Additive);
        
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(nextScene));
        SceneManager.UnloadSceneAsync(currentScene);

        buttonPressed = false;

    }
}
