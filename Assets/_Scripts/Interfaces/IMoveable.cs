using UnityEngine;

public interface IMoveable
{
    void MoveTo(Vector2 targetPosition);
    bool IsMoving { get; }
}

