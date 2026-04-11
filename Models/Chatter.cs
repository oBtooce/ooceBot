using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ooceBot.Models
{
    public class Chatter
    {
        public string Id { get; set; }

        public string DisplayName { get; set; }

        public int HasTheme { get; set; }

        public int HasChattedThisStream { get; set; }
    }
}
