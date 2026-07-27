using System.Collections.Generic;


// Lists which units, a building (like a barracks) can produce and defines the method.
public interface IUnitProducable
{
    List<UnitData> ProduceableUnits { get; }
    void ProduceUnit(UnitData unitData);
}