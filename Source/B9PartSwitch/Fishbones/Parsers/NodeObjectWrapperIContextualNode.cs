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
    public class NodeObjectWrapperIContextualNode : INodeObjectWrapper
    {
        public readonly Type type;
        public NodeObjectWrapperIContextualNode(Type type)
        {
            type.ThrowIfNullArgument(nameof(type));
            if (!type.Implements<IContextualNode>()) throw new ArgumentException($"Type {type} does not implement {typeof(IContextualNode)}", nameof(type));
            this.type = type;
        }

        public void Load(ref object obj, ConfigNode node, OperationContext context)
        {
            obj.EnsureArgumentType<IContextualNode>(nameof(obj));
            node.ThrowIfNullArgument(nameof(node));
            context.ThrowIfNullArgument(nameof(context));

            if (obj.IsNull()) obj = Activator.CreateInstance(type);

            ((IContextualNode)obj).Load(node, context);
        }

        public ConfigNode Save(object obj, OperationContext context)
        {
            obj.ThrowIfNullArgument(nameof(obj));
            obj.EnsureArgumentType<IContextualNode>(nameof(obj));
            context.ThrowIfNullArgument(nameof(context));

            ConfigNode node = new ConfigNode();
            ((IContextualNode)obj).Save(node, context);
            return node;
        }
    }
}
