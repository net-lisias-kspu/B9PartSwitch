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
    public interface IPartModifier
    {
        string Description { get; }
        bool ChangesGeometry { get; }

        void DeactivateOnStartEditor();
        void DeactivateOnStartFlight();
        void ActivateOnStartEditor();
        void ActivateOnStartFlight();
        void ActivateOnStartFinishedEditor();
        void ActivateOnStartFinishedFlight();
        void DeactivateOnStartFinishedEditor();
        void DeactivateOnStartFinishedFlight();
        void DeactivateOnSwitchEditor();
        void DeactivateOnSwitchFlight();
        void ActivateOnSwitchEditor();
        void ActivateOnSwitchFlight();
        void OnIconCreateInactiveSubtype();
        void OnIconCreateActiveSubtype();
        void UpdateVolumeEditor();
        void UpdateVolumeFlight();
        void OnWillBeCopiedActiveSubtype();
        void OnWillBeCopiedInactiveSubtype();
        void OnWasCopiedActiveSubtype();
        void OnWasCopiedInactiveSubtype();
        void OnBeforeReinitializeInactiveSubtype();
        void OnBeforeReinitializeActiveSubtype();
        void OnAfterReinitializeInactiveSubtype();
        void OnAfterReinitializeActiveSubtype();
    }
}
