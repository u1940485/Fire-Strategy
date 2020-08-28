using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Scene_Manager : MonoBehaviour {

    public SoundManager soundManager;

    void Awake()
    {

    }

    // Use this for initialization
    void Start()
    {


    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void loadTitleScene() {

        soundManager.storeAudioVolume();
        SceneManager.LoadScene(0);

    }

    public void loadGameAsBlueTeamForPlayer()
    {
        soundManager.storeAudioVolume();
        PlayerPrefs.SetString("playerteam", "blue");
        loadGameScene();

    }

    public void loadGameAsRedTeamForPlayer()
    {
        soundManager.storeAudioVolume();
        PlayerPrefs.SetString("playerteam", "red");
        loadGameScene();

    }
    public void loadGameScene()
    {

        SceneManager.LoadScene(1);
    }

    public void loadGameOverScene()
    {
        soundManager.storeAudioVolume();
        SceneManager.LoadScene(2);

    }

    public void loadVictoryScene()
    {
        soundManager.storeAudioVolume();
        SceneManager.LoadScene(3);

    }

    public void quitGame() {

        Application.Quit();
        
    }
}
