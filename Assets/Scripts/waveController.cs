using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class waveController : MonoBehaviour
{

    public float maxHeight;
    public float duration;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("starting waveController");
        StartCoroutine(Animate(duration, maxHeight));
    }

    //to run a repeating animation use the coroutine function
    IEnumerator Animate(float duration, float maxHeight)
    {
        while (true)
        {

            Vector3 startPosition = transform.position;
            float elapsedTime = 0;

            while (elapsedTime < duration)
            {
                // Adjust normalizedTime to oscillate between 0 and 1
                float normalizedTime = Mathf.PingPong(elapsedTime / (duration / 2), 1);

                // Apply the EaseInOutSine function
                float verticalOffset = EaseInOutSine(normalizedTime, 0, maxHeight, 1);

                // Update GameObject's position by adding the offset to the original position
                transform.position = new Vector3(startPosition.x, startPosition.y, startPosition.z + verticalOffset);

                // Wait for the next frame
                yield return null;

                // Increment the time elapsed since the start of the animation
                elapsedTime += Time.deltaTime;
            }

            // Reset the GameObject's position to its original position
            transform.position = startPosition;
        }
    }

    //the following function will run a interpolation animation using easeInOutSine    // EaseInOutSine Interpolation Function
    float EaseInOutSine(float t, float b, float c, float d)
    {
        return -c / 2 * (Mathf.Cos(Mathf.PI * t / d) - 1) + b;
    }

}
