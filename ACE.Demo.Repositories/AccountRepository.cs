﻿using ACE.Demo.Model.Accounts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace ACE.Demo.Repositories
{
    public class AccountRepository : BaseRepository, IAccountRepository
    {
        public Account Get(int id)
        {
            using (IDbConnection connection = OpenConnection())
            {
                return connection.Query<Account>("SELECT AccountId, Amount FROM ace_demo_account WHERE AccountId = @AccountId;",
                    new { AccountId = id }).SingleOrDefault();
            }
        }
    }
}
