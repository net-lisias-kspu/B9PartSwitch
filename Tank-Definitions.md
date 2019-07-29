The tank definitions define common fuel tanks for subtypes of [[ModuleB9PartSwitch]] to use. Tank definitions define the resources that the tank contains and additional mass/cost that the tank should have

### Defining Tank Types

Tank types are defined by nodes called `B9_TANK_TYPE` at the root level of a config

### Fields

* **`name`** - Name of this tank type (will be used to reference it from subtypes). Make it something descriptive and possible specific to your mod. Duplicates won't be loaded. Should not contain any spaces for easy ModuleManager access
* **`primaryColor`** - Color to use on the left side of the switching UI button. If not specified, it will attempt to pick a color based on common resource combinations.  See [Valid Color Formats](Valid-Color-Formats) for a list of ways to specify a color.
* **`secondaryColor`** - Color to use on the right side of the switching UI button. If not specified, it will attempt to pick a color based on common resource combinations.  See [Valid Color Formats](Valid-Color-Formats) for a list of ways to specify a color.
* **`tankMass`** - Mass that this tank has, per unit of volume
* **`tankCost`** - Cost that this tank has, per unit of volume **NOTE**: B9PartSwitch adds the cost of any resources, so there is no need to add the resource cost here as in stock tanks
* **`percentFilled`** - The percentage that this tank type's resources should be filled (default 100%). Can be overridden on the subtype or on the individual resources
* **`resourcesTweakable`** - Whether this tank type's resources should be tweakable (default is whatever the KSP resource definition specifies). Can be overridden on the subtype.
* **`RESOURCE`** - Each node defines a resource that this tank will hold
  * **`name`** - The name of the resource that this tank should hold (looked up in the game's resource library)
  * **`unitsPerVolume`** - How many units of this resource should each unit of tank volume hold?
  * **`percentFilled`** - The percentage that this resource should be filled. Overrides `percentFilled` on the tank type or subtype

### Default Tank Types

B9PartSwitch comes with several default tank types for common stock fuel configurations. They can be found [here](https://github.com/blowfishpro/B9PartSwitch/blob/master/GameData/B9PartSwitch/DefaultTankTypes.cfg). **NOTE** - The masses and costs of these tank types assume that the "structural" mass and cost have already been added to the base part. You are welcome to use your own tank definitions if you don't wish to use this paradigm

There is also a hard-coded tank type called "Structural" which has zero mass and cost and no resources. It is the default tank type for any subtypes which do not have another one defined.

I highly recommend that you create your own tank types. This will ensure consistency across your mod's parts.

### Examples

The best source of examples is [the default tank types](https://github.com/blowfishpro/B9PartSwitch/blob/master/GameData/B9PartSwitch/DefaultTankTypes.cfg)