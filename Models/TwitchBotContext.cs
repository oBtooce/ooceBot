using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ooceBot.Models
{
    public class TwitchBotContext : DbContext
    {
        public DbSet<Chatter> Chatters { get; set; }

        public DbSet<Attendance> AttendanceRecords { get; set; }

        public DbSet<ArcadeStats> ArcadeRecords { get; set; }

        public DbSet<DapStats> DapRecords { get; set; }

        public DbSet<Miscellaneous> GeneralStreamData { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite("Data Source=TwitchStats.db");
    }
}
