﻿using Grit.ACE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACE.Demo.Contracts.Actions
{
    public class InvestmentPayRequest : InvestmentActionBase
    {
        public int InvestmentId { get; set; }
    }
}
