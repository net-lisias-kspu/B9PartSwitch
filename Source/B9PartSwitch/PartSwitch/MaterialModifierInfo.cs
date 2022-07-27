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
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using B9PartSwitch.Fishbones;
using B9PartSwitch.Fishbones.Context;
using B9PartSwitch.PartSwitch.PartModifiers;
using B9PartSwitch.Utils;

namespace B9PartSwitch
{
    public class MaterialModifierInfo : IContextualNode
    {
        [NodeData]
        public string name;

        [NodeData(name = "baseTransform")]
        public List<IStringMatcher> baseTransformNames = new List<IStringMatcher>();

        [NodeData(name = "transform")]
        public List<IStringMatcher> transformNames = new List<IStringMatcher>();

        [NodeData(name = "FLOAT")]
        public List<FloatPropertyModifierInfo> floatPropertyModifierInfos = new List<FloatPropertyModifierInfo>();

        [NodeData(name = "COLOR")]
        public List<ColorPropertyModifierInfo> colorPropertyModifierInfos = new List<ColorPropertyModifierInfo>();

        [NodeData(name = "TEXTURE")]
        public List<TexturePropertyModifierInfo> texturePropertyModifierInfos = new List<TexturePropertyModifierInfo>();

        public void Load(ConfigNode node, OperationContext context) => this.LoadFields(node, context);

        public void Save(ConfigNode node, OperationContext context) => this.SaveFields(node, context);

        public IEnumerable<IPartModifier> CreateModifiers(Transform rootTransform, Action<string> onError)
        {
            rootTransform.ThrowIfNullArgument(nameof(rootTransform));

            IEnumerable<Renderer> renderers;
            if (baseTransformNames.IsNullOrEmpty() && transformNames.IsNullOrEmpty())
            {
                renderers = rootTransform.GetComponentsInChildren<Renderer>(true);
            }
            else
            {
                renderers = GetBaseTransformRenderers(rootTransform, onError);
                renderers = renderers.Concat(GetTransformRenderers(rootTransform, onError));

                renderers = renderers.Distinct();
            }

            foreach (FloatPropertyModifierInfo floatPropertyModifierInfo in floatPropertyModifierInfos)
            {
                foreach (IPartModifier partModifier in floatPropertyModifierInfo.CreateModifiers(renderers))
                {
                    yield return partModifier;
                }
            }

            foreach (ColorPropertyModifierInfo colorPropertyModifierInfo in colorPropertyModifierInfos)
            {
                foreach (IPartModifier partModifier in colorPropertyModifierInfo.CreateModifiers(renderers))
                {
                    yield return partModifier;
                }
            }

            foreach (TexturePropertyModifierInfo texturePropertyModifierInfo in texturePropertyModifierInfos)
            {
                foreach (IPartModifier partModifier in texturePropertyModifierInfo.CreateModifiers(renderers, onError))
                {
                    yield return partModifier;
                }
            }
        }

        private IEnumerable<Renderer> GetBaseTransformRenderers(Transform rootTransform, Action<string> onError)
        {
            IEnumerable<Renderer> result = Enumerable.Empty<Renderer>();
            if (baseTransformNames == null) return result;

            foreach (IStringMatcher baseTransformName in baseTransformNames)
            {
                bool foundTransform = false;

                foreach (Transform transform in rootTransform.TraverseHierarchy().Where(t => baseTransformName.Match(t.name)))
                {
                    foundTransform = true;

                    Renderer[] transformRenderers = transform.GetComponentsInChildren<Renderer>(true);

                    if (transformRenderers.Length == 0)
                    {
                        onError($"No renderers found on transform '{baseTransformName}'");
                        continue;
                    }

                    result = result.Concat(transformRenderers);
                }

                if (!foundTransform) onError($"No transforms matching '{baseTransformName}' found");
            }

            return result;
        }

        private IEnumerable<Renderer> GetTransformRenderers(Transform rootTransform, Action<string> onError)
        {
            IEnumerable<Renderer> result = Enumerable.Empty<Renderer>();
            if (transformNames == null) return result;

            foreach (IStringMatcher transformName in transformNames)
            {
                bool foundTransform = false;

                foreach (Transform transform in rootTransform.TraverseHierarchy().Where(t => transformName.Match(t.name)))
                {
                    foundTransform = true;
                    Renderer[] transformRenderers = transform.GetComponents<Renderer>();

                    if (transformRenderers.Length == 0)
                    {
                        onError($"No renderers found on transform '{transformName}'");
                        continue;
                    }

                    result = result.Concat(transformRenderers);
                }

                if (!foundTransform) onError($"No transforms matching '{transformName}' found");
            }

            return result;
        }
    }
}
