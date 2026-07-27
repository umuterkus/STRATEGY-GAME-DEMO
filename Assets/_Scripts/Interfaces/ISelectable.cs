using UnityEngine;

// A simple interface for objects that can be selected and deselected by the player.
public interface ISelectable
{
    void Select();
    void Deselect();
}
