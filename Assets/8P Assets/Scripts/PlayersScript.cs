using TMPro;
using UnityEngine;

public class PlayersScript : MonoBehaviour
{
	[SerializeField] protected TextMeshPro playerText;
	[SerializeField] protected GameObject refGM;
	private GameMaster8PScript _scriptGM;
	public int _currentPlayer = 1; // 1 = player one ; 2 = player two
	public bool colorHasBeenSet = false;
	public bool firstTurnToScore = false;
	public bool fault = false; // did the player do a fault on his last turn ?
	public string[] playersBallsType; // [player 1 type ; player 2 type]
	public bool hasScored = false; // did the player score one of his balls ?
	private void Start()
	{
		playersBallsType = new string[2]{"none", "none"};
		_scriptGM = refGM.GetComponent<GameMaster8PScript>();
	}

	// Update is called once per frame
	void Update()
	{
        
	}

	public void ScoredCheckIfCorrectType(string type)
	{
		if (hasScored) return;
		hasScored = playersBallsType[(_currentPlayer + 1) % 2] == type;
	}

	public void CheckIfFault(string firstBallTouched)
	{
		if (firstTurnToScore)
		{
			firstTurnToScore = false;
			fault = false;
			return;
		}
		if (playersBallsType[0] == "none")
		{
			fault = firstBallTouched == "black";
		}
		else
		{
			var currentPlayerRemaining = GetCurrentPlayerRemaining();
			fault = firstBallTouched != playersBallsType[_currentPlayer - 1] ||
			        (firstBallTouched == "black" && currentPlayerRemaining != 0);
		}

		if (fault)
		{
			StartCoroutine(_scriptGM.ShowFaultText(_currentPlayer));
		}
	}

	public int GetCurrentPlayerRemaining()
	{
		if (playersBallsType[0] == "none")
		{
			return 7;
		}
		var currentPlayerRemaining = playersBallsType[_currentPlayer - 1] == "full"
			? _scriptGM.remainingBalls[0]
			: _scriptGM.remainingBalls[1];
		return currentPlayerRemaining;
	}

	public void ChangePlayer()
	{
		_currentPlayer = _currentPlayer == 1 ? 2 : 1;
		playerText.text = "Player " + _currentPlayer + " is playing";
	}
}