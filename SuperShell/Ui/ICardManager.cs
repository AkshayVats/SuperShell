﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperShell.Ui
{
    internal interface ICardManager
    {
        void AddEmptyCard();
        ShellCard GetLastCard();
    }
}
