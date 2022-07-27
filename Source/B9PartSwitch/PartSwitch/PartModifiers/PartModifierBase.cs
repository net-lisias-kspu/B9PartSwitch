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
    public abstract class PartModifierBase : IPartModifier
    {
        public abstract string Description { get; }
        public virtual bool ChangesGeometry => false;

        public virtual void DeactivateOnStartEditor() { }
        public virtual void DeactivateOnStartFlight() { }
        public virtual void ActivateOnStartEditor() { }
        public virtual void ActivateOnStartFlight() { }
        public virtual void ActivateOnStartFinishedEditor() { }
        public virtual void ActivateOnStartFinishedFlight() { }
        public virtual void DeactivateOnStartFinishedEditor() { }
        public virtual void DeactivateOnStartFinishedFlight() { }
        public virtual void DeactivateOnSwitchEditor() { }
        public virtual void DeactivateOnSwitchFlight() { }
        public virtual void ActivateOnSwitchEditor() { }
        public virtual void ActivateOnSwitchFlight() { }
        public virtual void OnIconCreateInactiveSubtype() { }
        public virtual void OnIconCreateActiveSubtype() { }
        public virtual void UpdateVolumeEditor() { }
        public virtual void UpdateVolumeFlight() { }
        public virtual void OnWillBeCopiedActiveSubtype() { }
        public virtual void OnWillBeCopiedInactiveSubtype() { }
        public virtual void OnWasCopiedActiveSubtype() { }
        public virtual void OnWasCopiedInactiveSubtype() { }
        public virtual void OnBeforeReinitializeInactiveSubtype() { }
        public virtual void OnBeforeReinitializeActiveSubtype() { }
        public virtual void OnAfterReinitializeInactiveSubtype() { }
        public virtual void OnAfterReinitializeActiveSubtype() { }
    }
}
