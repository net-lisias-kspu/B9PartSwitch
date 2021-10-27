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
using System.Collections;
using System.Collections.Generic;
using B9PartSwitch.Fishbones.Parsers;
using B9PartSwitch.Fishbones.Context;

namespace B9PartSwitch.Fishbones.NodeDataMappers
{
    public class NodeListMapper : INodeDataMapper
    {
        public readonly string name;
        public readonly Type listType;
        public readonly INodeObjectWrapper nodeObjectWrapper;

        public NodeListMapper(string name, Type elementType, INodeObjectWrapper nodeObjectWrapper)
        {
            name.ThrowIfNullArgument(nameof(name));
            elementType.ThrowIfNullArgument(nameof(elementType));
            nodeObjectWrapper.ThrowIfNullArgument(nameof(nodeObjectWrapper));

            this.name = name;
            listType = typeof(List<>).MakeGenericType(elementType);

            this.nodeObjectWrapper = nodeObjectWrapper;
        }

        public bool Load(ref object fieldValue, ConfigNode node, OperationContext context)
        {
            node.ThrowIfNullArgument(nameof(node));
            fieldValue.EnsureArgumentType(listType, nameof(fieldValue));
            context.ThrowIfNullArgument(nameof(context));

            ConfigNode[] nodes = node.GetNodes(name);
            if (nodes.IsNullOrEmpty()) return false;

            if (fieldValue.IsNull()) fieldValue = Activator.CreateInstance(listType);
            IList list = (IList)fieldValue;

            if (context.Operation == Operation.Deserialize || context.Operation == Operation.LoadInstance)
                list.Clear();

            foreach (ConfigNode innerNode in nodes)
            {
                if (innerNode.IsNull()) continue;
                object obj = null;
                nodeObjectWrapper.Load(ref obj, innerNode, context);
                list.Add(obj);
            }

            return true;
        }

        public bool Save(object fieldValue, ConfigNode node, OperationContext context)
        {
            fieldValue.EnsureArgumentType(listType, nameof(fieldValue));
            node.ThrowIfNullArgument(nameof(node));
            context.ThrowIfNullArgument(nameof(context));

            IList list = (IList)fieldValue;
            if (list.IsNullOrEmpty()) return false;

            foreach (object value in list)
            {
                if (value.IsNull()) continue;
                node.AddNode(name, nodeObjectWrapper.Save(value, context));
            }

            return true;
        }
    }
}
