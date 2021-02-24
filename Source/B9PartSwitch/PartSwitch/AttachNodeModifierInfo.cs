﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using B9PartSwitch.Fishbones;
using B9PartSwitch.Fishbones.Context;
using B9PartSwitch.PartSwitch.PartModifiers;
using B9PartSwitch.Utils;

namespace B9PartSwitch
{
    public class AttachNodeModifierInfo : IContextualNode
    {
        [NodeData(name = "name")]
        public IStringMatcher nodeID;

        [NodeData]
        public Vector3? position;

        [NodeData]
        public int? size;

        public void Load(ConfigNode node, OperationContext context)
        {
            this.LoadFields(node, context);
        }

        public void Save(ConfigNode node, OperationContext context)
        {
            this.SaveFields(node, context);
        }

        public IEnumerable<IPartModifier> CreatePartModifiers(Part part, ILinearScaleProvider linearScaleProvider, Action<string> onError)
        {
            AttachNode node = part.attachNodes.FirstOrDefault(n => (n.nodeType == AttachNode.NodeType.Stack || n.nodeType == AttachNode.NodeType.Dock) && nodeID.Match(n.id));

            if (node == null)
            {
                onError($"Attach node with id matching '{nodeID}' not found for attach node modifier");
                yield break;
            }

            // Explanation
            // Config has scale and rescaleFactor which both multiply node positions, but doesn't store scale directly
            // Instead it stores scaleFactor which is scale / rescaleFactor
            // So we have to multiply by rescaleFactor again to get it back
            // Use the prefab since TweakScale modifies rescaleFactor
            Part maybePrefab = part.partInfo?.partPrefab ?? part;
            float fixedScale = maybePrefab.scaleFactor * maybePrefab.rescaleFactor * maybePrefab.rescaleFactor;

            if (position != null) yield return new AttachNodeMover(node, position.Value * fixedScale, linearScaleProvider);
            if (size != null) yield return new AttachNodeSizeModifier(node, size.Value, linearScaleProvider);
        }
    }
}
