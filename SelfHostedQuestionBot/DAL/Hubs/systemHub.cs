using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfHostedQuestionBot.DAL.Hubs
{
    public class systemHub : Hub
    {
        [Authorize]
        public void getCurrentDate()
        {
            Clients.Caller.updateCurrentDate("[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "]");
        }

        /// <summary>
        /// Incase we need to find out if we are logged in
        /// </summary>
        /// <param name="pCallbackFunc"></param>
        /// <returns></returns>
        public void IsAuthenticated()
        {
            var lLoggedIn = false;
            if (Context.User != null)
            {
                if (Context.User.Identity.IsAuthenticated)
                {
                    lLoggedIn = true;
                }
            }

            Clients.Caller.updateLoginStatus(lLoggedIn);
        }
    }
}
