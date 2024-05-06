using Core.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Edu.Repository.Ioc;
using Edu.Service.Interfaces;
using Edu.Service.Services;

namespace Edu.Service.Ioc
{
    public static class IocService
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterRepositories(configuration);

            #region Services

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<ITokenBuilder, TokenBuilder>();
            services.AddTransient<IUserProviderService, UserProviderService>();
            services.AddTransient<IFileService, FileService>();
            services.AddTransient<ISmtpBuilder, SmtpBuilder>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IHangfireService, HangfireService>();

            services.AddTransient<IAdmissionStatusService, AdmissionStatusService>();
            services.AddTransient<IBatchService, BatchService>();
            services.AddTransient<IBatchUserService, BatchUserService>();
            services.AddTransient<IIntakeService, IntakeService>();
            services.AddTransient<IIntakeInstituteService, IntakeInstituteService>();
            services.AddTransient<IIntakeLocationService, IntakeLocationService>();
            services.AddTransient<IIntakeTemplateService, IntakeTemplateService>();
            services.AddTransient<ISchemeInstituteService, SchemeInstituteService>();
            services.AddTransient<ISchemeService, SchemeService>();
            services.AddTransient<IDepartmentService, DepartmentService>();
            services.AddTransient<IDesignationService, DesignationService>();
            services.AddTransient<IDivisionService, DivisionService>();
            services.AddTransient<IDocumentService, DocumentService>();
            services.AddTransient<IDurationTypeService, DurationTypeService>();
            services.AddTransient<IInstituteService, InstituteService>();
            services.AddTransient<ILocationService, LocationService>();
            services.AddTransient<INominationInstituteService, NominationInstituteService>();
            services.AddTransient<INominationService, NominationService>();
            services.AddTransient<IEmailAccountService, EmailAccountService>();
            services.AddTransient<INominationStatusService, NominationStatusService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<ISettingService, SettingService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IUserLoginService, UserLoginService>();
            services.AddTransient<IIntakeDocumentTypeService, IntakeDocumentTypeService>();
            services.AddTransient<IDocumentTypeService, DocumentTypeService>();
            services.AddTransient<IVerticalService, VerticalService>();
            services.AddTransient<IRoleDesignationService, RoleDesignationService>();
            services.AddTransient<IAdmissionService, AdmissionService>();
            services.AddTransient<IAdmissionUserService, AdmissionUserService>();
            services.AddTransient<ITemplateService, TemplateService>();

            #endregion
        }
    }
}
