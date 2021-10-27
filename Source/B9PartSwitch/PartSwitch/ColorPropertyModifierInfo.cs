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
using UnityEngine;
using B9PartSwitch.Fishbones;
using B9PartSwitch.Fishbones.Context;
using B9PartSwitch.PartSwitch.PartModifiers;

namespace B9PartSwitch
{
    public class ColorPropertyModifierInfo : IContextualNode
    {
        [NodeData(name = "color")]
        public Color newColor;

        [NodeData(name = "shaderProperty")]
        public string shaderPropName = "_Color";

        public void Load(ConfigNode node, OperationContext context) => this.LoadFields(node, context);

        public void Save(ConfigNode node, OperationContext context) => this.SaveFields(node, context);

        public IEnumerable<IPartModifier> CreateModifiers(IEnumerable<Renderer> renderers)
        {
            foreach (Renderer renderer in renderers)
            {
                if (!renderer.sharedMaterial.HasProperty(shaderPropName)) continue;

                yield return new ColorPropertyModifier(renderer, shaderPropName, newColor);
            }
        }
    }
}
