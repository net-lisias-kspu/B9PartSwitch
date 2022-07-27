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
using System.Collections;
using System.Collections.Generic;
using UniLinq;

namespace B9PartSwitch
{
    public static class ListExtensions
    {
        public static bool IsList(this object o)
        {
            if (o == null)
                return false;
            return IsListType(o.GetType());
        }

        public static bool IsListType(this Type t)
        {
            if (t == null)
                return false;
            return t.GetInterfaces().Contains(typeof(IList)) && t.IsGenericType && t.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }

        public static bool IsNullOrEmpty(this IList list) => list.IsNull() || list.Count == 0;

        public static bool ValidIndex(this IList list, int index) => (index >= 0) && (index < list.Count);

        public static bool SameElementsAs<T>(this IEnumerable<T> set1, IEnumerable<T> set2) =>
            (set1.Count() == set2.Count()) &&
            !set1.Except(set1).Any() &&
            !set2.Except(set1).Any();
    }
}
