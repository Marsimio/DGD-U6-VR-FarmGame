using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(GrowOverTime))]
public class BlockGrow : XRGrabInteractable
{
    GrowOverTime grow;

    protected override void Awake()
    {
        base.Awake();
        grow = GetComponent<GrowOverTime>();
    }

    public override bool IsHoverableBy(IXRHoverInteractor interactor)
    {
        if (grow != null && grow.IsGrowing)
            return interactor is XRSocketInteractor;

        return base.IsHoverableBy(interactor);
    }

    public override bool IsSelectableBy(IXRSelectInteractor interactor)
    {
        if (grow != null && grow.IsGrowing)
            return interactor is XRSocketInteractor;

        return base.IsSelectableBy(interactor);
    }
}