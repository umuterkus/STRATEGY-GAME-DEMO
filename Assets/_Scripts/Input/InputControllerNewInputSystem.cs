using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputControllerNewInputSystem : MonoBehaviour
{
    [SerializeField] private BuildingPlacementController placementController;
    [SerializeField] private SelectionController selectionController;

    private GameControls controls;

    private void Awake()
    {
        controls = new GameControls();
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
        controls.Gameplay.Select.performed += OnSelectPerformed;
        controls.Gameplay.MoveOrAttack.performed += OnMoveOrAttackPerformed;
    }

    private void OnDisable()
    {
        controls.Gameplay.Select.performed -= OnSelectPerformed;
        controls.Gameplay.MoveOrAttack.performed -= OnMoveOrAttackPerformed;
        controls.Gameplay.Disable();
    }

    private void OnSelectPerformed(InputAction.CallbackContext context)
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

    private void OnMoveOrAttackPerformed(InputAction.CallbackContext context)
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (placementController.IsPlacing)
        {
            placementController.CancelPlacement();
            return;
        }

        selectionController?.TryMoveUnit();
    }
}