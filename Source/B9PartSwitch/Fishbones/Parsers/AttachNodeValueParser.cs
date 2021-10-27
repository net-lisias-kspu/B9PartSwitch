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

namespace B9PartSwitch.Fishbones.Parsers
{
    public class AttachNodeValueParser : ValueParser<AttachNode>
    {
        public static AttachNode ParseAttachNode(string value)
        {
            value.ThrowIfNullArgument(nameof(value));

            string[] splitStr = value.Split(',');
            int length = splitStr.Length;

            if (length < 6)
            {
                throw new FormatException($"Not enough values to parse an AttachNode: '{value}'");
            }

            float[] floatValues = new float[6];
            for (int i = 0; i < 6; i++)
            {
                floatValues[i] = float.Parse(splitStr[i]);
            }

            AttachNode attachNode = new AttachNode();

            attachNode.id = "parsed-attach-node";

            attachNode.position = new Vector3(floatValues[0], floatValues[1], floatValues[2]);
            attachNode.orientation = new Vector3(floatValues[3], floatValues[4], floatValues[5]);

            attachNode.originalPosition = attachNode.position;
            attachNode.originalOrientation = attachNode.orientation;

            if (length >= 7)
                attachNode.size = int.Parse(splitStr[6]);
            else
                attachNode.size = 1;

            if (length >= 8)
                attachNode.attachMethod = (AttachNodeMethod)int.Parse(splitStr[7]);

            if (length >= 9)
                attachNode.ResourceXFeed = (int.Parse(splitStr[8]) > 0);

            if (length >= 10)
                attachNode.rigid = (int.Parse(splitStr[9]) > 0);

            return attachNode;
        }

        public static string FormatAttachNode(AttachNode node)
        {
            node.ThrowIfNullArgument(nameof(node));

            return string.Join(", ", new[] {
                node.position.x.ToString(),
                node.position.y.ToString(),
                node.position.z.ToString(),
                node.orientation.x.ToString(),
                node.orientation.y.ToString(),
                node.orientation.z.ToString(),
                node.size.ToString(),
                Enum.Format(typeof(AttachNodeMethod), node.attachMethod, "d"),
                node.ResourceXFeed ? "1" : "0",
                node.rigid ? "1" : "0",
            });
        }

        public AttachNodeValueParser() : base(ParseAttachNode, FormatAttachNode) { }
    }
}
