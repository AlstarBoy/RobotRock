using UnityEngine;
using DG.Tweening;

// Ensures that the GameObject has a ParticleSystem component
[RequireComponent(typeof(ParticleSystem))]
public class ParticleSystemScript : MonoBehaviour
{
    private ParticleSystem particle; // Reference to the ParticleSystem component
    private ParticleSystemRenderer particleRenderer; // Renderer for the particle system
    private Material particleMaterial; // Material used for the particle system

    void Start()
    {
        // Initialize references to the ParticleSystem and its Renderer
        particle = GetComponent<ParticleSystem>();
        particleRenderer = GetComponent<ParticleSystemRenderer>();
        particleMaterial = particleRenderer.material; // Get the material used by the particle system
    }

    // Plays the particle system at a specified position with a fade-in and fade-out effect
    public void PlayParticleAtPosition(Vector3 pos)
    {
        // Access the main settings of the particle system
        var pmain = particle.main;

        // Move the particle system to the given position
        transform.position = pos;

        // Play the particle system
        particle.Play();

        // Instantly set the material's alpha to 1 (fully visible)
        particleMaterial.DOFade(1, 0);

        // Gradually fade the material's alpha to 0 over the particle system's lifetime
        particleMaterial
            .DOFade(0, pmain.startLifetime.constant) // Duration equals the particle system's lifetime
            .SetEase(Ease.InExpo); // Smooth exponential easing for the fade-out
    }
}