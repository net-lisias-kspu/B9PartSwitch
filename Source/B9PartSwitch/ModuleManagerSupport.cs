/*
		This file is part of B9PartSwitch /L Unofficial
		© 202 LisiasT

		B9PartSwitch /L Unofficial is licensed as follows:

		* LGPL 3.0 : https://www.gnu.org/licenses/lgpl-3.0.txt

		B9PartSwitch /L Unofficial is distributed in the hope that it will be useful,
		but WITHOUT ANY WARRANTY; without even the implied warranty of
		MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

		You should have received a copy of the GNU Lesser General Public License 3.0
		along with B9PartSwitch /L Unofficial. If not, see <https://www.gnu.org/licenses/>.
*/
using System.Collections.Generic;

namespace B9PartSwitch
{
	public static class ModuleManagerSupport
	{
		public static IEnumerable<string> ModuleManagerAddToModList()
		{
			string[] r = {typeof(ModuleManagerSupport).Namespace};
			return r;
		}
	}
}
