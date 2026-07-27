using System;


//The base interface for objects that can be registered on the grid system and notify listeners when they despawn.

public interface IGridEntity
{

    event Action<IGridEntity> OnDespawned;
}
