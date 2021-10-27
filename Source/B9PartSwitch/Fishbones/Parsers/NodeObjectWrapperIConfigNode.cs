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
using B9PartSwitch.Fishbones.Context;

namespace B9PartSwitch.Fishbones.Parsers
{
    public class NodeObjectWrapperIConfigNode : INodeObjectWrapper
    {
        public readonly Type type;

        public NodeObjectWrapperIConfigNode(Type type)
        {
            type.ThrowIfNullArgument(nameof(type));
            if (!type.Implements<IConfigNode>()) throw new ArgumentException($"Type {type} does not implement {nameof(IConfigNode)}", nameof(type));
            this.type = type;
        }

        public void Load(ref object obj, ConfigNode node, OperationContext context)
        {
            obj.EnsureArgumentType<IConfigNode>(nameof(obj));
            node.ThrowIfNullArgument(nameof(node));

            if (obj.IsNull()) obj = Activator.CreateInstance(type);

            ((IConfigNode)obj).Load(node);
        }

        public ConfigNode Save(object obj, OperationContext context)
        {
            obj.ThrowIfNullArgument(nameof(obj));
            obj.EnsureArgumentType<IConfigNode>(nameof(obj));

            ConfigNode node = new ConfigNode();
            ((IConfigNode)obj).Save(node);
            return node;
        }
    }
}
