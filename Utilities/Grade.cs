using System.ComponentModel.DataAnnotations;

namespace TadrousManassa.Utilities
{
    public enum Grade
    {
        [Display(Name = "Fifth Primary")]
        FifthPrimary = 5,

        [Display(Name = "Sixth Primary")]
        SixthPrimary = 6,

        [Display(Name = "First Preparatory")]
        FirstPreparatory = 1,

        [Display(Name = "Second Preparatory")]
        SecondPreparatory = 2,

        [Display(Name = "Third Preparatory")]
        ThirdPreparatory = 3
    }
}