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
    public class TransformRotator : PartModifierBase, IPartAspectLock
    {
        private readonly Transform transform;
        private readonly Quaternion rotationOffset;
        private bool isActive = false;

        public TransformRotator(Transform transform, Quaternion rotationOffset)
        {
            transform.ThrowIfNullArgument(nameof(transform));

            this.transform = transform;
            this.rotationOffset = rotationOffset;
        }

        public override string Description => $"transform '{transform.name}' rotation offset";
        public object PartAspectLock => transform.GetInstanceID() + "---rotation";
        public override bool ChangesGeometry => true;

        public override void ActivateOnStartEditor() => Activate();
        public override void ActivateOnStartFlight() => Activate();
        public override void ActivateOnSwitchEditor() => Activate();
        public override void ActivateOnSwitchFlight() => Activate();
        public override void DeactivateOnSwitchEditor() => Deactivate();
        public override void DeactivateOnSwitchFlight() => Deactivate();
        public override void OnIconCreateActiveSubtype() => Activate();
        public override void OnWillBeCopiedActiveSubtype() => Deactivate();
        public override void OnWasCopiedActiveSubtype() => Activate();
        public override void OnBeforeReinitializeActiveSubtype() => Deactivate();
        public override void OnAfterReinitializeActiveSubtype() => Activate();

        private void Activate()
        {
            if (isActive) return;

            transform.localRotation *= rotationOffset;
            isActive = true;
        }

        private void Deactivate()
        {
            if (!isActive) return;

            transform.localRotation *= Quaternion.Inverse(rotationOffset);
            isActive = false;
        }
    }
}
