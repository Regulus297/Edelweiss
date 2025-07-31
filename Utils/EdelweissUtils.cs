namespace Edelweiss.Utils
{
    public static class EdelweissUtils
    {
        public static string CamelCaseToText(this string input)
        {
            string output = "";
            foreach (char c in input)
            {
                if (char.IsUpper(c) && (output == "" || char.IsLower(output[^1])))
                {
                    output += " ";
                }
                output += c;
            }
            return char.ToUpper(output[0]) + output.Substring(1);
        }
    }
}