using System;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class CueScript : MonoBehaviour
{
    public GameObject refWhiteBall;
    [SerializeField] protected GameObject refGM; // game master
    private GameMaster8PScript _refGMScript;
    [SerializeField] protected GameObject prefabGuidingLine; // guiding line prefab
    [SerializeField] protected GameObject refPlayers;
    private PlayersScript refPlayersScript;
    public GameObject _guidingLine;
    public bool isMoving = false; // is there a ball still moving on the field ?
    private int _isLoading = 0; // 0, normal state, aiming ; 1, is loading a shot ; 2, has shot
    private float _shotAngle;
    private float _shotForce;
    public string _firstBallHit = "none"; // none ; striped ; full ; black
    
    private void Start()
    {
        _refGMScript = refGM.GetComponent<GameMaster8PScript>();
        refPlayersScript = refPlayers.GetComponent<PlayersScript>();
        _guidingLine = CreateGuidingLine(refGM.GetComponent<GameMaster8PScript>().WhiteBallPos);
    }
    private void Update()
    {
        if (_refGMScript.gameIsOver) return;
        if (isMoving)
        {
            if (_refGMScript.GetMaxBallSpeed() > .01f) return;
            isMoving = false;
            refPlayersScript.CheckIfFault(_firstBallHit);
            if (!refPlayersScript.hasScored && !refPlayersScript.fault)
            {
                refPlayersScript.ChangePlayer();
            }
            _firstBallHit = "none";
            if (refPlayersScript.fault)
            {
                refPlayersScript.ChangePlayer();
                _refGMScript.ToggleReplaceWB(true);
            }
            else
            {
                _guidingLine = CreateGuidingLine(refWhiteBall.gameObject.transform.position);
            }
            refPlayersScript.hasScored = false;
        }
        else if (_refGMScript.isPlacingWB)
        {
            
        }
        else
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = -1f;
            var wBallPos = refWhiteBall.gameObject.transform.position;
            var relativePos = mousePos - wBallPos;
            switch (_isLoading)
            {
                case 0 :
                    _shotAngle = (float)Math.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg + 180;
                    SetPosAngle(this.gameObject, _shotAngle, wBallPos);
                    _guidingLine.transform.localEulerAngles = new Vector3(0, 0, _shotAngle);
                    _isLoading = Input.GetMouseButtonDown(0) ? 1 : 0;
                    break;
                 case 1:
                     var angle = GetAngleCueWBcomaMouseWB(mousePos, wBallPos);
                     var magnitude = Magnitude2D(relativePos);
                     _shotForce = GameMaster8PScript.IsIn(angle, 0, (float)Math.PI / 2 -.1f)
                         ? (float)(Math.Cos(angle) * magnitude)
                         : 0;
                     UpdateGuidingLineSize();
                     var cueAngleRad = Mathf.Deg2Rad * (_shotAngle - 180);
                     UpdateCuePos(wBallPos, cueAngleRad);
                     if (Input.GetMouseButtonUp(0))
                     {
                         _isLoading = 2;
                     } else if (Input.GetMouseButtonDown(1))
                     {
                         ResetGuidingLineSize();
                         _isLoading = 0;
                     }
                     break;
                 case 2:
                     Destroy(_guidingLine);
                     Shoot(_shotAngle, _shotForce * 4, wBallPos);
                     _isLoading = 0;
                     break;
                     
            }
        }
    }
    private void ResetGuidingLineSize()
    {
        _guidingLine.transform.localScale =
            new Vector3(prefabGuidingLine.transform.localScale.x, prefabGuidingLine.transform.localScale.y, 0);
    }

    private void UpdateGuidingLineSize()
    {
        _guidingLine.transform.localScale =
            new Vector3(_shotForce / (float)Math.PI, _guidingLine.transform.localScale.y, 0);
    }

    private void UpdateCuePos(Vector3 wBallPos, float cueAngleRad)
    {
        this.transform.position = new Vector3(
            wBallPos.x + (float)Math.Cos(cueAngleRad) * _shotForce,
            wBallPos.y + (float)Math.Sin(cueAngleRad) * _shotForce, wBallPos.z);
    }


    private float Magnitude2D(Vector3 vect)
    {
        return (float)Math.Sqrt(Sq(vect.x) + Sq(vect.y));
    }
    public GameObject CreateGuidingLine(Vector3 pos)
    {
        Destroy(_guidingLine);
        var guidingLine = Instantiate(prefabGuidingLine);
        guidingLine.transform.position = pos;
        return guidingLine;
    }
    private float GetAngleCueWBcomaMouseWB(Vector2 mousePos, Vector2 wBallPos)
    {
        var mouseWB = (float)Math.Sqrt(Sq(mousePos.x - wBallPos.x) +
                                       Sq(mousePos.y - wBallPos.y));
        var cueWB = (float)Math.Sqrt(Sq(this.transform.position.x - wBallPos.x) +
                                     Sq(this.transform.position.y - wBallPos.y));
        var cueMouse = (float)Math.Sqrt(Sq(this.transform.position.x - mousePos.x) +
                                        Sq(this.transform.position.y - mousePos.y));
        if (cueWB == 0 || mouseWB == 0)
        {
            return 0;
        }
        else
        {
            return (float)Math.Acos((Sq(cueWB) + Sq(mouseWB) - Sq(cueMouse)) / (2 * cueWB * mouseWB));
        }
    }

    private float Sq(float x)
    {
        return (float)Math.Pow(x, 2);
    }
    private void SetPosAngle(GameObject obj, float angle, Vector3 wBallPos)
    {
        obj.transform.localEulerAngles = new Vector3(0, 0, angle);
        obj.transform.position = new Vector3(wBallPos.x, wBallPos.y, -2f);
    }

    private void Shoot(float angle, float force, Vector3 wBallPos)
    {
        this.transform.position = wBallPos;
        isMoving = true;
        var dir = (Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.right);
        refWhiteBall.GetComponent<BallScript>().rb2D.AddForce(dir * force,ForceMode2D.Impulse);
    }
}
