using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MessAidVOne.Application.Abstructions;
using Microsoft.AspNetCore.Http;

namespace MessAidVOne.Application.Features.AuthManagement
{
    public class ModifyUserCommand : CreateUserCommand, ICommand<Result<UserInformationDto>>
    {
        [Required(ErrorMessage = "Id is required.")]
        [Range(1, long.MaxValue, ErrorMessage = "Id must be a positive number.")]
        public long Id { get; set; }

        public bool IsPhotoRemove { get; set; } = false;

    }
}
