# STRATEGY-GAME-DEMO

2D RTS Prototype — Unity 6.3 LTS

Overview

A small real-time strategy prototype built in Unity, featuring grid-based building placement, unit production, A* pathfinding, and combat. 
Built as a demonstration of core OOP principles, SOLID design, and common gameplay design patterns (Factory, Singleton, Object Pooling, Event-driven architecture).
The player can place buildings on a grid-based game board, produce combat units from barracks, select units/buildings to inspect their stats, and command units to move or attack across the map using pathfinding. 
The UI includes an infinite-scrolling production menu and an information panel that reacts to whatever is currently selected.

Features
Grid-based building placement with valid/invalid area preview
Unlimited unit & building production
Information panel showing selected building/unit stats and production options
A* pathfinding for unit movement
Infinite scrolling production menu

Design Patterns & Concepts Used
OOP: Inheritance & Polymorphism (UnitBase → MoveableUnit → Soldier, BuildingBase → Barracks/PowerPlant)
SOLID principles
Factory Pattern (BuildingFactory, UnitFactory)
Singleton (GridManager, UnitManager, PathfindingManager)
Object Pooling (ComponentPool<T>)
Event-driven architecture (EventBus)
Coroutines (movement, attack loop)
UI/Logic separation
