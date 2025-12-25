using System.ComponentModel.DataAnnotations;
using MessAidVOne.Application.Abstructions;

namespace MessAidVOne.Application.Features.MessManagement.Commands
{
    public class ModifyMessCommand : CreatMessCommand, ICommand<Result<MessInformationDto>>
    {
        [Required(ErrorMessage = "Id is required.")]
        [Range(1, long.MaxValue, ErrorMessage = "Id must be a positive number.")]
        public long Id { get; set; }

        public bool IsPhotoRemove { get; set; } = false;
    }
}
