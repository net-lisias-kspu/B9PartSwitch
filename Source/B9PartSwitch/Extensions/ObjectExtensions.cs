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

namespace B9PartSwitch
{
    public static class ObjectExtensions
    {
        public static bool IsNull(this object o)
        {
            return (o == null);
        }

        public static bool IsNotNull(this object o)
        {
            return (o != null);
        }

        public static void ThrowIfNullArgument(this object o, string paramName)
        {
            if (o.IsNull()) throw new ArgumentNullException(paramName);
        }

        public static void EnsureArgumentType(this object o, Type type, string paramName)
        {
            if (o.IsNotNull() && !o.GetType().Implements(type)) throw new ArgumentException($"Expected parameter of type {type} but got {o.GetType()}", paramName);
        }

        public static void EnsureArgumentType<T>(this object o, string paramName) => o.EnsureArgumentType(typeof(T), paramName);

        public static void EnsureArgumentType<T1, T2>(this object o, string paramName)
        {
            if (o.IsNull()) return;
            Type t = o.GetType();

            bool validType = t.Implements<T1>() || t.Implements<T2>();

            if (!validType) throw new ArgumentException($"Expected parameter of type {typeof(T1)} or {typeof(T2)} but got {t}", paramName);
        }
    }
}
