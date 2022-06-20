using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScript : MonoBehaviour
{
    [SerializeField] protected TextMeshPro textLivesGm; // refers to the text of the Lives game mode
    [SerializeField] protected TextMeshPro textTimeGm; // refers to the text of the Timer game mode
    public static int GameMode = 0; // game mode of the game : 0 = lives; 1 = time
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(LoadScene("Menu"));
        }
        if (Input.GetKey(KeyCode.Return)) // if the user hits return
        {
            StartCoroutine(LoadScene("ACGame")); // changes the scene starts the game 
        } else if (Input.GetKey(KeyCode.L)) // if he hits the L key
        {
            GameMode = 0; // changes the game mode to 0
            textLivesGm.text = "- Lives (3) (selected)"; // updates the texts accordingly
            textTimeGm.text = "- Time (2min)";
        } else if (Input.GetKey(KeyCode.T)) // if he hits the T key
        {
            GameMode = 1; // changes the game mode to 1 
            textTimeGm.text = "- Time (2min) (selected)"; // updates the texts accordingly
            textLivesGm.text = "- Lives (3)";
        }
    }

    public static IEnumerator LoadScene(string scene) // loads another scene
    {
        var load = SceneManager.LoadSceneAsync(scene);
        while (!load.isDone)
        {
            yield return null;
        }
    }
}
