using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject[] celestialObjects;
    public GameObject nextObject;
    public GameObject currentObject;
    public GameObject startPos;
    public GameObject nextUI;
    public float gameSpeed = 1f;
    public bool objectPlaced=false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (nextObject == null)
        {
            nextObject = celestialObjects[Random.Range(0, celestialObjects.Length)];
        }
        pickObject();
    }

    // Update is called once per frame
    void Update()
    {
        if (objectPlaced)
        {
            pickObject();
        }
    }

    void pickObject()
    {
        if (currentObject == null || objectPlaced == true)
        {
            currentObject = nextObject;
            if (nextObject == null)
            {
                nextObject = celestialObjects[Random.Range(0, celestialObjects.Length)];
            }
            currentObject = Instantiate(currentObject, startPos.transform.position, Quaternion.identity);
            currentObject.GetComponent<placingObject>().mousePos = startPos;
            currentObject.GetComponent<placingObject>().gameC = this;
            objectPlaced = false;
        }
    }
}
