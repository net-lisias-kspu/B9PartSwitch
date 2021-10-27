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
    public class AttachNodeToggler : PartModifierBase
    {
        private readonly AttachNode node;

        public AttachNodeToggler(AttachNode node)
        {
            node.ThrowIfNullArgument(nameof(node));

            this.node = node;
        }

        public override string Description => $"stack node '{node.id}' enabled status";

        public override void DeactivateOnStartEditor() => Deactivate();
        public override void ActivateOnStartEditor() => MaybeActivate();
        public override void DeactivateOnSwitchEditor() => Deactivate();
        public override void ActivateOnSwitchEditor() => MaybeActivate();
        public override void OnWillBeCopiedInactiveSubtype() => Activate();
        public override void OnWasCopiedInactiveSubtype() => Deactivate();
        public override void OnWasCopiedActiveSubtype() => MaybeActivate();
        public override void OnBeforeReinitializeInactiveSubtype() => Activate();

        private void MaybeActivate() => node.owner.UpdateNodeEnabled(node);
        private void Activate() => node.Unhide();
        private void Deactivate() => node.Hide();
    }
}
