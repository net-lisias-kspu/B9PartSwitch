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
    public class PartAttachNodeModifier : PartModifierBase, IPartAspectLock
    {
        public const string PART_ASPECT_LOCK = "attachNode";

        private readonly AttachNode partAttachNode;
        private readonly AttachNode referenceAttachNode;
        private readonly AttachNode newAttachNode;
        private readonly ILinearScaleProvider linearScaleProvider;

        public object PartAspectLock => PART_ASPECT_LOCK;
        public override string Description => "a part's surface attach node";

        public PartAttachNodeModifier(AttachNode partAttachNode, AttachNode referenceAttachNode, AttachNode newAttachNode, ILinearScaleProvider linearScaleProvider)
        {
            partAttachNode.ThrowIfNullArgument(nameof(partAttachNode));
            linearScaleProvider.ThrowIfNullArgument(nameof(linearScaleProvider));

            this.partAttachNode = partAttachNode;
            this.referenceAttachNode = referenceAttachNode;
            this.newAttachNode = newAttachNode;
            this.linearScaleProvider = linearScaleProvider;
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
            partAttachNode.position = newAttachNode.position * linearScaleProvider.LinearScale;
            partAttachNode.orientation = newAttachNode.orientation;
        }

        private void Deactivate()
        {
            partAttachNode.position = referenceAttachNode.position * linearScaleProvider.LinearScale;
            partAttachNode.orientation = referenceAttachNode.orientation;
        }
    }
}
