using System.Text;

public class StringUtils
{
    static StringBuilder stringBuilder = new StringBuilder();

    public static string Format(string format, params object[] args) => stringBuilder.Clear().AppendFormat(format, args).ToString();
    public static string Join(string seperator, params object[] args) => stringBuilder.Clear().AppendJoin(seperator, args).ToString();
}
