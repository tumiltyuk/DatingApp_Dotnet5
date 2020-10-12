using System;

namespace API.Extensions
{
    public static class DateTimeExtentions
    {
        public static int CalculateAge(this DateTime dob)
        {
            var today = DateTime.Today;
            var age = today.Year - dob.Year;
            // check if dob DAY is greater than today or not - if not then reduce age by 1
            if (dob.Date > today.AddYears(-age)) age --;
            
            return age;
        }
    }
}