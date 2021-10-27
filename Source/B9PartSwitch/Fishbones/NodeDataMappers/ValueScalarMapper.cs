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
    public class ValueScalarMapper : INodeDataMapper
    {
        public readonly string name;
        public readonly IValueParser parser;

        public ValueScalarMapper(string name, IValueParser parser)
        {
            name.ThrowIfNullArgument(nameof(name));
            parser.ThrowIfNullArgument(nameof(parser));

            this.name = name;
            this.parser = parser;
        }

        public bool Load(ref object fieldValue, ConfigNode node, OperationContext context)
        {
            node.ThrowIfNullArgument(nameof(node));

            string value = node.GetValue(name);
            if (value.IsNull()) return false;

            fieldValue = parser.Parse(value);
            return true;
        }

        public bool Save(object fieldValue, ConfigNode node, OperationContext context)
        {
            node.ThrowIfNullArgument(nameof(node));
            if (fieldValue.IsNull()) return false;

            string value = parser.Format(fieldValue);
            if (value.IsNull()) return false;

            node.SetValue(name, value, true);
            return true;
        }
    }
}
