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

namespace B9PartSwitch.Fishbones.Parsers
{
    public static class NodeObjectWrapper
    {
        public static INodeObjectWrapper For(Type type)
        {
            type.ThrowIfNullArgument(nameof(type));
            if (type.Implements<IConfigNode>())
                return new NodeObjectWrapperIConfigNode(type);
            else if (type.Implements<IContextualNode>())
                return new NodeObjectWrapperIContextualNode(type);
            else if (type == typeof(ConfigNode))
                return new NodeObjectWrapperConfigNode();
            else
                throw new NotImplementedException($"No way to build node object wrapper for type {type}");
        }

        public static bool IsNodeType(Type type)
        {
            return type.Implements<IConfigNode>() || type.Implements<IContextualNode>() || type == typeof(ConfigNode);
        }
    }
}
