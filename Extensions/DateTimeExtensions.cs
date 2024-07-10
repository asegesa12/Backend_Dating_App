namespace API.Extensions
{
    public static class DateTimeExtensions
    {
        public static int CalculateAge(this DateOnly DateOfBirth)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var age = today.Year - DateOfBirth.Year;

            if (DateOfBirth > today.AddYears(-age)) age--;

            return age;
        }
    }
}
