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
using UnityEngine;

namespace B9PartSwitch
{
    public static class TypeExtensions
    {
        public static bool Implements(this Type self, Type other)
        {
            return other.IsAssignableFrom(self);
        }

        public static bool Implements<T>(this Type self) => self.Implements(typeof(T));

        public static bool IsUnitySerializableType(this Type t)
        {
            if (t == null)
                return false;
            if (t.IsPrimitive) return true;
            if (t == typeof(string)) return true;
            if (t.IsSubclassOf(typeof(UnityEngine.Object))) return true;
            if (t.IsListType() && t.GetGenericArguments()[0].IsUnitySerializableType()) return true;
            if (t.IsArray && t.GetElementType().IsUnitySerializableType()) return true;
            if (t.IsEnum) return true;
            if (t.IsSerializable) return true;

            // Unity serializable types
            if (t == typeof(Vector2)) return true;
            if (t == typeof(Vector3)) return true;
            if (t == typeof(Vector4)) return true;
            if (t == typeof(Quaternion)) return true;
            if (t == typeof(Matrix4x4)) return true;
            if (t == typeof(Color)) return true;
            if (t == typeof(Rect)) return true;
            if (t == typeof(LayerMask)) return true;

            return false;
        }

        public static bool IsNullableValueType(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}
