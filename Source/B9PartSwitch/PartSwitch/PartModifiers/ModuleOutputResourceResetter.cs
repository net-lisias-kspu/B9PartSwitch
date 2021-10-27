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
    public class ModuleOutputResourceResetter : PartModifierBase
    {
        private readonly PartModule module;

        public ModuleOutputResourceResetter(PartModule module)
        {
            module.ThrowIfNullArgument(nameof(module));

            this.module = module;
        }

        public override string Description => $"output resources on {module}";

        public override void ActivateOnStartEditor() => Fire();
        public override void ActivateOnStartFlight() => Fire();
        public override void DeactivateOnSwitchEditor() => Fire();
        public override void DeactivateOnSwitchFlight() => Fire();
        public override void ActivateOnSwitchEditor() => Fire();
        public override void ActivateOnSwitchFlight() => Fire();
        public override void OnWillBeCopiedActiveSubtype() => Fire();
        public override void OnWasCopiedActiveSubtype() => Fire();

        private void Fire() => module.resHandler.outputResources.Clear();
    }
}
