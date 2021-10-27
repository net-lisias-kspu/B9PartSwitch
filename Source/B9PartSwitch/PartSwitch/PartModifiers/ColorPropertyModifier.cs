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
    public class ColorPropertyModifier : PartModifierBase, IPartAspectLock
    {
        private readonly Renderer renderer;
        private readonly string shaderProperty;
        private readonly Color originalColor;
        private readonly Color newColor;

        public ColorPropertyModifier(Renderer renderer, string shaderProperty, Color newColor)
        {
            renderer.ThrowIfNullArgument(nameof(renderer));
            shaderProperty.ThrowIfNullOrEmpty(nameof(shaderProperty));
            newColor.ThrowIfNullArgument(nameof(newColor));

            this.renderer = renderer;
            this.shaderProperty = shaderProperty;
            this.newColor = newColor;

            if (!renderer.sharedMaterial.HasProperty(shaderProperty)) throw new ArgumentException($"{renderer.sharedMaterial.name} has no property {shaderProperty}");

            originalColor = renderer.sharedMaterial.GetColor(shaderProperty);
        }

        public object PartAspectLock => renderer.GetInstanceID() + "---" + shaderProperty;
        public override string Description => $"object {renderer.name} shader property {shaderProperty}";

        public override void ActivateOnStartEditor() => Activate();
        public override void ActivateOnStartFlight() => Activate();
        public override void ActivateOnSwitchEditor() => Activate();
        public override void ActivateOnSwitchFlight() => Activate();
        public override void DeactivateOnSwitchEditor() => Deactivate();
        public override void DeactivateOnSwitchFlight() => Deactivate();
        public override void OnIconCreateActiveSubtype() => Activate();
        public override void OnWillBeCopiedActiveSubtype() => Deactivate();
        public override void OnWasCopiedActiveSubtype()
        {
            // At this point, the copy hasn't been initialized yet, so it still shares a material with this
            // So make a copy of the material and assign it to avoid affecting the copy too
            renderer.material = new Material(renderer.material);
            Activate();
        }

        public override void OnBeforeReinitializeActiveSubtype() => Deactivate();
        public override void OnAfterReinitializeActiveSubtype() => Activate();

        private void Activate() => renderer.material.SetColor(shaderProperty, newColor);
        private void Deactivate() => renderer.material.SetColor(shaderProperty, originalColor);
    }
}
