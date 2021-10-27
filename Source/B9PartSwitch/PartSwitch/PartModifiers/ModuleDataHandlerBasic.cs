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
    public class ModuleDataHandlerBasic : PartModifierBase
    {
        protected readonly PartModule module;
        protected readonly ConfigNode originalNode;
        protected readonly ConfigNode dataNode;

        private readonly BaseEventDetails moduleDataChangedEventDetails;

        public ModuleDataHandlerBasic(PartModule module, ConfigNode originalNode, ConfigNode dataNode, BaseEventDetails moduleDataChangedEventDetails)
        {
            module.ThrowIfNullArgument(nameof(module));
            originalNode.ThrowIfNullArgument(nameof(originalNode));
            dataNode.ThrowIfNullArgument(nameof(dataNode));
            moduleDataChangedEventDetails.ThrowIfNullArgument(nameof(moduleDataChangedEventDetails));

            this.module = module;
            this.originalNode = originalNode;
            this.dataNode = dataNode;
            this.moduleDataChangedEventDetails = moduleDataChangedEventDetails;
        }

        public override string Description => $"data on module {module}";

        public override void ActivateOnStartEditor() => Activate();
        public override void ActivateOnStartFlight() => Activate();
        public override void DeactivateOnSwitchEditor() => Deactivate();
        public override void DeactivateOnSwitchFlight() => Deactivate();
        public override void ActivateOnSwitchEditor() => Activate();
        public override void ActivateOnSwitchFlight() => Activate();
        public override void OnWillBeCopiedActiveSubtype() => Deactivate();
        public override void OnWasCopiedActiveSubtype() => Activate();

        private void Activate()
        {
            bool isEnabled = module.isEnabled;
            module.Load(dataNode);
            module.isEnabled = isEnabled;
            module.Events.Send("ModuleDataChanged", moduleDataChangedEventDetails);
        }

        private void Deactivate()
        {
            bool isEnabled = module.isEnabled;
            module.Load(originalNode);
            module.isEnabled = isEnabled;
            module.Events.Send("ModuleDataChanged", moduleDataChangedEventDetails);
        }
    }
}
