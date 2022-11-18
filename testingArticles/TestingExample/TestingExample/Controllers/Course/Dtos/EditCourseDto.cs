using FluentValidation;
using System;

namespace TestingExample.Controllers.Course.Dtos
{
    public record EditCourseDto(
        string? Title,
        int? Credits,
        Guid? DepartmentId);

    public class EditCourseDtoValidator : AbstractValidator<EditCourseDto>
    {
        public EditCourseDtoValidator()
        {
            When(x => x.Credits != null, () =>
            {
                RuleFor(x => x.Credits).InclusiveBetween(0, 5);
            });
            When(x => x.Title != null, () =>
            {
                RuleFor(x => x.Title).MinimumLength(3).MaximumLength(50);
            });
        }
    }

}
