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
using System.Text;

namespace B9PartSwitch.Utils
{
    public class GroupedStringBuilder
    {
        private enum State
        {
            Initial,
            TextWritten,
            WaitingForNewline,
            WaitingForGroup,
        }

        private readonly StringBuilder stringBuilder = new StringBuilder();

        private State state = State.Initial;

        public void BeginGroup()
        {
            if (state != State.Initial) state = State.WaitingForGroup;
        }

        public void Append(string value)
        {
            CheckState();
            stringBuilder.Append(value);
            state = State.TextWritten;
        }

        public void Append(string value, object arg0)
        {
            CheckState();
            stringBuilder.AppendFormat(value, arg0);
            state = State.TextWritten;
        }

        public void Append(string format, object arg0, object arg1)
        {
            CheckState();
            stringBuilder.AppendFormat(format, arg0, arg1);
            state = State.TextWritten;
        }

        public void Append(string format, object arg0, object arg1, object arg2)
        {
            CheckState();
            stringBuilder.AppendFormat(format, arg0, arg1, arg2);
            state = State.TextWritten;
        }

        public void AppendLine()
        {
            CheckState();
            state = State.WaitingForNewline;
        }

        public void AppendLine(string value)
        {
            CheckState();
            stringBuilder.AppendFormat(value);
            state = State.WaitingForNewline;
        }

        public void AppendLine(string format, object arg0)
        {
            CheckState();
            stringBuilder.AppendFormat(format, arg0);
            state = State.WaitingForNewline;
        }

        public void AppendLine(string format, object arg0, object arg1)
        {
            CheckState();
            stringBuilder.AppendFormat(format, arg0, arg1);
            state = State.WaitingForNewline;
        }

        public override string ToString() => stringBuilder.ToString();

        public void Clear()
        {
            stringBuilder.Length = 0;
            state = State.Initial;
        }

        private void CheckState()
        {
            if (state == State.WaitingForGroup)
                stringBuilder.Append("\n\n");
            else if (state == State.WaitingForNewline)
                stringBuilder.Append("\n");
        }
    }
}
