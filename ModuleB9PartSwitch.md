`ModuleB9PartSwitch` is the PartModule at the heart of B9PartSwitch.  Each `ModuleB9PartSwitch` has multiple subtypes which can be switched between. An unlimited number of `ModuleB9PartSwitch` modules can exist on the same part, with some restrictions.

## Fields

* **`moduleID`** - A unique identifier for this module, which may be blank if there is only one `ModuleB9PartSwitch` on the part. Similar to `engineID` on engine modules. I recommend that it not contain spaces so that it can accessed easily with ModuleManager.
* **`baseVolume`** - The volume of tanks in this module, in KSP volume units (i.e. units of `LiquidFuel`). Subtypes may modify this volume
* **`switcherDescription`** - A description of the switcher which is used in the part's right click menu in the editor. Default: "Subtype". Other examples: "Tank", "Top Nodes".  Should generally be kept short and descriptive as a single control contains this, the subtype title, and the controls to switch subtypes.
* **`switcherDescriptionPlural`** - Describes the switcher/subtypes in the part catalog. Default: "Subtypes", which would become a description of "3 Subtypes" (or whatever number).
* **`affectDragCubes`** - Whether the part's drag cubes should be re-calculated when switching the subtype. Defaults to `true`, however, drag cubes will never be re-calculated if no transforms/models are switched on subtypes. Should be set to `false` if transforms/models are switched but they do not differ significantly in shape.
* **`affectFARVoxels`** - If FerramAerospaceResearch is installed, this affects whether vessel re-voxelization should be triggered when switching the subtype. Defaults to `true`, however, re-voxelization will never be triggered if no transforms/models are switched on subtypes. Should be set to `false` if transforms/models are switched but they do not differ significantly in shape.

## Subtypes

Each node named `SUBTYPE` defines a different subtype. Subtypes have the following fields:

* **`name`** - Unique name for the subtype. Shouldn't contain any spaces for easy ModuleManager access
* **`title`** - Human-readable name for the subtype. Will be filled from `name` if blank.
* **`addedMass`** - Mass that is added to the part by this subtype (in addition to tank and resource mass).
* **`addedCost`** - Cost that is added to the part by this subtype (in addition to tank and resource cost).
* **`volumeAdded`** - Tank volume added by this subtype (added on top of the module's `baseVolume`)
* **`volumeMultiplier`** - Multiplier to apply to the module's `baseVolume` for this subtype. This probably shouldn't be used.
* **`tankType`** - Name of the tank type that this subtype should use. Tank types are defined by global `B9_TANK_TYPE` nodes. A description of how to define tank types can be found on the [[Tank Definitions|Tank-Definitions]] page.
* **`transform`** - Name of Unity transform(s) which should be enabled on this subtype (it will be disabled on all others unless they also have it).  Multiple are allowed, so you can have `transform = a` and `transform = b` on separate lines within the same subtype. If multiple transforms have the same name they will all be included.
* **`node`** - Attach node id for stack nodes that should be enabled.  Important things to note: (1) KSP strips out the node_stack part when creating the node id, so `node_stack_top01` will have a node id of `top01` (2) This is done as a partial text search, so `top` will match `top01` and `top02`. More than one can be defined, so you can have e.g. `node = bottom01` and `node = top01` on the same subtype.
* **`maxTemp`** - Temperature (in kelvins) to set the part's `maxTemp` to with this subtype. Other subtypes will use the part prefab's `maxTemp`
* **`skinMaxTemp`** - Temperature (in kelvins) to set the part's `skinMaxTemp` to with this subtype. Other subtypes will use the part prefab's `skinMaxTemp`
* **`attachNode`** - If set, will change the part's surface attachment node to this. Only works if the part is already surface attachable. Subtypes that don't have this defined will use the prefab's.  Follows the usual node format of position x, y, z, normal x, y, z (the final size parameter is not used)
* **`crashTolerance`** - Maximum speed (in m/s) at which the part can survive a crash.  Subtypes which do not have this set will use the part prefab's value.  **NOTE** - There is apparently a bug in KSP where only one collider on a part will be considered for crashes.  There is nothing I can do about that.

## Multiple Modules on the same Part

An unlimited number of `ModuleB9PartSwitch` modules can exist on the same part with the following restrictions:

* Each must have a unique `moduleID`
* They cannot manage the same transforms/models. A module "manages" a transform if any subtype uses it.
* They cannot manage the same resources. A module "manages" a resource if any subtype has a tank type which uses it.
* They cannot manage the same stack nodes. A module "manages" a stack node if any subtype uses it.
* Only one module can manage the part's `maxTemp`
* Only one module can manage the part's `skinMaxTemp`
* Only one module can manage the part's surface attachment node
* Only one module can manage the part's `crashTolerance`

## Incompatible Modules

Resource switching on ModuleB9PartSwitch will be disabled if any of the following modules are found on the part:

* `FSfuelSwitch`
* `InterstellarFuelSwitch`
* `ModuleFuelTanks`

This is because the way that these modules do resource switching is incompatible with `ModuleB9PartSwitch`.  Note that when any of these modules are found, the tank type for all subtypes is set to the default `Structural` subtype.  This means that no subtype will receive any mass or cost from the tank.

## Examples

A simple module might look something like this:

```
	MODULE
	{
		name = ModuleB9PartSwitch
		moduleID = fuelSwitch
		baseVolume = 1000.0

		SUBTYPE
		{
			name = Structural
			transform = model_str
		}

		SUBTYPE
		{
			name = Structural_Big
			title = Structural (Big)
			transform = model_str
			addedMass = 0.5
			addedCost = 100
		}

		SUBTYPE
		{
			name = LiquidFuel
			title = Fuel Tank
			tankType = LiquidFuel
			transform = model_lf
			transform = model_lf_2
		}
	}
```