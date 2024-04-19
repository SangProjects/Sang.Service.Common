using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Sang.Service.Common.Extension;
using Sang.Service.Common.Models;
using System.Linq;

namespace Sang.Service.Common.Validators
{
    public class FileUploadValidator : AbstractValidator<IFormFile>
    {
        private readonly IApiSettings _apiSettings;
        public FileUploadValidator(IApiSettings apiSettings)
        {
            _apiSettings = apiSettings;

            RuleFor(request => request).NotNull().WithMessage("File is required.");
            RuleFor(request => request.FileName)
                .Must(HasAllowedExtension)
                .WithMessage($"Invalid file type. Only {string.Join(", ", _apiSettings.AllowedFileExtensions)} files are allowed.");
        }

        private bool HasAllowedExtension(string fileName)
        {
            var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
            return _apiSettings.AllowedFileExtensions.Contains(fileExtension);

            // NOTE: Uncomment if required
            //return _fileUploadSettings.AllowedFileExtensions
            //.Any(ext => ext.ToLowerInvariant() == fileExtension); //TODO:Verify lower varient working
        }
    }


}
