`ModuleB9PartSwitch` is the PartModule at the heart of B9PartSwitch.  Each `ModuleB9PartSwitch` has multiple subtypes which can be switched between. An unlimited number of `ModuleB9PartSwitch` modules can exist on the same part, with some restrictions.

#### Fields

* **`moduleID`** - A unique identifier for this module, which may be blank if there is only one `ModuleB9ParSwitch` on the part. Similar to `engineID` on engine modules. I recommend that it not contain spaces so that it can accessed easily with ModuleManager.
* **`baseVolume`** - The volume of tanks in this module, in KSP volume units (i.e. units of `LiquidFuel`). Subtypes may modify this volume
* **`switcherDescription`** - A description of the switcher which is used in the part's right click menu in the editor. Default: "Subtype". Other examples: "Tank", "Top Nodes".  Should generally be kept short and descriptive as a single control contains this, the subtype title, and the controls to switch subtypes.
* **`switcherDescriptionPlural`** - **_coming soon_** - Describes the switcher/subtypes in the part catalog. Default: "Subtypes", which would become a description of "3 Subtypes" (or whatever number).
* **`affectDragCubes`** - Whether the part's drag cubes should be re-calculated when switching the subtype. Defaults to `true`, however, drag cubes will never be re-calculated if no transforms/models are switched on subtypes. Should be set to `false` if transforms/models are switched but they do not differ significantly in shape.
* **`affectFARVoxels`** - If FerramAerospaceResearch is installed, this affects whether vessel re-voxelization should be triggered when switching the subtype. Defaults to `true`, however, re-voxelization will never be triggered if no transforms/models are switched on subtypes. Should be set to `false` if transforms/models are switched but they do not differ significantly in shape.

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
			name = 
			title = HL (LF)
			tankType = LiquidFuel
			transform = model_lf
		}
	}
```