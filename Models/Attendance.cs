using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ooceBot.Models
{
    public class Attendance
    {
        public string Id { get; set; }

        public int AttendanceCount { get; set; }

        public int TotalAttendance { get; set; }

        public bool IsPresent { get; set; }

        public int PointsForRedemption { get; set; }

        public DateOnly? LastPresentDate { get; set; }
    }
}
