using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LSports.Scheduler
{
    public class Sports
    {
		// Hardcoded: all kind of sports
        public static List<Sport> sports = new List<Sport>()
        {
            new Sport() {Id = 35232 },
            new Sport() {Id = 46957 },
            new Sport() {Id = 530129 },
            new Sport() {Id = 307126 },
            new Sport() {Id = 452674 },

            new Sport() {Id = 274791 },
            new Sport() {Id = 274792 },
            new Sport() {Id = 261354 },
            new Sport() {Id = 54094 },
            new Sport() {Id = 687887 },

            new Sport() {Id = 687888 },
            new Sport() {Id = 687889 },
            new Sport() {Id = 687890 },
            new Sport() {Id = 48242 },
            new Sport() {Id = 291987 },

            new Sport() {Id = 687893 },
            new Sport() {Id = 687894 },
            new Sport() {Id = 687895 },
            new Sport() {Id = 687896 },
            new Sport() {Id = 687897 },

            new Sport() {Id = 388764 },
            new Sport() {Id = 262622 },
            new Sport() {Id = 621569 },
            new Sport() {Id = 389537 },
            new Sport() {Id = 154914 },

            new Sport() {Id = 1149093 },
            new Sport() {Id = 154919 },
            new Sport() {Id = 154923 },
            new Sport() {Id = 154830 },
            new Sport() {Id = 165874 },

            new Sport() {Id = 131506 },
            new Sport() {Id = 35706 },
            new Sport() {Id = 265917 },
            new Sport() {Id = 35709 },
            new Sport() {Id = 6046 }
        };
    }

    public class Sport
    {
        public int Id { get; set; }
    }
}