using UnityEngine;

public class SelectionController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    private ISelectable currentSelected;

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
                if (currentSelected != selectable)
                {
                    currentSelected?.Deselect();
                    currentSelected = selectable;
                    currentSelected.Select();
                }
                return;
            }
        }

        currentSelected?.Deselect();
        currentSelected = null;
    }
}