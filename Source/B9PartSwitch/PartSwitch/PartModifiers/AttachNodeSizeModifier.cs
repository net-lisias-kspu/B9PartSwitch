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
    public class AttachNodeSizeModifier : PartModifierBase, IPartAspectLock
    {
        public readonly AttachNode attachNode;
        private readonly int originalSize;
        private readonly int size;
        private readonly ILinearScaleProvider linearScaleProvider;

        public AttachNodeSizeModifier(AttachNode attachNode, int size, ILinearScaleProvider linearScaleProvider)
        {
            attachNode.ThrowIfNullArgument(nameof(attachNode));
            linearScaleProvider.ThrowIfNullArgument(nameof(linearScaleProvider));

            this.attachNode = attachNode;
            this.size = size;
            this.linearScaleProvider = linearScaleProvider;

            originalSize = attachNode.size;
        }

        public object PartAspectLock => attachNode.id + "---size";
        public override string Description => $"attach node '{attachNode.id}' size";

        public override void ActivateOnStartEditor() => SetAttachNodeSize();
        public override void ActivateOnStartFlight() => SetAttachNodeSize();

        // Wait until OnStartFinished thanks to TweakScale
        public override void ActivateOnStartFinishedEditor() => SetAttachNodeSize();
        public override void ActivateOnStartFinishedFlight() => SetAttachNodeSize();

        public override void ActivateOnSwitchEditor() => SetAttachNodeSize();
        public override void ActivateOnSwitchFlight() => SetAttachNodeSize();
        public override void DeactivateOnSwitchEditor() => UnsetAttachNodeSize();
        public override void DeactivateOnSwitchFlight() => UnsetAttachNodeSize();
        public override void OnBeforeReinitializeActiveSubtype() => UnsetAttachNodeSize();
        public override void OnAfterReinitializeActiveSubtype() => SetAttachNodeSize();

        private void SetAttachNodeSize() => attachNode.size = Mathf.RoundToInt(size * linearScaleProvider.LinearScale);
        private void UnsetAttachNodeSize() => attachNode.size = Mathf.RoundToInt(originalSize * linearScaleProvider.LinearScale);
    }
}
