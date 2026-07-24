using UnityEngine;

public class SelectionController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

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
            BuildingBase building = hit.collider.GetComponentInParent<BuildingBase>();
            if (building != null)
            {
                Debug.Log("Bina Seçildi: " + building.BuildingData.BuildingName);
                EventBus.OnBuildingSelected?.Invoke(building);
                return;
            }
        }

        Debug.Log("clciked empty space");
        EventBus.OnBuildingSelected?.Invoke(null);
    }
}