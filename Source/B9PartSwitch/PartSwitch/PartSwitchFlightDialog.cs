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
using System.Collections.Generic;
using System.Linq;
using TMPro;
using B9PartSwitch.UI;

namespace B9PartSwitch
{
    public static class PartSwitchFlightDialog
    {
        public static void Spawn(ModuleB9PartSwitch module)
        {
            try
            {
                MaybeCreateResourceRemovalWarning(module, () => CreateDialogue(module));
            }
            catch (Exception ex)
            {
                Log.error(ex, ex.Message);
                FatalErrorHandler.HandleFatalError(ex);
            }
        }

        private static void MaybeCreateResourceRemovalWarning(ModuleB9PartSwitch module, Action onConfirm)
        {
            if (HighLogic.LoadedSceneIsFlight && module.CurrentTankType.ResourceNames.Any(name => module.part.Resources[name].amount > 0))
            {
                CreateWarning(module, onConfirm);
            }
            else
            {
                onConfirm();
            }
        }

        private static void CreateWarning(ModuleB9PartSwitch module, Action onConfirm)
        {
            PopupDialog.SpawnPopupDialog(
                new MultiOptionDialog(
                    "B9PartSwitch_SwitchInFlightWarning",
                    Localization.PartSwitchFlightDialog_ResourcesWillBeDumpedWarning(module.part.partInfo.title, module.switcherDescription), // <<1>> has resources that will be dumped by switching the <<2>>
                    Localization.PartSwitchFlightDialog_ConfirmResourceRemovalDialogTitle, // Confirm Resource Removal
                    HighLogic.UISkin,
                    new DialogGUIButton(Localization.PartSwitchFlightDialog_AcceptString, () => onConfirm()),
                    new DialogGUIButton(Localization.PartSwitchFlightDialog_CancelString, delegate { })
                ),
                false,
                HighLogic.UISkin
            );
        }

        private static void CreateDialogue(ModuleB9PartSwitch module)
        {
            List<Callback> afterCreateCallbacks = new List<Callback>();
            PopupDialog.SpawnPopupDialog(
                new MultiOptionDialog(
                    "B9PartSwitch_SwitchInFlight",
                    Localization.PartSwitchFlightDialog_SelectNewSubtypeDialogTitle(module.switcherDescription), // Select <<1>>
                    module.part.partInfo.title,
                    HighLogic.UISkin,
                    CreateOptions(module, afterCreateCallbacks)
                ),
                false,
                HighLogic.UISkin
            );

            foreach (Callback callback in afterCreateCallbacks)
            {
                callback();
            }
        }

        private static DialogGUIBase[] CreateOptions(ModuleB9PartSwitch module, IList<Callback> afterCreateCallbacks)
        {
            List<DialogGUIBase> options = new List<DialogGUIBase>();

            SwitcherSubtypeDescriptionGenerator subtypeDescriptionGenerator = new SwitcherSubtypeDescriptionGenerator(module);

            foreach (PartSubtype subtype in module.subtypes)
            {
                if (!subtype.IsUnlocked()) continue;

                if (subtype == module.CurrentSubtype)
                {
                    string currentSubtypeText = Localization.PartSwitchFlightDialog_CurrentSubtypeLabel(subtype.title);  // <<1>> (Current)
                    DialogGUILabel label = new DialogGUILabel(currentSubtypeText, HighLogic.UISkin.button);
                    afterCreateCallbacks.Add(delegate
                    {
                        if (!(label.uiItem.GetComponent<TextMeshProUGUI>() is TextMeshProUGUI textUI))
                            throw new Exception("Could not find TextMeshProUGUI");
                        else
                            textUI.raycastTarget = true;
                    });
                    afterCreateCallbacks.Add(() => TooltipHelper.SetupSubtypeInfoTooltip(label.uiItem, subtype.title, subtypeDescriptionGenerator.GetFullSubtypeDescription(subtype)));
                    options.Add(label);
                }
                else if (HighLogic.LoadedSceneIsEditor || subtype.allowSwitchInFlight)
                {
                    DialogGUIButton button = new DialogGUIButton(subtype.title, () => module.SwitchSubtype(subtype.Name));
                    afterCreateCallbacks.Add(() => TooltipHelper.SetupSubtypeInfoTooltip(button.uiItem, subtype.title, subtypeDescriptionGenerator.GetFullSubtypeDescription(subtype)));
                    options.Add(button);
                }
            }

            options.Add(new DialogGUIButton(Localization.PartSwitchFlightDialog_CancelString, delegate { } ));

            return options.ToArray();
        }
    }
}
