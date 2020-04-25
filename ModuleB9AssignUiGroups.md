Module for assigning PAW groups on other modules (requires KSP 1.8 or newer).

* Takes one or more `MODULE` nodes that each identify a module to have its UI group assigned
* each one must have an `IDENTIFIER` node to identify the module
* it must have a name which is the name of the module (wildcards and regex are allowed)
* it can have any other fields that uniquely identify the module, this is the same as the `IDENTIFIER` in a module switcher
* **`uiGroupName`** is the unique identifier of the group
* **`uiGroupDisplayName`** is the human readable name of the group to show in the UI
* only applies to fields/events that don't already have a group
* Cannot apply to `ModuleB9PartSwitch`, `ModuleB9PartInfo`, `ModuleB9AssignUiGroups` (itself), or `ModuleSimpleAdjustableFairing`
