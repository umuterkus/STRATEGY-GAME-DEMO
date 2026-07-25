using UnityEngine;

public class SelectionController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    private ISelectable currentUnit;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    public void TrySelect()
    {
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            ISelectable selectable = hit.collider.GetComponentInParent<ISelectable>();
            if (selectable != null)
            {
                if (currentUnit != selectable)
                {
                    currentUnit?.Deselect();
                    currentUnit = selectable;
                    currentUnit.Select();
                }
                return;
            }
        }

        currentUnit?.Deselect();
        currentUnit = null;
    }

    public void TryMoveUnit()
    {
        if (currentUnit == null) return;

        if (currentUnit is IMoveable moveable)
        {
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            moveable.MoveTo(mousePos);
        }
    }
}