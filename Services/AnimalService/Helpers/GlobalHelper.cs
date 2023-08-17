using System.Text.RegularExpressions;
using shortid;
using shortid.Configuration;

namespace AnimalService.Helpers
{
    public class GlobalHelper
    {
        public static string GenerateSlug(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // Convert to lowercase
            input = input.ToLower();

            // Remove invalid characters
            input = Regex.Replace(input, @"[^a-z0-9\s-]", "");

            // Replace spaces with hyphens
            input = input.Replace(" ", "-");

            // Collapse multiple hyphens
            input = Regex.Replace(input, @"-+", "-");

            // Trim leading and trailing hyphens
            input = input.Trim('-');

            return input;
        }

        public static string GenerateShortId()
        {
            return ShortId.Generate(new GenerationOptions(useNumbers: true, useSpecialCharacters: false));
        }

    }
}