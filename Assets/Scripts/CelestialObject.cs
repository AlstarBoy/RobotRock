
using UnityEngine;

/// <summary>
/// Represents a celestial object with tier and merge data.
/// </summary>
public class CelestialObject : MonoBehaviour
{
    public string objectName;
    public int tier;
    public SpriteRenderer spriteRenderer;

    public Vector2Int gridPosition;
    public Tile currentTile;

    public ParticleSystem mergeEffect;
    public AudioClip mergeSound;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayMergeEffect()
    {
        if (mergeEffect != null)
            Instantiate(mergeEffect, transform.position, Quaternion.identity);

        if (mergeSound != null && audioSource != null)
            audioSource.PlayOneShot(mergeSound);
    }

    public void MergeInto()
    {
        PlayMergeEffect();
        ObjectPooler.Instance.ReturnToPool(this.gameObject);
    }

    private void OnMouseEnter()
    {
        if (MergeManager.Instance.CanMerge(this))
            spriteRenderer.color = Color.cyan; // Highlight
    }

    private void OnMouseExit()
    {
        spriteRenderer.color = Color.white; // Reset
    }
}
