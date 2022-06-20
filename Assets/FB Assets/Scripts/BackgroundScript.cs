using UnityEngine;

public class BackgroundScript : MonoBehaviour
{
    [SerializeField] protected GameObject skyPrefab;
    [SerializeField] protected GameObject mountainsPrefab;
    [SerializeField] protected GameObject cloudsPrefab;
    [SerializeField] protected GameObject gameMaster;
    // skyWidth = 36f;              Constants to initialize on the unity game scene on the default ones already there
    // cloudsWidth = 18f;
    // moutainsWidth = 34f;
    // skySpeed = 1f;
    // cloudsSpeed = 1.5f;
    // moutainsSpeed = 2f;

    public void CreateNewBgElement(string type)
    {
        GameObject newElem = null;
        switch (type)
        {
            case "sky":
                newElem = Instantiate(skyPrefab);
                break;
            case "clouds":
                newElem = Instantiate(cloudsPrefab);
                break;
            case "mountains":
                newElem = Instantiate(mountainsPrefab);
                break;
            default:
                newElem = Instantiate(cloudsPrefab);
                break;
        }
        var newElemScript = newElem.GetComponent<BackgroundElementScript>();
        newElemScript.bgScript = this.GetComponent<BackgroundScript>();
    }
}
