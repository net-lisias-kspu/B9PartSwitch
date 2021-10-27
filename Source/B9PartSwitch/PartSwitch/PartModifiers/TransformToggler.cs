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
    public class TransformToggler : PartModifierBase
    {
        private readonly Transform transform;
        private readonly Part part;

        public TransformToggler(Transform transform, Part part)
        {
            transform.ThrowIfNullArgument(nameof(transform));
            part.ThrowIfNullArgument(nameof(part));

            this.transform = transform;
            this.part = part;
        }

        public override string Description => $"Transform {transform.name} enabled state";
        public override bool ChangesGeometry => true;

        public override void DeactivateOnStartEditor() => Deactivate();
        public override void DeactivateOnStartFlight() => Deactivate();
        public override void ActivateOnStartEditor() => Activate();
        public override void ActivateOnStartFlight() => Activate();
        public override void DeactivateOnSwitchEditor() => Deactivate();
        public override void DeactivateOnSwitchFlight() => Deactivate();
        public override void ActivateOnSwitchEditor() => Activate();
        public override void ActivateOnSwitchFlight() => Activate();
        public override void OnIconCreateInactiveSubtype() => Deactivate();
        public override void OnIconCreateActiveSubtype() => Activate();
        public override void OnAfterReinitializeActiveSubtype() => Activate();
        public override void OnAfterReinitializeInactiveSubtype() => Deactivate();

        private void Activate()
        {
            part.UpdateTransformEnabled(transform);
        }

        private void Deactivate()
        {
            transform.Disable();

            if (part.partRendererBoundsIgnore.Contains(transform.name)) part.partRendererBoundsIgnore.Add(transform.name);
        }
    }
}
