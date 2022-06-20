using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMaster8PScript : MonoBehaviour
{
    [SerializeField] protected TextMeshPro refFaultText;
    [SerializeField] protected TextMeshPro endGameText;
    [SerializeField] protected TextMeshPro refTextColors;
    [SerializeField] protected GameObject prefabBall;
    [SerializeField] protected Sprite[] ballsSprites;
    [SerializeField] protected GameObject refPlayers;
    private PlayersScript _refPlayersScript;
    [SerializeField] protected GameObject refCue;
    private CueScript _cueScript;
    private const float BallSize = .25f;
    private readonly Vector2 TerrainSize = new Vector2(10f, 5f);
    private float _heightDiff;
    public readonly Vector3 WhiteBallPos = new Vector3(-3f, 0f,0f);
    private readonly Vector3 _firstBallPos = new Vector3(2f, 0f,0f);
    private GameObject[] _balls = new GameObject[15];
    public int[] remainingBalls = new int[]{7,7}; // [full; striped}
    private GameObject _whiteBall;
    private GameObject _blackBall; 
    public bool isPlacingWB = false;
    public bool gameIsOver = false;
    private void Start()
    {
        _refPlayersScript = refPlayers.GetComponent<PlayersScript>();
        _cueScript = refCue.GetComponent<CueScript>();
        _heightDiff = (float) Math.Round(Math.Sqrt(Math.Pow(BallSize, 2) - Math.Pow(BallSize / 2, 2)),2);
        _whiteBall = Instantiate(prefabBall);
        _whiteBall.transform.position = WhiteBallPos;
        _whiteBall.GetComponent<SpriteRenderer>().sprite = ballsSprites[15];
        _whiteBall.tag = "white";
        var wbScript = _whiteBall.GetComponent<BallScript>();
        wbScript.refGM = this.gameObject;
        wbScript.refGMScript = this.GetComponent<GameMaster8PScript>();
        wbScript.refCue = this.refCue;
        wbScript._refCueScript = refCue.GetComponent<CueScript>();
        wbScript.refPlayer = this.refPlayers;
        wbScript.refPlayersScript = this._refPlayersScript;
        refCue.GetComponent<CueScript>().refWhiteBall = _whiteBall;
        for (var i = 0; i < _balls.Length; i++)
        {
            SetBallProperties(i);
            if (i == 8) _blackBall = _balls[i];
        }
        _blackBall.tag = "black";
    }

    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(TitleScript.LoadScene("Menu"));
        }
        if (gameIsOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
        else
        {
            if (!isPlacingWB) return;
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            _whiteBall.transform.position = mousePos;
            if (Input.GetMouseButtonDown(0) && GetMaxBallSpeed() <= .01f)
            {
                ToggleReplaceWB(!CheckAvailableSpot());
            }
        }
    }
    private void SetBallProperties(int i)
    {
        _balls[i] = Instantiate(prefabBall);
        var script = _balls[i].GetComponent<BallScript>();
        script.refTextColors = this.refTextColors;
        script.type = i < 7 ? "full": "striped";
        script.type = i == 7 ? "black" : script.type;
        script.refGM = this.gameObject;
        script.refCue = this.refCue;
        script.refGMScript = this.GetComponent<GameMaster8PScript>();
        script._refCueScript = refCue.GetComponent<CueScript>();
        script.refPlayer = this.refPlayers;
        script.refPlayersScript = this._refPlayersScript;
        script._refCueScript = refCue.GetComponent<CueScript>();
        _balls[i].transform.position = SetBallPos(i, _firstBallPos);
        _balls[i].GetComponent<SpriteRenderer>().sprite = ballsSprites[i];
        _balls[i].name = "ball" + i;
    }

    public void ToggleReplaceWB(bool isPlacing)
    {
        _whiteBall.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        refCue.SetActive(!isPlacing);
        if (!isPlacing)
        {
            _cueScript._guidingLine = _cueScript.CreateGuidingLine(_cueScript.refWhiteBall.gameObject.transform.position);
        }
        _whiteBall.GetComponent<CircleCollider2D>().isTrigger = isPlacing;
        isPlacingWB = isPlacing;
    }
    public void RemoveBall(string name)
    {
        var indexToRemove = 0;
        for (int i = 0; i < _balls.Length; i++)
        {
            if (_balls[i].name == name)
            {
                indexToRemove = i;
                break;
            }
        }
        Destroy(_balls[indexToRemove].gameObject);
        _balls = _balls.Where((source, index) => index != indexToRemove).ToArray();
    }

    public float GetMaxBallSpeed()
    {
        var max = _whiteBall.GetComponent<BallScript>().GetVelocity();
        foreach (var ball in _balls)
        {
            max = Math.Max(max,ball.GetComponent<BallScript>().GetVelocity());
        }
        return max;
    }

    public IEnumerator ShowFaultText(int player)
    {
        refFaultText.text = "Player " + player + " has faulted!";
        yield return new WaitForSeconds(1);
        refFaultText.text = " ";
    }

    public void GameOver(int winner)
    {
        endGameText.text = "Player " + winner + " won!\n<size=8>Press [Esc] to quit or [_] to play again";
        gameIsOver = true;
    }

    private Vector3 SetBallPos(int i, Vector3 firstBallPos)
    {
        switch (i)
        {
            case 0:
                return new Vector3(2.0f, 0f, 0);
            case 9:
                return new Vector3(2.2f, -0.1f, 0);
            case 1:
                return new Vector3(2.2f, 0.1f, 0);
            case 2:
                return new Vector3(2.4f, -0.3f, 0);
            case 7:
                return new Vector3(2.4f, 0f, 0);
            case 11:
                return new Vector3(2.4f, 0.3f, 0);
            case 14:
                return new Vector3(2.65f, -0.4f, 0);
            case 4:
                return new Vector3(2.65f, -0.1f, 0);
            case 10:
                return new Vector3(2.65f, 0.1f, 0);
            case 6:
                return new Vector3(2.65f, 0.4f, 0);
            case 3:
                return new Vector3(2.9f, -0.5f, 0);
            case 8:
                return new Vector3(2.9f, -0.3f, 0);
            case 13:
                return new Vector3(2.9f, 0f, 0);
            case 5:
                return new Vector3(2.9f, 0.3f, 0);
            case 12:
                return new Vector3(2.9f, 0.5f, 0);
            default:
                return firstBallPos;
        }
    }
    /**
     * Checks if the white ball is over another ball or out of the field/on a hole when replacing it
     */
    private bool CheckAvailableSpot()
    {
        var res = true;
        var wBPos = _whiteBall.transform.position;
        if (!(IsIn(wBPos.x, -TerrainSize.x / 2, TerrainSize.x / 2) &&
              IsIn(wBPos.y, -TerrainSize.y / 2, TerrainSize.y / 2)))
        {
            return false;
        }
        foreach (var ball in _balls)
        {
            Vector2 relativeWBPos = wBPos - ball.transform.position; // relative to the current ball 
            if (IsIn(relativeWBPos.x, -BallSize, BallSize) &&
                IsIn(relativeWBPos.y, -BallSize, BallSize))
            {
                res = false;
                break;
            }
        }
        return res;
    }
    
    public static bool IsIn(float x, float inf, float sup)
    {
        return x <= sup && x >= inf;
    }
}
