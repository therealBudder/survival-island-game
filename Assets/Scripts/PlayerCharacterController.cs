using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacterController : MonoBehaviour {

    public float maxHealth = 100;
    public float maxHunger = 100;
    public float health;
    public float hunger;
    public float hungerFalloff = 0.5f;
    public float hungerDamage = 5;
    public float hungerRegen = 5;
    public float regenWindow = 10;
    public Slider healthSlider;
    public Slider hungerSlider;
    public GameObject gameOverText;
    public Camera gameOverCam;
    public TextMeshProUGUI scoreText;
    public GameObject mainMenuButton;
    public GameObject pauseBackground;
    public TextMeshProUGUI pauseText;

    public bool canGetHit = true;
    
    public int mushroomsEaten;
    private int minutesSurvived;

    void Start() {

        Cursor.lockState = CursorLockMode.Locked;
        
        health = maxHealth;
        hunger = maxHunger;
        
        InvokeRepeating("ReduceHunger", 1.0f, 10);
        InvokeRepeating("UpdateHealth", 1.0f, 20);
        InvokeRepeating("CountMinutes", 1.0f, 60);

        healthSlider = GameObject.Find("Health").GetComponent<Slider>();
        hungerSlider = GameObject.Find("Hunger").GetComponent<Slider>();
        
        gameOverText = GameObject.Find("GameOverText");
        GameObject GameOverCamGO = GameObject.Find("GameOverCam");
        gameOverCam = GameOverCamGO.GetComponent<Camera>();
        GameObject scoreTextGO = GameObject.Find("ScoreText");
        scoreText = scoreTextGO.GetComponent<TextMeshProUGUI>();
        mainMenuButton = GameObject.Find("MainMenuButton");
        pauseBackground = GameObject.Find("PauseBackground");
        GameObject pauseTextGO = GameObject.Find("PauseText");
        pauseText = pauseTextGO.GetComponent<TextMeshProUGUI>();
        
        GameOverCamGO.SetActive(false);
        gameOverText.SetActive(false);
        scoreTextGO.SetActive(false);
        mainMenuButton.SetActive(false);
        pauseBackground.SetActive(false);
        pauseTextGO.SetActive(false);
        
    }
    
    void Update() {

        health = Mathf.Clamp(health, 0, maxHealth);
        hunger = Mathf.Clamp(hunger, 0, maxHunger);

        healthSlider.value = health;
        hungerSlider.value = hunger;

        if (health <= 0) {
            gameOverText.SetActive(true);
            gameOverCam.gameObject.SetActive(true);
            scoreText.text = "You survived " + minutesSurvived + " minutes and ate " + mushroomsEaten + " mushrooms";
            scoreText.gameObject.SetActive(true);
            mainMenuButton.SetActive(true);
            gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Confined;
        }
        else {
            
            if (Input.GetKey(KeyCode.Escape)) {
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0;
                pauseBackground.SetActive(true);
                pauseText.gameObject.SetActive(true);
            }

            if (Cursor.lockState == CursorLockMode.None && Input.GetMouseButton(0)) {
                Cursor.lockState = CursorLockMode.Locked;
                Time.timeScale = 1;
                pauseBackground.SetActive(false);
                pauseText.gameObject.SetActive(false);
            }
            
        }

    }

    void CountMinutes() {
        minutesSurvived += 1;
    }

    void ReduceHunger() {
        hunger -= hungerFalloff;
    }

    void UpdateHealth() {
        if (hunger <= 0) {
            health -= hungerDamage;
        }
        else if (hunger >= maxHunger - regenWindow) {
            health += hungerRegen;
            hunger -= hungerFalloff * 3;
        }
    }
    
}
