using System.ComponentModel;
using System.Reflection;

namespace SharedKernel.Extensions
{
    public static class RecruitmentStatusExtension
    {
        public static string ToFriendlyString(int value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? value.ToString();
        }
    }
}