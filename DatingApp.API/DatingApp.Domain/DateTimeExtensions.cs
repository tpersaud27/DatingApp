using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Services.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// This is a simple implementation of calculating age
        /// </summary>
        /// <param name="dateOfBirth"></param>
        /// <returns></returns>
        public static int CalculateAge(this DateOnly dateOfBirth)
        {
            // Todays date
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            // Calculate the age
            var age = today.Year - dateOfBirth.Year;
            
            if(dateOfBirth > today.AddYears(-age)) {
                age--;
            }

            return age;
        }
    }
}
