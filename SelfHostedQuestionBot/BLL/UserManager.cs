using SelfHostedQuestionBot.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfHostedQuestionBot
{
    public class UserManager
    {
        private GameContext zGC = new GameContext();

        public User Authenticate(String pUsername, String pPassword)
        {
            var lUser = this.zGC.Users.FirstOrDefault(x => x.Username == pUsername && x.Password == pPassword);
            return lUser;
        }
    }
}
