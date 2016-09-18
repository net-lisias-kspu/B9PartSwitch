The tank definitions define common fuel tanks for subtypes of [[ModuleB9PartSwitch]] to use. Tank definitions define the resources that the tank contains and additional mass/cost that the tank should have

### Defining Tank Types

Tank types are defined by nodes called `B9_TANK_TYPE` at the root level of a config

### Fields

* **`name`** - Name of this tank type (will be used to reference it from subtypes). Make it something descriptive and possible specific to your mod. Duplicates won't be loaded. Should not contain any spaces for easy ModuleManager access
* **`tankMass`** - Mass that this tank has, per unit of volume
* **`tankCost`** - Cost that this tank has, per unit of volume **NOTE**: B9PartSwitch adds the cost of any resources, so there is no need to add the resource cost here as in stock tanks
* **`RESOURCE`** - Each node defines a resource that this tank will hold
  * **`name`** - The name of the resource that this tank should hold (looked up in the game's resource library)
  * **`unitsPerVolume`** - How many units of this resource should each unit of tank volume hold?

### Default Tank Types

B9PartSwitch comes with several default tank types for common stock fuel configurations. They can be found [here](https://github.com/blowfishpro/B9PartSwitch/blob/master/GameData/B9PartSwitch/DefaultTankTypes.cfg). **NOTE** - The masses and costs of these tank types assume that the "structural" mass and cost have already been added to the base part. You are welcome to use your own tank definitions if you don't wish to use this paradigm

There is also a hard-coded tank type called "Structural" which has zero mass and cost and no resources. It is the default tank type for any subtypes which do not have another one defined.

### Examples

The best source of examples is [the default tank types](https://github.com/blowfishpro/B9PartSwitch/blob/master/GameData/B9PartSwitch/DefaultTankTypes.cfg)