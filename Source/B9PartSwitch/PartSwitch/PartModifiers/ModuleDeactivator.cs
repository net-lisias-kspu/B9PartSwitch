/*
	This file is part of B9PartSwitch /L Unleashed
		© 2021 Lisias T : http://lisias.net <support@lisias.net>
		© 216-2021 blowfish
		© 2015 bac9

	B9PartSwitch /L Unofficial is licensed as follows:

		* LGPL 3.0 : https://www.gnu.org/licenses/lgpl-3.0.txt

	B9PartSwitch /L Unleashed is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

	You should have received a copy of the GNU Lesser General Public License 3.0
	along with B9PartSwitch /L Unofficial. If not, see <https://www.gnu.org/licenses/>.

*/
using System;

namespace B9PartSwitch.PartSwitch.PartModifiers
{
    public class ModuleDeactivator : PartModifierBase
    {
        public readonly PartModule module;
        protected readonly PartModule parent;

        public ModuleDeactivator(PartModule module, PartModule parent)
        {
            module.ThrowIfNullArgument(nameof(module));
            parent.ThrowIfNullArgument(nameof(parent));

            this.module = module;
            this.parent = parent;
        }

        public override string Description => $"module {module} activated status";

        public override void ActivateOnStartEditor() => Activate();
        public override void ActivateOnStartFlight() => Activate();
        public override void DeactivateOnSwitchEditor() => MaybeDeactivate();
        public override void DeactivateOnSwitchFlight() => MaybeDeactivate();
        public override void ActivateOnSwitchEditor() => Activate();
        public override void ActivateOnSwitchFlight() => Activate();
        public override void OnWillBeCopiedActiveSubtype() => Deactivate();
        public override void OnWasCopiedActiveSubtype() => Activate();

        protected virtual void Activate()
        {
            module.enabled = false;
            module.isEnabled = false;
            Log.detail("Module {0} was deactivated.", module);
        }

        protected virtual void MaybeDeactivate()
        {
            foreach (PartModule otherModule in module.part.Modules)
            {
                if (otherModule == parent) continue;
                if (!(otherModule is ModuleB9PartSwitch switchModule)) continue;
                if (switchModule.ModuleShouldBeEnabled(module)) return;
            }

            Deactivate();
        }

        protected virtual void Deactivate()
        {
            module.enabled = true;
            module.isEnabled = true;
            Log.detail("Module {0} was activated.", module);
        }
    }
}
