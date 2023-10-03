using AutoMapper;
using IMMIWeb.Service.Models;


namespace IMMIWeb.Infrastructure
{
    public class MapperConfiguration : Profile
    {
        public MapperConfiguration()
        {
            //CreateMap<Agreement, AgreementViewModel>();
            //CreateMap<AgreementViewModel, Agreement>();

            //CreateMap<User, UserViewModel>();
            //CreateMap<UserViewModel, User>();

            CreateMap<Consultant, ConsultantViewModel>();
            CreateMap<ConsultantViewModel, Consultant>();

            CreateMap<TypeOfService, TypeOfServiceViewModel>();
            CreateMap<TypeOfServiceViewModel, TypeOfService>();

            CreateMap<Country, CountryViewModel>();
            CreateMap<CountryViewModel, Country>();

            CreateMap<Country, CommonListViewModel>();
            CreateMap<CommonListViewModel, Country>();

            CreateMap<Language, CommonListViewModel>();
            CreateMap<CommonListViewModel, Language>();

            CreateMap<TypeOfService, CommonListViewModel>();
            CreateMap<CommonListViewModel, TypeOfService>();

            CreateMap<Cm, CMSViewModel>();
            CreateMap<CMSViewModel, Cm>();

            CreateMap<RatingReviewConsultant, ConsultantReviewViewModel>();
            CreateMap<ConsultantReviewViewModel, RatingReviewConsultant>();

            CreateMap<AppLanguage, CommonListViewModel>();
            CreateMap<CommonListViewModel, AppLanguage>();

        }
    }
}
