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
using UnityEngine;

namespace B9PartSwitch
{
    public static class FatalErrorHandler
    {
        private const int MAX_MESSAGE_COUNT = 10;
        private static PopupDialog dialog;
        private static List<string> allMessages = new List<string>();

        public static void HandleFatalError(Exception exception)
        {
            try
            {
                string message = exception.Message;
                Exception innerException = exception.InnerException;
                while (innerException != null)
                {
                    message += "\n  ";
                    message += innerException.Message;
                    innerException = innerException.InnerException;
                }
                UpsertDialog(message);
            }
            catch (Exception ex)
            {
                Log.error(ex, "Exception while trying to create the fatal exception dialog");
                Application.Quit();
            }
        }

        private static void UpsertDialog(string message)
        {
            if (string.IsNullOrEmpty(message) || allMessages.Contains(message)) return;

            if (allMessages.Count < MAX_MESSAGE_COUNT)
            {
                allMessages.Add(message);
            }
            else if (allMessages.Count == MAX_MESSAGE_COUNT)
            {
                Log.error("[FatalExceptionHandler] Not displaying fatal error because too many errors have already been added:");
                Log.error(message);
                allMessages.Add("(too many error messages to display)");
            }
            else
            {
                Log.error("[FatalExceptionHandler] Not displaying fatal error because too many errors have already been added:");
                Log.error(message);
                return;
            }

            if (dialog != null) dialog.Dismiss();

            string errorMsg =  $"B9PartSwitch has encountered a fatal error and KSP needs to close.\n\n{string.Join("\n\n", allMessages.ToArray())}\n\nPlease see KSP's log for additional details";
            Log.error(errorMsg);
            dialog = PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new MultiOptionDialog(
                    "B9PartSwitchFatalError",
                   errorMsg,
                    "B9PartSwitch - Fatal Error",
                    HighLogic.UISkin,
                    new Rect(0.5f, 0.5f, 500f, 60f),
                    new DialogGUIFlexibleSpace(),
                    new DialogGUIHorizontalLayout(
                        new DialogGUIFlexibleSpace(),
                        new DialogGUIButton("Quit", Application.Quit, 140.0f, 30.0f, true),
                        new DialogGUIFlexibleSpace()
                    )
                ),
                true,
                HighLogic.UISkin);
        }
    }
}
