using System.Collections.Generic;

public interface IUnitProducer
{
    List<SoldierData> ProduceableUnits { get; }
    void ProduceUnit(SoldierData unitData);
}