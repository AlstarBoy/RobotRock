using UnityEngine;

public class ItemStats : MonoBehaviour
{
    [Header("Stats")]
    public int planetTier;
    public float size;
    public float growSpeed = 0.1f;
    public float growTime;
    public bool canGrow = false;
    private placingObject pObject;

    [Header("Grow Timer")]
    public Vector3 targetScale = new Vector3(2f, 2f, 2f);
    public float duration = 20f;
    private float elapsedTime = 0f;
    public bool loop = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pObject = GetComponent<placingObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pObject.isPlaced)
        {
            canGrow = true;
            print("placed");
            growOverTime(growSpeed);
        }    
    }

    void growOverTime(float speed)
    {
        if (canGrow == true)
        {
            if (elapsedTime < duration)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
            }
            else if (loop)
            {
                elapsedTime = 0f;
                (transform.localScale, targetScale) = (targetScale, transform.localScale);
            }
        }
    }
}
