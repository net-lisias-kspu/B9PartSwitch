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

namespace B9PartSwitch
{
    public class TankTypeValueParser : IValueParser
    {
        public object Parse(string value)
        {
            value.ThrowIfNullArgument(nameof(value));
            return B9TankSettings.GetTankType(value);
        }

        public string Format(object value)
        {
            value.ThrowIfNullArgument(nameof(value));
            return ((TankType)value).tankName;
        }

        public Type ParseType => typeof(TankType);
    }
}
