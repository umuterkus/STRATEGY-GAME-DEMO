using UnityEngine;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour
{
    [SerializeField] private BuildingPlacementController placementController;
    [SerializeField] private SelectionController selectionController;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            HandleLeftClick();

        if (Input.GetMouseButtonDown(1))
            HandleRightClick();
    }

    private void HandleLeftClick()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (placementController.IsPlacing)
        {
            placementController.TryPlace();
            return;
        }

        selectionController.TrySelect();
    }

    private void HandleRightClick()
    {
        if (placementController.IsPlacing)
        {
            placementController.CancelPlacement();
            return;
        }

        selectionController?.TryMoveUnit();
    }
}