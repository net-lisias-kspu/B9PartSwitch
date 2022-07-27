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

namespace B9PartSwitch.Utils
{
    public class ChangeTransactionManager
    {
        private enum TransactionState
        {
            PreInitialize,
            OutsideTransactionNoChangeNeeded,
            InTransactionNoChangeNeeded,
            InTransactionChangeNeeded,
            AfterTransactionChanging,
        }

        private TransactionState state = TransactionState.PreInitialize;
        private readonly Action change;

        public ChangeTransactionManager(Action change)
        {
            this.change = change ?? throw new ArgumentNullException(nameof(change));
        }

        public void Initialize()
        {
            if (state != TransactionState.PreInitialize) return;
            state = TransactionState.OutsideTransactionNoChangeNeeded;
        }

        public void RequestChange()
        {
            if (state == TransactionState.PreInitialize) return;

            if (state == TransactionState.InTransactionNoChangeNeeded)
            {
                state = TransactionState.InTransactionChangeNeeded;
            }
            else if (state == TransactionState.OutsideTransactionNoChangeNeeded)
            {
                change();
            }
            else if (state == TransactionState.AfterTransactionChanging)
            {
                throw new InvalidOperationException("Circular change condition detected");
            }
        }

        public void WithTransaction(Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            if (state == TransactionState.OutsideTransactionNoChangeNeeded || state == TransactionState.PreInitialize)
                state = TransactionState.InTransactionNoChangeNeeded;

            try
            {
                action();

                if (state == TransactionState.InTransactionChangeNeeded)
                {
                    state = TransactionState.AfterTransactionChanging;
                    change();
                }
            }
            finally
            {
                state = TransactionState.OutsideTransactionNoChangeNeeded;
            }
        }
    }
}
