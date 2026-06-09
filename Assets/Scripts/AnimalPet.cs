using System.Collections;
using UnityEngine;

public class AnimalPet : MonoBehaviour
{
    public float holdTime = 2f;
    public float flipDuration = 0.5f;

    float timer = 0f;
    bool flipping = false;
    bool hasFlippedThisStay = false;

    void OnTriggerStay(Collider other)
    {
        if (flipping || hasFlippedThisStay) return;

        timer += Time.deltaTime;

        if (timer >= holdTime)
        {
            hasFlippedThisStay = true;
            StartCoroutine(DoubleFlip());
        }
    }

    void OnTriggerExit(Collider other)
    {
        timer = 0f;
        hasFlippedThisStay = false;
    }

    IEnumerator DoubleFlip()
    {
        flipping = true;

        yield return StartCoroutine(Flip180());

        yield return StartCoroutine(Flip180());

        flipping = false;
        timer = 0f;
    }

    IEnumerator Flip180()
    {
        Quaternion start = transform.rotation;
        Quaternion end = start * Quaternion.Euler(0f, 0f, -180f);

        float t = 0f;
        while (t < flipDuration)
        {
            t += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(start, end, t / flipDuration);
            yield return null;
        }

        transform.rotation = end;
    }
}