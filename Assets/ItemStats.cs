using UnityEngine;
using System.Collections;

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
            StartCoroutine(Grow(new Vector3(2f, 2f, 2f), 60f));
        }    
    }



    void growOverTime(float speed)
    {
        if (canGrow)
        {
            if (elapsedTime < duration)
            {
                print("Grow");
                transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, speed * Time.deltaTime);
                elapsedTime += Time.deltaTime;
            }
            else if (loop)
            {
                elapsedTime = 0f;
                (transform.localScale, targetScale) = (targetScale, transform.localScale);
            }
        }
    }

    IEnumerator Grow(Vector3 targetScale, float duration)
    {
        Vector3 startScale = transform.localScale; // Capture the initial scale
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        transform.localScale = targetScale; // Ensure it reaches exact size
    }
}

