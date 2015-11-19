using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Threading.Tasks;

namespace SelfHostedQuestionBot.DAL
{
    [DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    public class GameContext : DbContext
    {
        public GameContext()
            : base("standard")
        {

        }

        public DbSet<User> Users { get; set; }
    }
}
