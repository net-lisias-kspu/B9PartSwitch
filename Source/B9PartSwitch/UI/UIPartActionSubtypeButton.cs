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
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using KSP.UI.TooltipTypes;

namespace B9PartSwitch.UI
{
    public class UIPartActionSubtypeButton : MonoBehaviour, IEventSystemHandler
    {
        [SerializeField]
        private Image imagePrimaryColor;

        [SerializeField]
        private Image imageSecondaryColor;

        [SerializeField]
        private Image imageSelected;

        [SuppressMessage("Code Quality", "IDE0052", Justification = "Reserved for future use")]
        [SerializeField]
        private Image imageInvalid;

        [SerializeField]
        private Button buttonMain;

        [SerializeField]
        private TooltipController_TitleAndText tooltipController;

        public UIPartActionSubtypeButton previousItem;
        public UIPartActionSubtypeButton nextItem;

        public string Title { get; private set; }
        public string Description { get; private set; }

        private Callback setSubtype;

        public static GameObject CreatePrefab(GameObject variantButtonGameObject)
        {
            GameObject partActionVariantButtonGameObject = Instantiate(variantButtonGameObject);
            DontDestroyOnLoad(partActionVariantButtonGameObject);

            UIPartActionVariantButton partActionVariantButton = partActionVariantButtonGameObject.GetComponent<UIPartActionVariantButton>();
            UIPartActionSubtypeButton partActionSubtypeButton = partActionVariantButtonGameObject.AddComponent<UIPartActionSubtypeButton>();

            partActionSubtypeButton.imagePrimaryColor = partActionVariantButton.imagePrimaryColor;
            partActionSubtypeButton.imageSecondaryColor = partActionVariantButton.imageSecomdaryColor;
            partActionSubtypeButton.imageSelected = partActionVariantButton.imageSelected;
            partActionSubtypeButton.imageInvalid = partActionVariantButton.imageInvalid;
            partActionSubtypeButton.buttonMain = partActionVariantButton.buttonMain;
            partActionSubtypeButton.tooltipController = partActionVariantButtonGameObject.AddComponent<TooltipController_TitleAndText>();

            Destroy(partActionVariantButton);

            return partActionVariantButtonGameObject;
        }

        public void Setup(string title, string description, Color primaryColor, Color secondaryColor, Callback setSubtype)
        {
            Title = title;
            Description = description;

            imagePrimaryColor.color = primaryColor;
            imageSecondaryColor.color = secondaryColor;

            TooltipHelper.SetupSubtypeInfoTooltip(tooltipController, title, description);

            buttonMain.onClick.AddListener(ButtonClick);

            this.setSubtype = setSubtype;
        }

        public void SetSubtype() => setSubtype();

        public void Activate() => imageSelected.gameObject.SetActive(true);

        public void Deactivate() => imageSelected.gameObject.SetActive(false);

        private void ButtonClick() => setSubtype();
    }
}
