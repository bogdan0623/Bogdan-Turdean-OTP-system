﻿using System.Text.RegularExpressions;

namespace OTPBackend.Services
{
    public class ValidateService
    {
        public static bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            return Regex.IsMatch(email, pattern);
        }
    }
}
