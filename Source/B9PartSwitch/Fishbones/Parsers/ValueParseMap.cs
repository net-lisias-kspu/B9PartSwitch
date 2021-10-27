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
using System.Collections.Generic;

namespace B9PartSwitch.Fishbones.Parsers
{
    public class ValueParseMap : IMutableValueParseMap
    {
        protected Dictionary<Type, IValueParser> parsers = new Dictionary<Type, IValueParser>();

        public virtual IValueParser GetParser(Type parseType)
        {
            parseType.ThrowIfNullArgument(nameof(parseType));

            if (parsers.TryGetValue(parseType, out IValueParser parser))
                return parser;
            else
                throw new ParseTypeNotRegisteredException(parseType);
        }

        public virtual void AddParser<T>(Func<string, T> parse, Func<T, string> format)
        {
            parse.ThrowIfNullArgument(nameof(parse));
            format.ThrowIfNullArgument(nameof(format));

            AddParser(new ValueParser<T>(parse, format));
        }

        public virtual void AddParser(IValueParser parser)
        {
            parser.ThrowIfNullArgument(nameof(parser));
            if (!CanAdd(parser.ParseType)) throw new ParseTypeAlreadyRegisteredException(parser.ParseType);
            parsers[parser.ParseType] = parser;
        }

        public virtual bool CanParse(Type parseType)
        {
            parseType.ThrowIfNullArgument(nameof(parseType));
            return parsers.ContainsKey(parseType);
        }

        public virtual bool CanAdd(Type parseType)
        {
            parseType.ThrowIfNullArgument(nameof(parseType));
            return !parsers.ContainsKey(parseType);
        }

        public ValueParseMap Clone()
        {
            ValueParseMap clone = new ValueParseMap();

            foreach(IValueParser parser in parsers.Values)
            {
                clone.AddParser(parser);
            }

            return clone;
        }
    }
}
