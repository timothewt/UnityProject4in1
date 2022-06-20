using System;
using TMPro;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    public TextMeshPro refTextColors;
    public Rigidbody2D rb2D;
    public GameObject refPlayer;
    public PlayersScript refPlayersScript;
    public GameObject refGM;
    public GameMaster8PScript refGMScript;
    public GameObject refCue;
    public CueScript _refCueScript;
    public string type; // "striped" or "full"
    private void Start()
    {
        rb2D = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public float GetVelocity()
    {
        var speedVect = rb2D.velocity;
        var speed = (float)Math.Sqrt(Math.Pow(speedVect.x, 2) + Math.Pow(speedVect.y, 2));
        return speed;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("hole"))
        {
            if (this.CompareTag("white"))
            {
                refGMScript.ToggleReplaceWB(true);
                StartCoroutine(refGMScript.ShowFaultText(refPlayersScript._currentPlayer));
                refPlayersScript.ChangePlayer();
            }
            else
            {
                if (!refPlayersScript.colorHasBeenSet)
                {
                    refPlayersScript.firstTurnToScore = true;
                    refPlayersScript.colorHasBeenSet = true;
                    if (refPlayersScript._currentPlayer == 1)
                    {
                        if (this.type == "striped")
                        {
                            refPlayersScript.playersBallsType = new string[2]{ "striped", "full" };
                        }
                        else
                        {
                            refPlayersScript.playersBallsType = new string[2]{ "full", "striped" };
                        }
                    }
                    else
                    {
                        if (this.type == "full")
                        {
                            refPlayersScript.playersBallsType = new string[2]{ "striped", "full" };
                        }
                        else
                        {
                            refPlayersScript.playersBallsType = new string[2]{ "full", "striped" };
                        }
                    }

                    refTextColors.text = "P1: " + refPlayersScript.playersBallsType[0] + " P2: " +
                                         refPlayersScript.playersBallsType[1];
                }
                refPlayersScript.ScoredCheckIfCorrectType(this.type);
                if (this.type == "full")
                {
                    refGMScript.remainingBalls[0]--;
                    
                }
                else if (this.type == "striped")
                {
                    refGMScript.remainingBalls[1]--;
                }
                else
                {
                    var remainingBalls = refPlayersScript.GetCurrentPlayerRemaining();
                    if (remainingBalls == 0)
                    {
                        refGMScript.GameOver(refPlayersScript._currentPlayer);
                    }
                    else
                    {
                        refGMScript.GameOver(refPlayersScript._currentPlayer % 2 + 1);
                    }
                }
                refGMScript.RemoveBall(this.name);
            }
        }

        if (col.gameObject.CompareTag("ball") && this.CompareTag("white") && _refCueScript._firstBallHit == "none")
        {
            _refCueScript._firstBallHit = col.gameObject.GetComponent<BallScript>().type;
        }
    }
}
