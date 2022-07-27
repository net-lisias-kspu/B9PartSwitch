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
using System.Collections.Generic;

namespace B9PartSwitch
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> All<T>(this IEnumerable<T> enumerable)
        {
            foreach (T item in enumerable)
            {
                yield return item;
            }
        }

        public static Tcollection MaxBy<Tcollection, Tcompare>(this IEnumerable<Tcollection> enumerable, Func<Tcollection, Tcompare> mapper) where Tcompare : IComparable<Tcompare>
        {
            enumerable.ThrowIfNullArgument(nameof(enumerable));
            mapper.ThrowIfNullArgument(nameof(mapper));

            IEnumerator<Tcollection> enumerator = enumerable.GetEnumerator();

            if (!enumerator.MoveNext()) throw new InvalidOperationException("Enumerable is empty!");

            Tcollection result = enumerator.Current;
            Tcompare resultValue = mapper(result);

            while (enumerator.MoveNext())
            {
                Tcollection testResult = enumerator.Current;
                Tcompare testValue = mapper(testResult);
                if (testValue.CompareTo(resultValue) <= 0) continue;
                result = testResult;
                resultValue = testValue;
            }

            return result;
        }
    }
}
