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
    public class ValueParser<T> : IValueParser
    {
        private readonly Func<string, T> parseFunction;
        private readonly Func<T, string> formatFunction;

        public ValueParser(Func<string, T> parseFunction, Func<T, string> formatFunction)
        {
            parseFunction.ThrowIfNullArgument(nameof(parseFunction));
            formatFunction.ThrowIfNullArgument(nameof(formatFunction));

            this.parseFunction = parseFunction;
            this.formatFunction = formatFunction;
        }

        public object Parse(string value)
        {
            value.ThrowIfNullArgument(nameof(value));

            return parseFunction(value);
        }

        public string Format(object value)
        {
            value.ThrowIfNullArgument(nameof(value));
            value.EnsureArgumentType<T>(nameof(value));

            return formatFunction((T)value);
        }

        public Type ParseType => typeof(T);
    }
}
