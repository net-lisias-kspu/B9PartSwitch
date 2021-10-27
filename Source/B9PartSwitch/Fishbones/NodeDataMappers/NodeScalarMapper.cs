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
using B9PartSwitch.Fishbones.Parsers;
using B9PartSwitch.Fishbones.Context;

namespace B9PartSwitch.Fishbones.NodeDataMappers
{
    public class NodeScalarMapper : INodeDataMapper
    {
        public readonly string name;
        public readonly INodeObjectWrapper nodeObjectWrapper;

        public NodeScalarMapper(string name, INodeObjectWrapper nodeObjectWrapper)
        {
            name.ThrowIfNullArgument(nameof(name));
            this.name = name;

            nodeObjectWrapper.ThrowIfNullArgument(nameof(nodeObjectWrapper));
            this.nodeObjectWrapper = nodeObjectWrapper;
        }

        public bool Load(ref object fieldValue, ConfigNode node, OperationContext context)
        {
            node.ThrowIfNullArgument(nameof(node));
            context.ThrowIfNullArgument(nameof(context));

            ConfigNode innerNode = node.GetNode(name);
            if (innerNode.IsNull()) return false;

            nodeObjectWrapper.Load(ref fieldValue, innerNode, context);
             return true;
        }

        public bool Save(object fieldValue, ConfigNode node, OperationContext context)
        {
            node.ThrowIfNullArgument(nameof(node));
            context.ThrowIfNullArgument(nameof(context));

            if (fieldValue.IsNull()) return false;

            ConfigNode innerNode = nodeObjectWrapper.Save(fieldValue, context);

            node.AddNode(name, innerNode);
            return true;
        }
    }
}
