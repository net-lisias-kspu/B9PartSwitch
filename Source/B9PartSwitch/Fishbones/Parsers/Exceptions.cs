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
    [Serializable]
    public class ParseTypeNotRegisteredException : Exception
    {
        public ParseTypeNotRegisteredException(Type parseType) :
            base($"Attempted to get the parser for type '{parseType}', but it has not been registered")
        {
            parseType.ThrowIfNullArgument(nameof(parseType));
        }
    }

    [Serializable]
    public class ParseTypeAlreadyRegisteredException : Exception
    {
        public ParseTypeAlreadyRegisteredException(Type parseType) :
            base($"Attempted to register perser for type '{parseType}', but it has already been registered")
        {
            parseType.ThrowIfNullArgument(nameof(parseType));
        }
    }
}
