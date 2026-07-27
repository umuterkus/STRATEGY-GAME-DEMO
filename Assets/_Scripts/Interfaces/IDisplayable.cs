using UnityEngine;

public interface IDisplayable
{
    //This is extra but, most of the RTS games Have Max Health and Current Health shown most of the time. 
    //I added to units and buildings but didnt used intentionaly.
    string DisplayName { get; }
    Sprite DisplayIcon { get; }
    int CurrentHealth { get; }
    int MaxHealth { get; }
}