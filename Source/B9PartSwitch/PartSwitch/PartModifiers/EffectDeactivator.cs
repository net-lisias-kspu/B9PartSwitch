/*
	This file is part of B9PartSwitch /L Unleashed
		© 2021-2022 LisiasT : http://lisias.net <support@lisias.net>
		© 2016-2021 blowfish
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
    public class EffectDeactivator : PartModifierBase
    {
        private readonly Part part;
        private readonly string originalEffectName;
        private readonly string newEffectName;

        public EffectDeactivator(Part part, string originalEffectName, string newEffectName)
        {
            part.ThrowIfNullArgument(nameof(part));

            this.part = part;
            this.originalEffectName = originalEffectName;
            this.newEffectName = newEffectName;
        }

        public override string Description => $"switch between effects {originalEffectName} and {newEffectName}";

        public override void ActivateOnStartFinishedFlight() => Activate();
        public override void DeactivateOnStartFinishedFlight() => Deactivate();
        public override void DeactivateOnSwitchFlight() => Deactivate();
        public override void ActivateOnSwitchFlight() => Activate();

        private void Activate()
        {
            part.Effect(originalEffectName, effectPower: 0);
        }

        private void Deactivate()
        {
            part.Effect(newEffectName, effectPower: 0);
        }
    }
}
