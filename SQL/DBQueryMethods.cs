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
        /// <summary>
        /// Upserts the chatter row for the message sender.
        /// </summary>
        /// <returns>True if this is the first message this chatter has sent since the stream went online.</returns>
        public static bool UpdateChatterDataPlusMaybeTheme(CommandArgs args)
        {
            Chatter existingChatter = args.Context.Chatters.FirstOrDefault(chatter => chatter.Id == args.ChatMessage.UserId);

            bool isFirstMessageThisStream;

            if (existingChatter is null)
            {
                Chatter newChatter = new Chatter
                {
                    Id = args.ChatMessage.UserId,
                    DisplayName = args.ChatMessage.DisplayName,
                    HasTheme = 0,
                    LastChattedStreamDate = BotVariables.StreamStartTime
                };

                Program.dbContext.Chatters.Add(newChatter);
                Program.dbContext.SaveChanges();

                isFirstMessageThisStream = true;
            }
            else
            {
                // Compared against the current stream's actual start time (not a boolean flag) so that
                // a bot restart mid-stream doesn't re-fire first-message-of-stream logic (e.g. shoutouts)
                isFirstMessageThisStream = existingChatter.LastChattedStreamDate != BotVariables.StreamStartTime;

                if (isFirstMessageThisStream)
                {
                    existingChatter.LastChattedStreamDate = BotVariables.StreamStartTime;
                    Program.dbContext.SaveChanges();
                }
            }

            // Todo: add logic for theme stuff

            return isFirstMessageThisStream;
        }
    }
}
