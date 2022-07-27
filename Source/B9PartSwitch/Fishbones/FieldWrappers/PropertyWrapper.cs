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
using System.Reflection;

namespace B9PartSwitch.Fishbones.FieldWrappers
{
    public class PropertyWrapper : IFieldWrapper
    {
        private readonly PropertyInfo property;
        private readonly MethodInfo setMethod;
        private readonly MethodInfo getMethod;

        public PropertyWrapper(PropertyInfo property)
        {
            property.ThrowIfNullArgument(nameof(property));

            if (!property.CanRead || !property.CanWrite)
                throw new ArgumentException($"Property must have read and write accessors, however property {property.Name} of class {property.DeclaringType} does not", nameof(property));

            this.property = property;
            getMethod = property.GetGetMethod(true);
            setMethod = property.GetSetMethod(true);
        }

        public object GetValue(object subject)
        {
            subject.ThrowIfNullArgument(nameof(subject));

            return getMethod.Invoke(subject, null);
        }

        public void SetValue(object subject, object value)
        {
            subject.ThrowIfNullArgument(nameof(subject));

            setMethod.Invoke(subject, new[] { value } );
        }

        public string Name => property.Name;
        public Type FieldType => property.PropertyType;
        public MemberInfo MemberInfo => property;
    }
}
