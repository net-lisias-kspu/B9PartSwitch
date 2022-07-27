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
using B9PartSwitch.Fishbones.Parsers;

namespace B9PartSwitch.Fishbones.NodeDataMappers
{
    public class NodeListMapperBuilder : INodeDataMapperBuilder
    {
        public readonly string nodeDataName;
        public readonly Type elementType;

        public NodeListMapperBuilder(string nodeDataName, Type fieldType)
        {
            nodeDataName.ThrowIfNullArgument(nameof(nodeDataName));
            fieldType.ThrowIfNullArgument(nameof(fieldType));

            this.nodeDataName = nodeDataName;

            if (fieldType.IsListType()) elementType = fieldType.GetGenericArguments()[0];
        }

        public bool CanBuild => elementType.IsNotNull() && NodeObjectWrapper.IsNodeType(elementType);

        public INodeDataMapper BuildMapper()
        {
            if (!CanBuild) throw new InvalidOperationException();

            return new NodeListMapper(nodeDataName, elementType, NodeObjectWrapper.For(elementType));
        }
    }
}
