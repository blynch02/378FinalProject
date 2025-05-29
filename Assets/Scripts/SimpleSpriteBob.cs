using UnityEngine;

public class SimpleSpriteBob : MonoBehaviour
{
    [Tooltip("The speed of the bobbing motion.")]
    public float bobSpeed = 1.5f;

    [Tooltip("The amount the sprite will move up and down on its Y-axis, relative to its original scale. E.g., 0.05 for a 5% change.")]
    public float bobAmount = 0.05f;

    private Vector3 initialLocalScale;
    private float randomOffset;

    void Start()
    {
        // Store the initial local scale of the GameObject
        initialLocalScale = transform.localScale;
        // Generate a random offset for the sine wave to prevent all sprites from bobbing in perfect sync
        randomOffset = Random.Range(0f, Mathf.PI * 2f); // Full cycle offset
    }

    void Update()
    {
        // Calculate the new Y scale
        float newYScale = initialLocalScale.y * (1f + Mathf.Sin(Time.time * bobSpeed + randomOffset) * bobAmount);

        // Apply the new scale, keeping original X and Z scales
        transform.localScale = new Vector3(initialLocalScale.x, newYScale, initialLocalScale.z);
    }

    // Optional: Public method to temporarily disable bobbing (e.g., during an attack)
    public void SetBobbing(bool isBobbing)
    {
        this.enabled = isBobbing;
        // If disabling, you might want to reset to the initial scale
        if (!isBobbing)
        {
            transform.localScale = initialLocalScale;
        }
    }
}
