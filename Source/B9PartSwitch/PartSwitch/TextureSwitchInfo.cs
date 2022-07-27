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
    public class TextureSwitchInfo : IContextualNode
    {
        [NodeData(name = "currentTexture")]
        public string currentTextureName;

        [NodeData(name = "baseTransform")]
        public List<IStringMatcher> baseTransformNames;

        [NodeData(name = "transform")]
        public List<IStringMatcher> transformNames;

        [NodeData(name = "texture")]
        public string newTexturePath;

        [NodeData]
        public bool isNormalMap = false;

        [NodeData(name = "shaderProperty")]
        public string shaderPropName;

        public void Load(ConfigNode node, OperationContext context)
        {
            this.LoadFields(node, context);
        }

        public void Save(ConfigNode node, OperationContext context)
        {
            this.SaveFields(node, context);
        }

        public IEnumerable<TextureReplacement> CreateTextureReplacements(Part part, Action<string> onError)
        {
            part.ThrowIfNullArgument(nameof(part));

            if (string.IsNullOrEmpty(newTexturePath))
            {
                onError("texture name is empty");
                yield break;
            }

            Texture newTexture = GameDatabase.Instance.GetTexture(newTexturePath, isNormalMap);

            if (newTexture == null)
            {
                onError($"Texture '{newTexturePath}' not found!");
                yield break;
            }

            string shaderProperty = shaderPropName;
            if (string.IsNullOrEmpty(shaderProperty))
            {
                if (isNormalMap) shaderProperty = "_BumpMap";
                else shaderProperty = "_MainTex";
            }

            IEnumerable<Renderer> renderers;
            if (baseTransformNames.IsNullOrEmpty() && transformNames.IsNullOrEmpty())
            {
                renderers = part.GetModelRoot().GetComponentsInChildren<Renderer>(true);
            }
            else
            {
                renderers = GetBaseTransformRenderers(part, onError);
                renderers = renderers.Concat(GetTransformRenderers(part, onError));

                renderers = renderers.Distinct();
            }

            foreach (Renderer renderer in renderers)
            {
                Material sharedMaterial = renderer.sharedMaterial;
                Texture texture = sharedMaterial.GetTexture(shaderProperty);
                if (texture == null) continue;

                if (!currentTextureName.IsNullOrEmpty())
                {
                    string baseTextureName = texture.name.Substring(texture.name.LastIndexOf('/') + 1);
                    if (baseTextureName != currentTextureName) continue;
                }

                yield return new TextureReplacement(renderer, shaderProperty, newTexture);
            }
        }

        private IEnumerable<Renderer> GetBaseTransformRenderers(Part part, Action<string> onError)
        {
            IEnumerable<Renderer> result = Enumerable.Empty<Renderer>();
            if (baseTransformNames == null) return result;

            foreach (IStringMatcher baseTransformName in baseTransformNames)
            {
                bool foundTransform = false;

                foreach (Transform transform in part.GetModelRoot().TraverseHierarchy().Where(t => baseTransformName.Match(t.name)))
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

        private IEnumerable<Renderer> GetTransformRenderers(Part part, Action<string> onError)
        {
            IEnumerable<Renderer> result = Enumerable.Empty<Renderer>();
            if (transformNames == null) return result;

            foreach (IStringMatcher transformName in transformNames)
            {
                bool foundTransform = false;

                foreach (Transform transform in part.GetModelRoot().TraverseHierarchy().Where(t => transformName.Match(t.name)))
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
