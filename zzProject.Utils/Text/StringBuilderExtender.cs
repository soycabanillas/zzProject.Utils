using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zzProject.Utils.Text
{
    public static class StringBuilderExtender
    {
        public static StringBuilder AppendComma(this StringBuilder stringBuilder, ref bool isFirstElement, string value)
        {
            if (isFirstElement)
            {
                isFirstElement = false;
                return stringBuilder.Append(value);
            }
            else return stringBuilder.AppendLine(",").Append(value);
        }
        public static StringBuilder AppendFormatComma(this StringBuilder stringBuilder, ref bool isFirstElement, string format, object arg)
        {
            if (isFirstElement)
            {
                isFirstElement = false;
                return stringBuilder.AppendFormat(format, arg);
            }
            else return stringBuilder.AppendLine(",").AppendFormat(format, arg);
        }
        public static StringBuilder AppendFormatComma(this StringBuilder stringBuilder, ref bool isFirstElement, string format, params object[] args)
        {
            if (isFirstElement)
            {
                isFirstElement = false;
                return stringBuilder.AppendFormat(format, args);
            }
            else return stringBuilder.AppendLine(",").AppendFormat(format, args);
        }
    }
}
