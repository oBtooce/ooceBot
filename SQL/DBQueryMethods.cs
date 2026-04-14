using Microsoft.Data.Sqlite;
using ooceBot.Commands;
using ooceBot.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using TwitchLib.Api.Core.Models.Undocumented.Chatters;
using TwitchLib.Client.Models;

namespace ooceBot.SQL
{
    public static class DBQueryMethods
    {
        public static void UpdateChatterDataPlusMaybeTheme(CommandArgs args)
        {
            Chatter existingChatter = args.Context.Chatters.FirstOrDefault(chatter => chatter.Id == args.ChatMessage.UserId);
            
            if (existingChatter is null)
            {
                Chatter newChatter = new Chatter
                {
                    Id = args.ChatMessage.UserId,
                    DisplayName = args.ChatMessage.DisplayName,
                    HasTheme = 0,
                    HasChattedThisStream = 1
                };

                Program.dbContext.Chatters.Add(newChatter);
                Program.dbContext.SaveChanges();
            }

            // Todo: add logic for theme stuff
        }
    }
}
