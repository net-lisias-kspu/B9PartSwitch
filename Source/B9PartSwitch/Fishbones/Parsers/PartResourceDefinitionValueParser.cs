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
    // No test for this because PartResourceLibrary can't exist outside of Unity, and not worth writing a wrapper for so little
    public class PartResourceDefinitionValueParser : ValueParser<PartResourceDefinition>
    {
        [Serializable]
        public class PartResourceNotFoundException : Exception
        {
            public PartResourceNotFoundException(string name) : base($"No resource definition named '{name}' could be found") { }
        }
        // This will raise an exception when the resource is not found
        public static PartResourceDefinition FindResourceDefinition(string name)
        {
            name.ThrowIfNullArgument(nameof(name));

            PartResourceDefinition resource = PartResourceLibrary.Instance.GetDefinition(name);
            if (resource.IsNull())
                throw new PartResourceNotFoundException(name);
            return resource;
        }

        public PartResourceDefinitionValueParser() : base(FindResourceDefinition, def => def.name) { }
    }
}
