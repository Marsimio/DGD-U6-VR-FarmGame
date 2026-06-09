using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SpawnSeed : MonoBehaviour
{
    [SerializeField] private XRSimpleInteractable interactable;
    [SerializeField] private GameObject seedPrefab;
    [SerializeField] private Transform spawnTransform;
    private float nextTime;
    private float cooldownSeconds = 0.2f;
    private void Reset()
    {
        interactable = GetComponent<XRSimpleInteractable>();
    }


    private void OnEnable()
    {
        if (interactable != null)
            interactable.selectEntered.AddListener(OnSelectEntered);
    }

    private void OnDisable()
    {
        if (interactable != null)
            interactable.selectEntered.RemoveListener(OnSelectEntered);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (Time.time < nextTime) return;
        nextTime = Time.time + cooldownSeconds;

        if (seedPrefab == null || spawnTransform == null) return;

        if (StatManager.instance.Money >= 1)
        {
            Instantiate(seedPrefab, spawnTransform.position, spawnTransform.rotation);
            StatManager.instance.Money -= 1;
        }
    }
}