using System.Collections;
using UnityEngine;

public class GrowOverTime : MonoBehaviour
{
    [SerializeField] Vector3 startingScale = new Vector3(2f, 2f, 2f);
    [SerializeField] Vector3 targetScale = new Vector3(2f, 2f, 2f);
    [SerializeField] float duration = 10f;

    Coroutine growRoutine;

    public void StartGrowing()
    {
        transform.localScale = startingScale;
        if (growRoutine != null) StopCoroutine(growRoutine);
        growRoutine = StartCoroutine(Grow());
    }

    IEnumerator Grow()
    {
        Vector3 startScale = transform.localScale;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / duration);
            transform.localScale = Vector3.Lerp(startScale, targetScale, lerp);
            yield return null;
        }

        transform.localScale = targetScale;
        growRoutine = null;
    }
}
