using System.Collections.Generic;
using AutoMapper.Internal;
using BaseCamp_Web_API.Api.Requests;
using BaseCamp_Web_API.Api.Validators.ErrorMessages;
using BaseCamp_WEB_API.Core.Filters;
using BaseCamp_WEB_API.Core.RepositoryAbstractions;
using FluentValidation;

namespace BaseCamp_Web_API.Api.Validators
{
    /// <summary>
    /// Validator for data received from user sneaker-related requests.
    /// </summary>
    public class SneakerValidator : AbstractValidator<CreateSneakerRequest>
    {
        private readonly IVendorRepository _vendorRepository;
        private readonly ISeasonRepository _seasonRepository;

        private readonly List<int> _vendorsIds = new ();
        private readonly List<int> _seasonsIds = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="SneakerValidator"/> class.
        /// </summary>
        /// <param name="vendorRepository">For accessing data about vendors.</param>
        /// <param name="seasonRepository">For accessing data about seasons.</param>
        public SneakerValidator(IVendorRepository vendorRepository,
            ISeasonRepository seasonRepository)
        {
            _vendorRepository = vendorRepository;
            _seasonRepository = seasonRepository;

            Init();

            RuleFor(s => s.VendorId)
                .Must(o => _vendorsIds.Contains(o))
                .WithMessage(SneakerValidatorMessages.VendorIdNotFound);

            RuleFor(s => s.ModelName)
                .NotEmpty()
                .WithMessage(SneakerValidatorMessages.ModelNameIsEmpty);

            RuleFor(s => s.SeasonId)
                .Must(s => _seasonsIds.Contains(s))
                .WithMessage(SneakerValidatorMessages.SeasonIdNotFound);

            RuleFor(s => s.GenderId)
                .InclusiveBetween(1, 3)
                .WithMessage(SneakerValidatorMessages.GenderIdNotFound);

            RuleFor(s => s.Price)
                .GreaterThan(0)
                .WithMessage(SneakerValidatorMessages.PriceIsZeroOrLess);
        }

        private async void Init()
        {
            (await _vendorRepository.GetAllAsync(new PaginationFilter())).ForAll(v => _vendorsIds.Add(v.Id));
            (await _seasonRepository.GetAllAsync(new PaginationFilter())).ForAll(s => _seasonsIds.Add(s.Id));
        }
    }
}