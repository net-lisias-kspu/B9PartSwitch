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
using System.Text.RegularExpressions;

namespace B9PartSwitch.Utils
{
    public static class StringMatcher
    {
        public static IStringMatcher Parse(string str)
        {
            str.ThrowIfNullArgument(nameof(str));

            if (str.Length > 2 && str[0] == '/' && str[str.Length - 1] == '/')
            {
                Regex regex = new Regex(str.Substring(1, str.Length - 2));
                return new RegexStringMatcher(regex);
            }

            if (str.Length > 2 && str[0] == '\\' && str[1] == '/') str = str.Substring(1);

            if (str.IndexOf('*') != -1 || str.IndexOf('?') != -1)
            {
                str = Regex.Escape(str).Replace("\\*", ".*").Replace("\\?", ".");
                Regex regex = new Regex('^' + str + '$');
                return new RegexStringMatcher(regex);
            }

            return new NormalStringMatcher(str);
        }
    }

    public interface IStringMatcher
    {
        bool Match(string testMatch);
    }

    public class NormalStringMatcher : IStringMatcher
    {
        private readonly string str;

        public NormalStringMatcher(string str)
        {
            this.str = str ?? throw new ArgumentNullException(nameof(str));
        }

        public bool Match(string testMatch)
        {
            testMatch.ThrowIfNullArgument(nameof(testMatch));

            return testMatch == str;
        }

        public override string ToString()
        {
            if (str.Length > 2 && str[0] == '/' && str[str.Length - 1] == '/')
                return '\\' + str;
            else
                return str;
        }
    }

    public class RegexStringMatcher : IStringMatcher
    {
        private readonly Regex regex;

        public RegexStringMatcher(Regex regex)
        {
            this.regex = regex ?? throw new ArgumentNullException(nameof(regex));
        }

        public bool Match(string testMatch)
        {
            testMatch.ThrowIfNullArgument(nameof(testMatch));

            return regex.IsMatch(testMatch);
        }

        public override string ToString() => $"/{regex}/";
    }
}
