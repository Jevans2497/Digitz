using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {

    private void Start() {
        AudioManager.Instance.playMenuMusic(false);
    }

    public void startGame() {
        SceneManager.LoadSceneAsync("DDR");
    }

    public void startTutorial() {
        SceneManager.LoadSceneAsync("Tutorial");
    }

    public void quitGame() {
        Application.Quit();
    }
}
