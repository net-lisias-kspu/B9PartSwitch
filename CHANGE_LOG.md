# B9 Part Switch :: Change Log

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
	
