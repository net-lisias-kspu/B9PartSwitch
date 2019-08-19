`ModuleB9PartSwitch` is the PartModule at the heart of B9PartSwitch.  Each `ModuleB9PartSwitch` has multiple subtypes which can be switched between. An unlimited number of `ModuleB9PartSwitch` modules can exist on the same part, with some restrictions.

## Fields

* **`moduleID`** - A unique identifier for this module, which may be blank if there is only one `ModuleB9PartSwitch` on the part. Similar to `engineID` on engine modules. I recommend that it not contain spaces so that it can accessed easily with ModuleManager.
* **`baseVolume`** - The volume of tanks in this module, in KSP volume units (i.e. units of `LiquidFuel`). Subtypes may modify this volume
* **`switcherDescription`** - A description of the switcher which is used in the part's right click menu in the editor. Default: "Subtype". Other examples: "Tank", "Top Nodes".  Should generally be kept short and descriptive as a single control contains this, the subtype title, and the controls to switch subtypes.
* **`switcherDescriptionPlural`** - Describes the switcher/subtypes in the part catalog. Default: "Subtypes", which would become a description of "3 Subtypes" (or whatever number).
* **`affectDragCubes`** - Whether the part's drag cubes should be re-calculated when switching the subtype. Defaults to `true`, however, drag cubes will never be re-calculated if no transforms/models are switched on subtypes. Should be set to `false` if transforms/models are switched but they do not differ significantly in shape.
* **`affectFARVoxels`** - If FerramAerospaceResearch is installed, this affects whether vessel re-voxelization should be triggered when switching the subtype. Defaults to `true`, however, re-voxelization will never be triggered if no transforms/models are switched on subtypes. Should be set to `false` if transforms/models are switched but they do not differ significantly in shape.
* **`parentID`** - `moduleID` of another `ModuleB9PartSwitch` - subtypes of this module can add volume to the "parent" module via `volumeAddedToParent`
* **`switchInFlight`** - whether you can switch the subtype in flight (default false).  **NOTE** - there are no checks to ensure that you aren't adding of subtracting mass out of nowhere with this.  Please use carefully (pay attention to which tank types you use too)
* **`advancedTweakablesOnly`** - whether this switcher is only available if you have advanced tweakables on (default false).  Note that this only disables the UI.

## Subtypes

Each node named `SUBTYPE` defines a different subtype. Subtypes have the following fields:

* **`name`** - Unique name for the subtype. Shouldn't contain any spaces for easy ModuleManager access
* **`title`** - Human-readable name for the subtype. Will be filled from `name` if blank.
* **`primaryColor`** - Color to use on the left side of the switching UI button.  If not specified, it will use the tank type's primary color.  If that isn't specified (or determined based on resources), it will be white.  See [Valid Color Formats](Valid-Color-Formats) for a list of ways to specify a color.
* **`secondaryColor`** - Color to use on the right side of the switching UI button.  If not specified, it will use the tank type's secondary color.  If that isn't specified (or determined based on resources), it will use the subtype's primary color (including tank/resource color).  If that isn't specified, it will be gray.  See [Valid Color Formats](Valid-Color-Formats) for a list of ways to specify a color.
* **`descriptionSummary`** - text about the subtype that appears in the tooltip before automatically generated info about resources, mass, cost, etc.  Keep it brief.
* **`descriptionDetail`** - text about the subtype that appears in the tooltip after automatically generated info.  Go wild with detailed information!
* **`upgradeRequired`** - name of a `PARTUPGRADE` that is required to unlock the subtype
* **`defaultSubtypePriority`** - number that determines what the default subtype is based on what's unlocked.  The unlocked subtype with the highest priority will be the default.  The default value for this is zero.  Any number, positive or negative, decimal or whole, is accepted.
* **`addedMass`** - Mass that is added to the part by this subtype (in addition to tank and resource mass).
* **`addedCost`** - Cost that is added to the part by this subtype (in addition to tank and resource cost).
* **`volumeAdded`** - Tank volume added by this subtype (added on top of the module's `baseVolume`)
* **`volumeMultiplier`** - Multiplier to apply to the module's `baseVolume` for this subtype. This probably shouldn't be used.
* **`volumeAddedToParent`** - If this module has a `parentID`, this subtype will add this much volume to the parent module specified by it
* **`percentFilled`** - If specified (and not overridden on an individual resource), this specifies the percentage that tanks on this subtype should be filled (note - this overrides `percentFilled` on the tank type)
* **`resourcesTweakable`** - If specified, this controls whether the resource amounts on this subtype are tweakable (note - this overrides `resourcesTweakable` on the tank type)
* **`tankType`** - Name of the tank type that this subtype should use. Tank types are defined by global `B9_TANK_TYPE` nodes. A description of how to define tank types can be found on the [[Tank Definitions|Tank-Definitions]] page.
* **`transform`** - Name of Unity transform(s) which should be enabled on this subtype (it will be disabled on all others unless they also have it).  Multiple are allowed, so you can have `transform = a` and `transform = b` on separate lines within the same subtype. If multiple transforms have the same name they will all be included.
* **`node`** - Attach node id for stack nodes that should be enabled. **NOTE** - KSP strips out the node_stack part when creating the node id, so `node_stack_top01` will have a node id of `top01`. More than one can be defined, so you can have e.g. `node = bottom01` and `node = top01` on the same subtype.
* **`maxTemp`** - Temperature (in kelvins) to set the part's `maxTemp` to with this subtype. Other subtypes will use the part prefab's `maxTemp`
* **`skinMaxTemp`** - Temperature (in kelvins) to set the part's `skinMaxTemp` to with this subtype. Other subtypes will use the part prefab's `skinMaxTemp`
* **`attachNode`** - If set, will change the part's surface attachment node to this. Only works if the part is already surface attachable. Subtypes that don't have this defined will use the prefab's.  Follows the usual node format of position x, y, z, normal x, y, z (the final size parameter is not used)
* **`crashTolerance`** - Maximum speed (in m/s) at which the part can survive a crash.  Subtypes which do not have this set will use the part prefab's value.  **NOTE** - There is apparently a bug in KSP where only one collider on a part will be considered for crashes.  There is nothing I can do about that.
* **`CoMOffset`** - Part's center of mass offset.  Subtypes that don't specify one will use the part's default `CoMOffset`
* **`CoPOffset`** - Part's center of pressure (aerodynamic force) offset.  Subtypes that don't specify one will use the part's default `CoPOffset`
* **`CoLOffset`** - Part's center of lift offset.  Subtypes that don't specify one will use the part's default `CoLOffset`
* **`CenterOfBuoyancy`** - Part's center of buoyancy.  Subtypes that don't specify one will use the part's default `CenterOfBuoyancy`
* **`CenterOfDisplacement`** - Part's center of displacement.  Subtypes that don't specify one will use the part's default `CenterOfDisplacement`
* **`allowSwitchInFlight`** - Whether this subtype can be changed to in flight (default true).  If false, it will be hidden from the available subtype options in flight.  Only matters if the module has `switchInFlight = true`
* **`stackSymmetry`** - The symmetry number to use on this part (affects how parts can be attached to symmetric stack nodes). Any subtypes that don't specify this will use the part's default `stackSymmetry`

Subtypes can also define the following nodes:

* **`TEXTURE`** - texture switching - this specifies a texture to replace on the part's model
  * `TEXTURE` nodes take the following fields:
    * `texture` (required) - path to the texture you want to use, e.g. `MyMod/Parts/SomePart/texture`
    * `currentTexture` (optional) - name of the current texture (just the filename excluding the extension, not the full path).  Anything that does not have this as the current texture will be ignored.
    * `isNormalMap` (optional, default false) - whether the texture is a normal map or not (necessary due to KSP treating normal maps differently when they are loaded)
    * `shaderProperty` (optional) - name of the shader property that the texture sits on.  Default is `_MainTex` if `isNormalMap = false` or `_BumpMap` if `isNormalMap = true`.  For an emissive texture you would want `_Emissive`
    * `transform` (optional, can appear more than once) - names of transforms to apply the texture switch to
    * `baseTransform` (optional, can appear more than once) - names of transforms where the texture switch should be applied to them and all of their children
  * If no `transform` or `baseTransform` is specified, it will look for textures to switch on the entire part
* **`RESOURCE`** - Resources can be specified here, or if the resource already exists on the tank type, any fields specified here will override those on the tank type.  The allowed fields can be found on the [[Tank-Definitions]] page
* **`NODE`** - Allows attach nodes on the part to be moved.  If a node is moved on some subtypes but not others, the subtypes that don't specify a position will use the attach node's default position.
  * `name` - the id of the attach node.  If the attach node is `node_stack_top01` you would have `name = top01`
  * `position` - the new position of the attach node, specified as x, y, z coordinates.
* **`TRANSFORM`** - Allows transforms to be modified
  * `name` - the name of the transform to be modified
  * `positionOffset` - x, y, z vector to offset the transform's local position by.  Any number of modules can modify this on the same transform.
  * `rotationOffset` - x, y, z rotation vector (in degrees) to offset the transform's local rotation by.  Only one module can modify this on a particular transform.
* **`MODULE`** - Allows other modules to be modified
  * Please note that this feature is highly experimental, please talk to me before trying to use it
  * **`IDENTIFIER`** - node that contains data used to identify the module being modified
    * **`name`** - value is required, this is the subtype name
    * Any other values can be used to identify the module, e.g. `engineID`, `experimentID`
  * **`DATA`** - node containing data to be loaded into the target module
  * **`moduleActive`** - if set to `false`, disable this module when this particular subtype is active

## Multiple Modules on the same Part

An unlimited number of `ModuleB9PartSwitch` modules can exist on the same part with the following restrictions:

* Each must have a unique `moduleID`
* If they manage the same transform, it will only enabled if all managing subtypes agree that it should be enabled. A module "manages" a transform if any subtype uses it.
* They cannot manage the same resources. A module "manages" a resource if any subtype has a tank type which uses it.
* If they manage the same stack node, it will only be enabled if all managing subtypes agree that it should be enabled. A module "manages" a stack node if any subtype uses it.
* Only one module can manage the part's `maxTemp`
* Only one module can manage the part's `skinMaxTemp`
* Only one module can manage the part's surface attachment node
* Only one module can manage the part's `crashTolerance`
* Only one module can manage the part's `CoMOffset`
* Only one module can manage the part's `CoPOffset`
* Only one module can manage the part's `CoLOffset`
* Only one module can manage the part's `CenterOfBuoyancy`
* Only one module can manage the part's `CenterOfDisplacement`
* Only one module can manage the part's `stackSymmetry`

## Drag cubes

* `ModuleB9PartSwitch` will re-render drag cubes as necessary if transforms are switched and `affectDragCubes` is true (it is by default).  If transforms are switched but do not change the shape significantly, it is recommended that you set `affectDragCubes = false` and `affectFARVoxels = false` to avoid any performance impact from re-rendering drag cubes. The drag cube re-rendering is compatible with one other module using `IMultipleDragCubes` (for instance, `ModuleJettison` or `ModuleAnimateGeneric`) - if this is the case, all of that module's drag cubes will be re-rendered when switching.

## Switching in flight

* If the module has `switchInFlight = true`, the option will be made available to change the subtype in flight. If any resources are present that would be removed, a confirmation is presented to the user before switching. After switching, all managed resources will be empty.

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