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

namespace B9PartSwitch.Fishbones
{
    public interface IUseParser
    {
        IValueParser CreateParser();
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class UseParser : Attribute, IUseParser
    {
        private readonly Type valueParserType;

        public UseParser(Type valueParserType)
        {
            valueParserType.ThrowIfNullArgument(nameof(valueParserType));

            if (!valueParserType.Implements<IValueParser>())
                throw new ArgumentException($"Type {valueParserType} does not implement {typeof(IValueParser)}");

            if (valueParserType.GetConstructor(Type.EmptyTypes).IsNull())
                throw new ArgumentException($"Type {valueParserType} does not have a parameterless constructor");

            this.valueParserType = valueParserType;
        }

        public IValueParser CreateParser()
        {
            return (IValueParser)Activator.CreateInstance(valueParserType);
        }
    }
}
