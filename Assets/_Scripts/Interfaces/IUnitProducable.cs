using System.Collections.Generic;

public interface IUnitProducable
{
    List<SoldierData> ProduceableUnits { get; }
    void ProduceUnit(SoldierData unitData);
}