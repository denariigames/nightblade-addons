# World Interactions

**World Interactions** provides the framework for non-networked, world objects that support targeting, movement, stopping, and interaction. Every BaseGameEntity has networking overhead. World Interaction Targets allows for massive quantity of NPCs, harvestables, etc where client-only interaction is desirable.

- This is primarily a _developer_ framework, meaning you will be expected to write your own handlers for Interaction Types like Harvest, Open and Talk.
- A sample handler for Use is provided where a GameItem is given to the player when Using the WorldTarget.

<img src="https://github.com/denariigames/nightblade-addons/blob/master/WorldInteractions/dist/screenshot.png" alt="WorldInteractions" height="350">

## Usage

- reference the **InteractionPlayerCharacterController** prefab in your GameInstance in Init scene.
- add the **InteractionManager** prefab to your Init scene. The InteractionManager is a singleton so only one should be added.
- add your handler scripts to the InteractionManager. The default prefab has the sample Use handler already added.
- any object can be made a **World Interactable Target** by adding the World Interactable Target script. See the Well in Demo for example and place in Map001 to test.
- on your **CanvasGameplay**, add the *UITargetInteractable* prefab and reference in UISceneGameplay.

## Limitations

World Interaction Targets are not networked, by design. This means there is no combat, damage, or sync across clients. There may be times you need these capabilities, for example, a hotly contested resource node where once harvested, is no longer available to other players. You should use core Harvestable GameEntities for this use case.

Handlers can and should use proper client-server logic, for example, sanitizing input and making Server RPC calls in BasePlayerCharacterEntity. See the Use handler for an example.

The controller and Target UI is still work in progress and will be improved in future releases. This package addon has only been tested with PlayerCharacterController, and needs to integrate with NearbyEntityDetector for other CharacterController support.