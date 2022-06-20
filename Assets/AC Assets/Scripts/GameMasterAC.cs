using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameMasterAC : MonoBehaviour
{
    private const float ScreenXLimit = 7.5f; // horizontal screen limit
    private const float ProbaBomb = .1f; // probability to spawn a bomb (in the interval [0;1])
    private float spawnTimer; // time before another apple or bomb spawns
    readonly Vector2 TimeInterval = new Vector2(.4f, 1f); // minimum and maximum durations between each spawn
    [SerializeField] protected GameObject prefPomme; // refers to the prefab of an apple
    [SerializeField] protected GameObject prefBomb; // refers to the prefab of a bomb
    [SerializeField] protected TextMeshPro pauseText; // refers to the 'Pause' textmeshpro component
    [SerializeField] protected BoxScript box; // refers to the box
    [SerializeField] protected AudioClip backgronudMusic; // refers to the background music audio
    private AudioSource sfxPlayer; // audio player
    public bool isPaused = false;

    private void Start()
    {
        sfxPlayer = gameObject.AddComponent<AudioSource>(); // creates the audio source
        sfxPlayer.clip = backgronudMusic; // sets the background music
        sfxPlayer.volume = .6f; // changes the volume to 60%
        sfxPlayer.loop = true; // loops the sound
        sfxPlayer.Play(); // begins to play the music
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 1;
            StartCoroutine(TitleScript.LoadScene("ACMenu"));
        }
        if (Input.GetKeyDown(KeyCode.P)) TogglePause();
        if (isPaused) return;
        spawnTimer -= Time.deltaTime; // subtracts elapsed time since last frame to timer
        if (!(spawnTimer <= 0 && box.lives > 0)) return; // if the timer still has time AND the player is still playing
        var newObject = Random.Range(0f, 1f) <= ProbaBomb // instantiates a new object randomly between bomb and apple
            ? Instantiate(prefBomb) : Instantiate(prefPomme); // if the random gives us the probability that it's a bomb instantiates a bomb otherwise instantiates an apple
        newObject.GetComponent<AppleScript>().box = this.box; // gives the reference of the box to the new object
        var posX = Random.Range(-ScreenXLimit, ScreenXLimit); // gives it a random horizontal position
        newObject.transform.position = new Vector3(posX, 6, 0); // updates its position accordingly
        spawnTimer = Random.Range(TimeInterval.x, TimeInterval.y); // sets a new timer for the spawn
    }

    private void TogglePause()
    {
        if (box.lives <= 0) return; 
        if (isPaused)
        {
            pauseText.transform.position = new Vector3(0, 0, 10);
            Time.timeScale = 1;
            isPaused = false;
        }
        else
        {
            pauseText.transform.position = new Vector3(0, 0, -5);
            Time.timeScale = 0;
            isPaused = true;
        }
    }
}
