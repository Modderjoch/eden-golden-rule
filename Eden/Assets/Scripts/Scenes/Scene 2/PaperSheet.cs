using UnityEngine;

public class PaperSheet : MonoBehaviour
{
    public float blowForce = 10f;

    public float maxSideForce = 2f;

    private AudioSource audioSource;
    private bool audioIsPlaying = false;

    private Rigidbody rb;

    protected void Update()
    {
        if (audioIsPlaying)
        {
            if(audioSource != null)
            {
                if (audioSource.isPlaying)
                {
                    audioSource.volume = Mathf.Clamp01((rb.velocity.x + rb.velocity.y + rb.velocity.z) / 3);
                }
            }
        }
    }

    public void Blow()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            float sideForceX = Random.Range(-maxSideForce, maxSideForce);
            float sideForceZ = Random.Range(-maxSideForce, maxSideForce);

            rb.AddForce(Vector3.up * blowForce, ForceMode.Impulse);

            Vector3 sideForce = new Vector3(sideForceX, 0f, sideForceZ);
            rb.AddForce(sideForce, ForceMode.Impulse);
        }
        else
        {
            Debug.LogWarning("Rigidbody component not found. Add a Rigidbody component to the object to use BlowIntoAir.");
        }

        AudioManager.Instance.Play("PaperFlying");
        audioSource = AudioManager.Instance.ReturnAudioSource("PaperFlying");
        audioIsPlaying = true;
    }
}
