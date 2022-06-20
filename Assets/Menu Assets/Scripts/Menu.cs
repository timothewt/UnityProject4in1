using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Menu : MonoBehaviour
{

    [SerializeField] public TextMeshPro refQuitText;
    [SerializeField] public GameObject refLogoAC;
    [SerializeField] public GameObject refLogoBB;
    [SerializeField] public GameObject refLogoFB;
    [SerializeField] public GameObject refButonAC;
    [SerializeField] public GameObject refButonBB;
    [SerializeField] public GameObject refButonFB;
    [SerializeField] public GameObject refVidPlayer;
    [SerializeField] public GameObject refInfoText;
    AudioSource refAudioSource1;
    AudioSource refAudioSource2;
    bool escapeactive;

    // Start is called before the first frame update
    void Start()
    {
        escapeactive = false;
        refAudioSource1 = GetComponent<AudioSource>();
        refAudioSource2 = refVidPlayer.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (escapeactive == false)
            {
                refQuitText.text = "Are you sure you want to leave us ? \n\n Press Q to quit \n\n Press escape again to stay";
                ToggleButtonsStates(false);
                escapeactive = true;
            }
            else
            {
                refQuitText.text = "";
                ToggleButtonsStates(true);
                escapeactive = false;
            }

        }

        if (Input.GetKeyDown(KeyCode.Q) && escapeactive)
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            ChangeTo8Pool();
        }



    }

    private void ToggleButtonsStates(bool state)
    {
        refButonAC.SetActive(state);
        refButonBB.SetActive(state);
        refButonFB.SetActive(state);
        refLogoAC.SetActive(state);
        refLogoBB.SetActive(state);
        refLogoFB.SetActive(state);
        refInfoText.SetActive(state);
    }

    public IEnumerator ChangeScene(string name)
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(name, LoadSceneMode.Single);
    }
    public void ChangeToFB()
    {
        StartCoroutine(ChangeScene("FBGame"));
        refAudioSource1.volume = 0;
        refAudioSource2.Play(0);
    }

    public void ChangeToAC()
    {
        StartCoroutine(ChangeScene("ACMenu"));
        refAudioSource1.volume = 0;
        refAudioSource2.Play(0);
    }

    public void ChangeToBB()
    {
        StartCoroutine(ChangeScene("BBGame"));
        refAudioSource1.volume = 0;
        refAudioSource2.Play(0);
    }

    public void ChangeTo8Pool()
    {
        StartCoroutine(ChangeScene("PoolGame"));
        refAudioSource1.volume = 0;
        refAudioSource2.Play(0);
    }
}
