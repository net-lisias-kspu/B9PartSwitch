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
using UnityEngine;
using B9PartSwitch.Fishbones;
using B9PartSwitch.Fishbones.Context;
using B9PartSwitch.PartSwitch.PartModifiers;
using B9PartSwitch.Utils;

namespace B9PartSwitch
{
    public class TransformModifierInfo : IContextualNode
    {
        [NodeData(name = "name")]
        public IStringMatcher transformName;

        [NodeData]
        public Vector3? positionOffset;

        [NodeData]
        public Vector3? rotationOffset;

        [NodeData]
        [UseParser(typeof(Fishbones.Parsers.ScaleParser))]
        public Vector3? scaleOffset;

        public void Load(ConfigNode node, OperationContext context) => this.LoadFields(node, context);
        public void Save(ConfigNode node, OperationContext context) => this.SaveFields(node, context);

        public IEnumerable<IPartModifier> CreatePartModifiers(Part part, Action<string> onError)
        {
            part.ThrowIfNullArgument(nameof(part));
            onError.ThrowIfNullArgument(nameof(onError));

            if (transformName == null)
            {
                onError("transform name is null");
                yield break;
            }

            bool foundTransform = false;

            foreach (Transform transform in part.GetModelRoot().TraverseHierarchy().Where(t => transformName.Match(t.name)))
            {
                foundTransform = true;

                if (positionOffset.HasValue)
                {
                    yield return new TransformMover(transform, positionOffset.Value);
                }
                if (rotationOffset.HasValue)
                {
                    yield return new TransformRotator(transform, Quaternion.Euler(rotationOffset.Value));
                }
                if (scaleOffset.HasValue)
                {
                    yield return new TransformScaleModifier(transform, scaleOffset.Value);
                }
            }

            if (!foundTransform) onError($"Could not find any transform named '{transformName}'");
        }
    }
}
