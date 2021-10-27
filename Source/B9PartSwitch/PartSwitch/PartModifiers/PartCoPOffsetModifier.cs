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
using UnityEngine;

namespace B9PartSwitch.PartSwitch.PartModifiers
{
    public class PartCoPOffsetModifier : PartModifierBase, IPartAspectLock
    {
        public const string PART_ASPECT_LOCK = "CoPOffset";

        private readonly Part part;
        private readonly Vector3 origCoPOffset;
        private readonly Vector3 newCoPOffset;

        public object PartAspectLock => PART_ASPECT_LOCK;
        public override string Description => "a part's CoPOffset";

        public PartCoPOffsetModifier(Part part, Vector3 origCoPOffset, Vector3 newCoPOffset)
        {
            part.ThrowIfNullArgument(nameof(part));

            this.part = part;
            this.origCoPOffset = origCoPOffset;
            this.newCoPOffset = newCoPOffset;
        }

        public override void ActivateOnStartEditor() => Activate();
        public override void ActivateOnStartFlight() => Activate();
        public override void DeactivateOnStartEditor() => Deactivate();
        public override void DeactivateOnStartFlight() => Deactivate();
        public override void ActivateOnSwitchEditor() => Activate();
        public override void ActivateOnSwitchFlight() => Activate();
        public override void DeactivateOnSwitchEditor() => Deactivate();
        public override void DeactivateOnSwitchFlight() => Deactivate();

        private void Activate()
        {
            part.CoPOffset = newCoPOffset;
        }

        private void Deactivate()
        {
            part.CoPOffset = origCoPOffset;
        }
    }
}
