using BussinessUnitFileProcAPI.Entities;
using BussinessUnitFileProcAPI.Models;
using FluentValidation;

namespace BussinessUnitFileProcAPI.Validators;
    public class BussinessUnitValidator : AbstractValidator<BussinessUnitEntity>
    {
        public BussinessUnitValidator()
        {
            RuleFor(x => x.BusinessUnit)
                .NotNull()
                .NotEmpty()
                .WithErrorCode("EmptyBussinessUnit")
                .WithMessage("Bussiness Unit Should not be null or empty.")
                .IsEnumName(typeof(ValidBussinessUnit), caseSensitive: false)
                .WithErrorCode("InvalidBussinessUnit")
                .WithMessage("Bussiness Unit name is not a valid.");
            
            RuleForEach(bu => bu.Attributes)
                .NotNull()
                .NotEmpty();

            RuleFor(p => p.expiryDate)
                .GreaterThanOrEqualTo(p => DateTime.Now.Date)
                .WithErrorCode("InvalidExpiryDate")
                .WithMessage("Expiry Date should not be a past date.");

            RuleFor(x => x.Files)
                .Must(x => x.Count > 0)
                .WithMessage("⦁\tAtleast one file should be exists in batch request body data")
                .WithErrorCode("InValidFileRequest")
                .ForEach(FileRule =>
                {
                    FileRule.Where(v => v.Attributes.Count() > 0).NotNull().NotEmpty();
                });

        }
    }
