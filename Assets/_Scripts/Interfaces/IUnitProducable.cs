using System.Collections.Generic;

public interface IUnitProducable
{
    List<UnitData> ProduceableUnits { get; }
    void ProduceUnit(UnitData unitData);
}