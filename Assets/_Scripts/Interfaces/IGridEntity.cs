using System;


public interface IGridEntity
{

    event Action<IGridEntity> OnDespawned;
}
