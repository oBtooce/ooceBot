using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Channels.ModifyChannelInformation;

namespace ooceBot.Commands
{
    public static class StreamCommandMethods
    {
        public static async Task<bool> SetStreamTitle(string newTitle, string channelId, TwitchAPI api)
        {
            var request = new ModifyChannelInformationRequest
            {
                Title = newTitle
            };

            await api.Helix.Channels.ModifyChannelInformationAsync(channelId, request);

            return true;
        }
    }
}
