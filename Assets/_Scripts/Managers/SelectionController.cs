using UnityEngine;

public class SelectionController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera; // reference to the camera used for clicking
    private ISelectable currentUnit; // the unit that is currently selected

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    public void TrySelect()
    {
        // Convert mouse screen position to world position
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // Shoots a 2D raycast that clicked
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            // check if the clicked object is ISelectable
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


    // If a unit is selected and the player clicks on an enemy, the unit attacks that enemy.
    // If the player clicks on empty ground instead, the unit stops attacking and walks to that spot.
    public void TryMoveUnit()
    {
        if (currentUnit == null) return;

        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        IDamageable damageableTarget = null;
        if (hit.collider != null)
            damageableTarget = hit.collider.GetComponentInParent<IDamageable>();  // Check if selected can take damage


        // If we clicked on something damageable that is NOT the currently selected unit itself
        bool clickedAttackableTarget = damageableTarget != null && !ReferenceEquals(damageableTarget, currentUnit);


        if (clickedAttackableTarget && currentUnit is IAttacker attacker)
        {
            attacker.AttackTarget(damageableTarget);
            return;
        }

        if (currentUnit is IMoveable moveable)
        {
            if (currentUnit is IAttacker activeAttacker)
                activeAttacker.CancelAttack();

            moveable.MoveTo(mousePos);
        }
    }
}