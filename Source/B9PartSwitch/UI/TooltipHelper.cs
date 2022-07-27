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
using UnityEngine;
using KSP.UI;
using KSP.UI.TooltipTypes;

namespace B9PartSwitch.UI
{
    public static class TooltipHelper
    {
        private static Tooltip subtypeInfoTooltip;

        public static void EnsurePrefabs()
        {
            if (subtypeInfoTooltip.IsNotNull()) return;

            subtypeInfoTooltip = CreateSubtypeInfoTooltipPrefab();
            Log.info("[UI.TooltipHelper] created subtype info tooltip prefab");

        }

        public static TooltipController_TitleAndText SetupSubtypeInfoTooltip(GameObject gameObject, string titleString, string textString)
        {
            TooltipController_TitleAndText tooltipController = gameObject.AddOrGetComponent<TooltipController_TitleAndText>();
            tooltipController.TooltipPrefabType = subtypeInfoTooltip;

            tooltipController.titleString = titleString;
            tooltipController.textString = textString;
            return tooltipController;
        }

        public static void SetupSubtypeInfoTooltip(TooltipController_TitleAndText tooltipController, string titleString, string textString)
        {
            tooltipController.TooltipPrefabType = subtypeInfoTooltip;

            tooltipController.titleString = titleString;
            tooltipController.textString = textString;
        }

        private static Tooltip CreateSubtypeInfoTooltipPrefab()
        {
            Tooltip tooltipPrefab_TitleAndText = AssetBase.GetPrefab<Tooltip>("Tooltip_TitleAndText");
            GameObject subtypeInfoTooltipPrefabGameObject = UnityEngine.Object.Instantiate(tooltipPrefab_TitleAndText.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(subtypeInfoTooltipPrefabGameObject);
            subtypeInfoTooltipPrefabGameObject.GetChild("Text").GetComponent<TMPro.TextMeshProUGUI>().alpha = 0.9f;
            subtypeInfoTooltipPrefabGameObject.GetChild("Title").GetComponent<UnityEngine.UI.LayoutElement>().minWidth = 300;

            return subtypeInfoTooltipPrefabGameObject.GetComponent<Tooltip>();
        }
    }
}
