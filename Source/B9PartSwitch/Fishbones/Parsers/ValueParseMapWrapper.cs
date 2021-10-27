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
    public class ValueParseMapWrapper : IValueParseMap
    {
        private readonly IValueParseMap map;

        public ValueParseMapWrapper(IValueParseMap map)
        {
            map.ThrowIfNullArgument(nameof(map));
            this.map = map;
        }

        public IValueParser GetParser(Type parseType)
        {
            parseType.ThrowIfNullArgument(nameof(parseType));
            return map.GetParser(parseType);
        }

        public bool CanParse(Type parseType)
        {
            parseType.ThrowIfNullArgument(nameof(parseType));
            return map.CanParse(parseType);
        }
    }
}
