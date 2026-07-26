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

        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        IDamageable damageableTarget = null;
        if (hit.collider != null)
            damageableTarget = hit.collider.GetComponentInParent<IDamageable>();

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