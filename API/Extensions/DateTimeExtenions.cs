using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Extensions
{

    public static class DateTimeExtenions
    {
        public static int CalculateAge(this DateTime dob)
        {
            var today = DateTime.Today;
            var age = today.Year - dob.Year;

            // This is essentially if the user did not have their birthday yet
            if (dob.Date > today.AddYears(-age))
            {
                // If the user did not have their birthday yet we substract the age by 1
                age--;
            }
            return age;
        }
    }
}