using System;
using UnityEngine;

namespace B9PartSwitch
{
    public class AttachNodeModifier
    {
        public readonly AttachNode attachNode;

        public readonly Vector3? position;

        public AttachNodeModifier(AttachNode attachNode, Vector3? position)
        {
            attachNode.ThrowIfNullArgument(nameof(attachNode));

            this.attachNode = attachNode;
            this.position = position;
        }

        public void ActivateOnStart()
        {
            if (position is Vector3 newPoosition)
            {
                attachNode.position = newPoosition;
            }
        }

        public void DeactivateOnStart()
        {
        }

        public void ActivateOnSwitch()
        {
            if (position is Vector3 newPoosition)
            {
                Vector3 offset = newPoosition - attachNode.position;
                attachNode.position = newPoosition;

                if (!HighLogic.LoadedSceneIsEditor) return;
                if (attachNode.owner.parent != null && attachNode.owner.parent == attachNode.attachedPart)
                {
                    attachNode.owner.transform.localPosition -= offset;
                }
                else if (attachNode.attachedPart != null)
                {
                    attachNode.attachedPart.transform.localPosition += offset;
                }
            }
        }

        public void DeactivateOnSwitch()
        {
            if (position is Vector3 newPoosition)
            {
                Vector3 offset = attachNode.originalPosition - attachNode.position;
                attachNode.position = attachNode.originalPosition;
                
                if (!HighLogic.LoadedSceneIsEditor) return;
                if (attachNode.owner.parent != null && attachNode.owner.parent == attachNode.attachedPart)
                {
                    attachNode.owner.transform.localPosition -= offset;
                }
                else if (attachNode.attachedPart != null)
                {
                    attachNode.attachedPart.transform.localPosition += offset;
                }
            }
        }
    }
}
