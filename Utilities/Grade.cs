using System.ComponentModel.DataAnnotations;

namespace TadrousManassa.Utilities
{
    public enum Grade
    {
        [Display(Name = "Fifth primary")]
        FifthPrimary = 5,

        [Display(Name = "Sixth primary")]
        SixthPrimary = 6,

        [Display(Name = "First Preparatory")]
        FirstPreparatory = 1,

        [Display(Name = "Second Preparatory")]
        SecondPreparatory = 2,

        [Display(Name = "Third Preparatory")]
        ThirdPreparatory = 3
    }

    // Helper class to get display names
    public static class GradeHelper
    {
        public static string GetDisplayName(this Grade grade)
        {
            var fieldInfo = grade.GetType().GetField(grade.ToString());
            var attribute = fieldInfo?.GetCustomAttributes(typeof(DisplayAttribute), false)
                .FirstOrDefault() as DisplayAttribute;
            return attribute?.Name ?? grade.ToString();
        }
    }
}
