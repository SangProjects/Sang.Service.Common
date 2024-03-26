using Sang.Security.WebApi.Models;
using FluentValidation;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;

namespace Sang.Service.Common.Validators
{
    public class FileUploadValidator : AbstractValidator<IFormFile>
    {      
        private readonly FileUploadSettings _fileUploadSettings;
        public FileUploadValidator(IOptions<FileUploadSettings> fileUploadSettings)
        {
            _fileUploadSettings = fileUploadSettings.Value;

            RuleFor(request => request).NotNull().WithMessage("File is required.");
            RuleFor(request => request.FileName)
                .Must(HasAllowedExtension)
                .WithMessage($"Invalid file type. Only {string.Join(", ", _fileUploadSettings.AllowedFileExtensions)} files are allowed.");
        }

        private bool HasAllowedExtension(string fileName)
        {
            var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
            return _fileUploadSettings.AllowedFileExtensions.Contains(fileExtension);
            //return _fileUploadSettings.AllowedFileExtensions
            //.Any(ext => ext.ToLowerInvariant() == fileExtension); //TODO:Verify lower varient working
        }
    }

   
}
