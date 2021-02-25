# B9 Part Switch :: Change Log

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
	
