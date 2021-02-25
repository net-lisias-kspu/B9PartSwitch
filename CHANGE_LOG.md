# B9 Part Switch :: Change Log

* 2019-0930: 2.10.1 (blowfish) for KSP 1.7.3
	+ Add missing Russian localization keys
	+ Fix surface attach node modifiers not actually doing anything
	+ Fix surface attach node modifiers checking whether the part allows surface attach to it rather than to parent part
* 2019-0819: 2.10.0 (blowfish) for KSP 1.7.3
	+ Use funds symbol for cost in tooltips
	+ Fix vessel size including disabled objects
	+ add new `upgradeRequired` field to `SUBTYPE`s
		- References the name of a `PARTUPGRADE` require do unlock the subtype
		- At least one subtype on every switcher must have no tech restriction (i.e. unlocks with the part), otherwise it will complain and remove the restriction from the first subtype
		- All subtypes are unlocked in sandbox regardless of whether upgrades are applied
		- Warning if the upgrade doesn't exist
		- If you attempt to load a craft with a locked subtype you get a warning that it was replaced with the highest priority unlocked subtype
	+ Add `defaultSubtypePriority` to `SUBTYPE`s
		- Number (float) that determines a subtype's priority as the "default" subtype (i.e. the one that is chosen when you freshly add the part).
		- The subtype with the highest priority that is also unlocked will be chosen
		- If two subtypes have the same priority and both are unlocked, it will choose the first
		- The default value is zero.
	+ Add basic implementation of module switching
		- HIGHLY EXPERIMENTAL
		- Subtypes now accept a `MODULE` node
			- inside is an `IDENTIFIER` node which is used to identify the module
			- it must have a `name` which is the same as the module
				- it can have any other fields that are used to identify the module
					- e.g. `engineID` on `ModuleEngines`
				- Identifying the module by nodes is not currently supported
			- It accepts a `DATA` node which provides new data to be loaded into the module
			- It accepts a `moduleActive = false` value which causes the module to be disabled
		- Not everything will work initially, custom handling will have to be added for some modules
		- Some modules are blacklisted for loading new data and disabling.  This list is subject to change.
			- `ModulePartVariants`
			- `ModuleB9PartSwitch`
			- `ModuleB9PartInfo`
			- `ModuleB9DisableTransform`
			- `FSfuelSwitch`
			- `FSmeshSwitch`
			- `FStextureSwitch`
			- `FStextureSwitch2`
			- `InterstellarFuelSwitch`
			- `IntersteallarMeshSwitch`
			- `InterstellarTextureSwitch`
* 2019-0730: 2.9.0 (blowfish) for KSP 1.7.3
	+ Implement new switching UI based on the stock variant switcher
	+ Have subtype switching buttons show some info about the subtype being switched to in a tooltip
		- By default shows resources (including parent), mass, cost, max temperature, max skin temperature, crash tolerance
		- Also shows `descriptionSummary` and `descriptionDetail` from subtype, before and after auto-generated info respectively, if present
	+ 4 new fieds on `SUBTYPE`
		- `descriptionSummary` - any info here will be put in the subtype switching tooltip before the auto-generated info - make it brief
		- `descriptionDetail` - any info here will be put in the subtype switching tooltip after the auto-generated info - go nuts
		- `primaryColor` - color to use in the left part of the switching button
			- if not specified, use the tank type's primaryColor
			- if that's not specified, use white
		- `secondaryColor` - color to use in the right part of the switching button
			- if not specified, use the tank's secondaryColor
			- if that's not specified, use the subtype's primaryColor
			- if that's not specified, use the tank's primaryColor
			- if that's not specified, use gray
	+ 2 new fields on `B9_TANK_TYPE`
		- `primaryColor` - color to use in the left part of the switching button i they subtype does not specify one.  If not specified, common resource combinations will be used.
		- `secondaryColor` - color to use in the right part of the switching button i they subtype does not specify one.  If not specified, common resource combinations will be used.
	+ add default colors for common resources
		- `ResourceColorLiquidFuel`
		- `ResourceColorLqdHydrogen`
		- `ResourceColorLqdMethane`
		- `ResourceColorOxidizer`
		- `ResourceColorMonoPropellant`
		- `ResourceColorXenonGas`
		- `ResourceColorElectricChargePrimary`
		- `ResourceColorElectricChareSecondary`
		- `ResourceColorOre`
	+ Automatically apply resource colors to common resource combinations in tanks (if colors are not specified by the tank or subtype):
		- LiquidFuel
		- LiquidFuel/Oxidizer
		- LqdHydrogen
		- LqdHydrogen/Oxidizer
		- LqdMethane
		- LqdMethane/Oxidizer
		- Oxidizer
		- MonoPropellant
		- XenonGas
		- Ore
		- ElectricCharge
* 2019-0724: 2.8.1 (blowfish) for KSP 1.7.3
	+ Recompile against KSP 1.7.3
* 2019-0609: 2.8.0 (blowfish) for KSP 1.7.1
	+ Recompile against KSP 1.7.1
	+ Fix part action window showing removed resources in KSP 1.7.1
	+ Add Russian localization
* 2019-0417: 2.7.1 (blowfish) for KSP 1.7.0
	+ Fix part into button being shown when there's no info to display
	+ Provide more context for subtype initialization errors in the warning dialog
* 2019-0412: 2.7.0 (blowfish) for KSP 1.7.0
	+ Compile for KSP 1.7.0
	+ Remove `ModuleB9PropagateCopyEvents` from parts since KSP handles this correctly now
		- Leave empty class so that KSP doesn't complain when loading craft/vessels
	+ Subtype initialization errors now warn the user but allow the game to continue
	+ Add fuzzy matching for attach node toggling
		- `?` will match any one character, `*` will match anything (or nothing)
		- All matching nodes will be switched
	+ Allow moving and rotation of transforms
		- Subtypes can now have `TRANSFORM` nodes
			- Each one should nave a `name` which is the name of the transform
			- Each one can have a `positionOffset = x, y, z` which is a local offset for that transform
				- Any number of modules can modify a transform's position (it's additive)
			- Each one can have a `rotationOffset = x, y, z` which is a local rotation offset
				- Only one module can modify a transform's position
	+ Remove KSP localization debug logging
	+ Add Brazilian Portuguese localization
	+ Localize switch subtype button
	+ Fix texture switches incorrectly saying the current texture wasn't found when really the new texture wasn't found
	+ Use more correct part names in some log messages
	+ Allow subtypes to specify a mirror symmetry counterpart
		- Subtypes now accept a `mirrorSymmetrySubtype` value which is the subtype name of the mirror symmetry subtype
		- When placing the part in mirror symmetry, the symmetry counterpart will use this mirror symmetry subtype, otherwise it will use the normal subtype
* 2019-0114: 2.6.0 (blowfish) for KSP 1.6.1
	+ Recompile against KSP 1.6.1
	+ Fix misspellings in fatal error and serious warning handlers
* 2018-1201: 2.5.1 (blowfish) for KSP 1.5.1
	+ Moved stack nodes now respect `scale`, `rescaleFactor`, and TweakScale
	+ Moved surface attach node now respects `scale` and `rescaleFactor`
	+ When only one subtype is present, disable switching GUI and display subtype title as non-interactable string
	+ Downgrade incompatible resource switching module to a warning and disable B9 resource switching in that case
	+ French localization
* 2018-1117: 2.5.0 (blowfish) for KSP 1.5.1
		- Allow moving stack nodes
			- Within a `SUBTYPE`, `NODE` nodes take a `name` (node ID) and a `position` (x, y, z position of the node)
		- Fix log message for duplicated subtype names
		- Fix texture switching behaving weirdly when copying a part in the editor
* 2018-1029: 2.4.5 (blowfish) for KSP 1.5.1
	+ Fix issues with resource switching and stock delta-v simulation code
		- Exception when copying a part in the editor
		- Delta-v simulation was probably off as well
* 2018-1019: 2.4.4 (blowfish) for KSP 1.5.1
	+ Recompile against KSP 1.5.1
	+ Downgrade certain fatal errors to warnings
		- The user will still get an on-screen message but it can be dismissed without closing the game
		- Duplicate subtype names is now only a serious warning
		- Subtype without a name is now only a serious warning
* 2018-1017: 2.4.3 (blowfish) for KSP 1.5
	+ Fix .version file again again
* 2018-1017: 2.4.2 (blowfish) for KSP 1.5
	+ Fix .version file again
* 2018-1016: 2.4.1 (blowfish) for KSP 1.5
	+ Fix .version file still listing KSP 1.4.x
* 2018-1016: 2.4.0 (blowfish) for KSP 1.4.5
	+ Recompile against KSP 1.5
	+ Provide better context for fatal exceptions
	+ A few incompatibilities that previously silently disabled functionality are now fatal errors
	+ Add Spanish translation of built-in strings
	+ Fire `onPartResourceListChange` when changing resources
* 2018-0727: 2.3.3 (blowfish) for KSP 1.4.5
	+ Recompile against KSP 1.4.5
* 2018-0622: 2.3.2 (blowfish) for KSP 1.4.4
	+ Recompile against KSP 1.4.4
* 2018-0521: 2.3.1 (blowfish) for KSP 1.4.3
	+ Fix ModuleJettison shroud disappearing in flight if used with a ModuleB9PartSwitch that affects drag cubes
	+ Don't destroy info module in flight since that messes with module order
* 2018-0501: 2.3.0 (blowfish) for KSP 1.4.3
	+ Recompile against KSP 1.4.3
	+ Remove a couple of hacky workarounds as fixes/improvements were added in KSP 1.4.3
	+ Use resource display names rather than identifiers in module description
	+ Extract all hard-coded UI strings into localization table
* 2018-0411: 2.2.2 (blowfish) for KSP 1.4.2
	+ Fix texture replacements getting locked in when loading a craft in the editor if a part up the hierarchy renders procedural drag cubes
* 2018-0402: 2.2.1 (blowfish) for KSP 1.4.2
	+ Recompile against KSP 1.4.2
	+ Fix transforms incorrectly being disabled in the part icon if subtypes are in a particular order
	+ Fix `transform` in a `TEXTURE` node looking for renderers in child transforms too
* 2018-0318: 2.2.0 (blowfish) for KSP 1.4.1
	+ Recompile for KSP 1.4.1
* 2018-0115: 2.1.1 (blowfish) for KSP 1.3.1
	+ Fix texture replacements being reset when drag cubes are rendered
	+ Fix battery tank type having 100x too much electric charge, bring mass in-line with stock
* 2017-1130: 2.1.0 (blowfish) for KSP 1.3.1
	+ Add texture switching
		- Each subtype can now have `TEXTURE` nodes which take the following fields:
			- `texture` (required) - path to the texture you want to use, e.g. `MyMod/Parts/SomePart/texture`
			- `currentTexture` (optional) - name of the current texture (just the filename excluding the extension, not the full path).  Anything that does not have this as the current texture will be ignored.
			- `isNormalMap` (optional, default false) - whether the texture is a normal map or not (necessary due to KSP treating normal maps differently when they are loaded)
			- `shaderProperty` (optional) - name of the shader property that the texture sits on.  Default is `_MainTex` if `isNormalMap = false` or `_BumpMap` if `isNormalMap = true`.  For an emissive texture you would want `_Emissive`
			- `transform` (optional, can appear more than once) - names of transforms to apply the texture switch to
			- `baseTransform` (optional, can appear more than once) - names of transforms where the texture switch should be applied to them and all of their children
		- If no `transform` or `baseTransform` is specified, it will look for textures to switch on the entire part
* 2017-1116: 2.0.0 (blowfish) for KSP 1.3.1
	+ Only match on exact attach node id
	+ When switching in flight, resources should always start empty
	+ Allow individual subtypes to not allow switching in flight via `allowSwitchInFlight` field
	+ Allow `ModuleB9PartSwitch` to have its GUI hidden if it has `advancedTweakablesOnly = true` and advanced tweakables are disabled
	+ Better error handling if resource of tank type does not exist (show error dialog in game and force the user to quit)
	+ Fix .version file not being able to be parsed by KSP-AVC
	+ Move remote .avc file from bintray to s3
	+ Add back assembly guid (accidentally removed a while ago)
* 2017-1008: 1.10.0 (blowfish) for KSP 1.3.1
	+ Add new GUI that allows selecting subtype from a list
	+ Allow switching in flight via switchInFlight parameter (uses new GUI)
* 2017-0702: 1.9.0 (blowfish) for KSP 1.3
	+ Add stackSymmetry part field to subtypes
* 2017-0620: 1.8.1 (blowfish) for KSP 1.3
	+ Changes
			- Fix drag cubes being overwritten with defaults on root part in flight scene
			- Fix vessel disappearing from map view if root part has a switcher that affects drag cubes
* 2017-0528: 1.8.0 (blowfish) for KSP 1.3
	+ Changes
			- Recompile for KSP 1.3
			- Drag cube re-rendering now supports IMultipleDragCubes
* 2017-0206: 1.7.1 (blowfish) for KSP 1.2.2
	+ Changes
		- Fix an occasional NRE when building part info
* 2017-0202: 1.7.0 (blowfish) for KSP 1.2.2
	+ Changes
		- Allow "child" part switch modules to modify volume of "parent" module
		- Allow multiple modules to manage the same transform or node, only enable it if they all agree
* 2017-0127: 1.6.1 (blowfish) for KSP 1.2.2
	+ Changes
		- Switch percentFilled priority to resource -> subtype -> tank type -> 100% since resources can be overridden on individual subtypes now
* 2017-0125: 1.6.0 (blowfish) for KSP 1.2.2
	+ Changes
		- Allow tanks to be partially filled - percentFilled can be defined on the subtype, resource, or tank type (in decreasing order of priority), defaulting to completely full
		- Allow toggling resource tweakability in the editor - resourcesTweakable can be defined on the subtype or tank type (subtype takes priority), default is whatever the standard is for that resource
		- Allow RESOURCE nodes directly on the subtype
			- If the resource already exists on the tank, values defined here will override what is already on the tank (won't affect other subtypes using the same tank)
			- If it isn't already on the tank, it will be added (won't affect other subtypes using the same tank)
		- Add ModuleB9DisableTransform to remove unused transforms on models
		- Major internal changes
* 2016-1209: 1.5.3 (blowfish) for KSP 1.2.2
	+ Changes
		- Recompile against KSP 1.2.2
		- Remove useless warnings in the log
		- A few internal changes
* 2016-1123: 1.5.2 (blowfish) for KSP 1.2.1
	+ Changes
		- Recompile against KSP 1.2.1
* 2016-1020: 1.5.1 (blowfish) for KSP 1.2
	+ Changes
		- Fix resource amounts displaying incorrectly in part tooltip
		- Reformat module title in part list tooltip a bit
		- Hopefully reduce GC some more
* 2016-1015: 1.5.0 (blowfish) for KSP 1.2
	+ Changes
		- Update for KSP 1.2
		- Add CoMOffset, CoPOffset, CoLOffset, CenterOfBuoyancy, CenterOfDisplacement to editable part fields
		- Hopefully reduce GC allocation a little bit
* 2016-0623: 1.4.3 (blowfish) for KSP 1.1.3
	+ Changes
		- Recompile against KSP 1.1.3
		- Remove some code which is unnecessary in KSP 1.1.3
* 2016-0618: 1.4.2 (blowfish) for KSP 1.1.2
	+ Changes
		- Fix TweakScale interaction - resource amounts did not account for scaling (broken since v1.4.0)
* 2016-0612: 1.4.1 (blowfish) for KSP 1.1.2
	+ Changes
		- Fix bug where we were setting maxTemp when we should have been setting skinMaxTemp or crashTolerance
* 2016-0611: 1.4.0 (blowfish) for KSP 1.1.2
	+ Changes
		- Find best subtype intelligently
			- If subtype name was previously set, use it to find the correct subtype (allows subtypes to be reordered without breaking craft)
			- If name was not previously set or not found, but index was, use it (this allows transitioning from current setup and renaming subtypes if necessary)
			- If index was not previously set, try to infer subtype based on part's resources (this allows easy transitioning from a non-switching setup)
			- Finally, just use first subtype
		- Add unit testing for subtype finding
		- Get rid of some unnecessary logging in debug mode
		- Refactor part switching a bit
* 2016-0607: 1.3.1 (blowfish) for KSP 1.1.2
	+ Changes
		- Fix bug where having ModuleB9PartInfo on a root part would cause physics to break due to an exception (really a stock issue but no sense waiting for a fix)
* 2016-0526: 1.3.0 (blowfish) for KSP 1.1.2
	+ Changes
		- Do not destroy incompatible fuel switchers.  Instead, disable fuel switching
		- Allow part's crash tolerance to be edited
		- Add info module to display changes to part in the info window.  Only displays things that can be changed.
		- Various internal changes
* 2016-0520: 1.2.0 (blowfish) for KSP 1.1.2
	+ Changes
		- Support TweakScale integration
		- Allow plural switcher description (in part catalog) to be edited)
		- Disable changing surface attach node size (problematic with Tweakscale)
* 2016-0506: 1.1.4 (blowfish) for KSP 1.1.2
	+ Changes
		- Don't remove FSfuelSwitch or InterstellarFuelSwitch if ModuleB9PartSwitch doesn't manage resources
		- Defer rendering drag cubes until part has been attached (fixes flickering in editor)
		- Avoid firing events multiple times when symmetric parts present
		- Various internal changes
* 2016-0503: 1.1.3 (blowfish) for KSP 1.1.2
	+ Changes
		- Recompile against KSP 1.1.2
		- Simplify part list info a bit
		- Hopefully make some error messages clearer
		- Various internal refactors and simplifications
* 2016-0429: 1.1.2 (blowfish) for KSP 1.1.1
	+ Changes
		- Removed `FSmeshSwitch` and `InterstellarMeshSwitch` from incompatible modules.  Fuels switchers remain incompatible.
		- Recompiled against KSP 1.1.1
* 2016-0428: 1.1.1 (blowfish) for KSP 1.1
	+ Changes
		- Fix resource cost not accounting for `unitsPerVolume` on the tank
* 2016-0424: 1.1 (blowfish) for KSP 1.1
	+ Changes
		- KSP 1.1 compatibility
		- Fixed bug where having part switching on the root part would cause physics to break
		- Moved UI controls to UI_ChooseOption
		- Adjust default Monopropellant tank type to be closer to (new) stock values
		- Use stock part mass modification
		- Hopefully fix incompatible module checking
		- Various refactors and simplifications which might improve performance a bit
	+ Known Issues
		- Still no TweakScale compatibility
* 2016-0215: 1.0.1 (blowfish) for KSP 1.0.5
	+ Changes
		- Fix NRE in flight scene
* 2016-0215: 1.0 (blowfish) for KSP 1.0.5
	+ Initial Release
	
