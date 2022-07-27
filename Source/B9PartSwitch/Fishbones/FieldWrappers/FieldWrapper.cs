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
    public class FieldWrapper : IFieldWrapper
    {
        private readonly FieldInfo field;

        public FieldWrapper(FieldInfo field)
        {
            field.ThrowIfNullArgument(nameof(field));

            this.field = field;
        }

        public object GetValue(object subject)
        {
            subject.ThrowIfNullArgument(nameof(subject));

            return field.GetValue(subject);
        }

        public void SetValue(object subject, object value)
        {
            subject.ThrowIfNullArgument(nameof(subject));

            field.SetValue(subject, value);
        }

        public string Name => field.Name;
        public Type FieldType => field.FieldType;
        public MemberInfo MemberInfo => field;
    }
}
