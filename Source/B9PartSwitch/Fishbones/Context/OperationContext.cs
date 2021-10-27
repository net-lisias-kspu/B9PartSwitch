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
namespace B9PartSwitch.Fishbones.Context
{
    public class OperationContext
    {
        public readonly Operation Operation;
        public readonly OperationContext ParentOperation;
        public readonly object Subject;

        public OperationContext(Operation operation, object subject)
        {
            subject.ThrowIfNullArgument(nameof(subject));

            Operation = operation;
            Subject = subject;
        }

        public OperationContext(OperationContext parentOperation, object subject)
        {
            parentOperation.ThrowIfNullArgument(nameof(parentOperation));
            subject.ThrowIfNullArgument(nameof(subject));

            ParentOperation = parentOperation;
            Operation = ParentOperation.Operation;
            Subject = subject;
        }

        public object Parent => ParentOperation?.Subject;

        public object Root => ParentOperation?.Root ?? Subject;
    }
}
