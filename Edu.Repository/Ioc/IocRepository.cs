using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edu.Repository.Contexts;
using Edu.Repository.Interfaces;
using Edu.Repository.Repositories;

namespace Edu.Repository.Ioc
{
    public static class IocRepository
    {
        public static void RegisterRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddEntityFrameworkSqlServer()
                    .AddDbContext<EduDbContext>(options => options.UseSqlServer(connectionString, sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    }),
            ServiceLifetime.Scoped//Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
            );

            #region Repositories

            services.AddTransient<IAdmissionStatusRepository, AdmissionStatusRepository>();
            services.AddTransient<IBatchRepository, BatchRepository>();
            services.AddTransient<IBatchUserRepository, BatchUserRepository>();
            services.AddTransient<IIntakeRepository, IntakeRepository>();
            services.AddTransient<IIntakeInstituteRepository, IntakeInstituteRepository>();
            services.AddTransient<IIntakeLocationRepository, IntakeLocationRepository>();
            services.AddTransient<IIntakeTemplateRepository, IntakeTemplateRepository>();
            services.AddTransient<ISchemeInstituteRepository, SchemeInstituteRepository>();
            services.AddTransient<ISchemeRepository, SchemeRepository>();
            services.AddTransient<IDepartmentRepository, DepartmentRepository>();
            services.AddTransient<IDesignationRepository, DesignationRepository>();
            services.AddTransient<IDivisionRepository, DivisionRepository>();
            services.AddTransient<IDocumentRepository, DocumentRepository>();
            services.AddTransient<IDurationTypeRepository, DurationTypeRepository>();
            services.AddTransient<IInstituteRepository, InstituteRepository>();
            services.AddTransient<ILocationRepository, LocationRepository>();
            services.AddTransient<INominationInstituteRepository, NominationInstituteRepository>();
            services.AddTransient<INominationRepository, NominationRepository>();
            services.AddTransient<IEmailAccountRepository, EmailAccountRepository>();
            services.AddTransient<INominationStatusRepository, NominationStatusRepository>();
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<ISettingRepository, SettingRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserLoginRepository, UserLoginRepository>();
            services.AddTransient<IIntakeDocumentTypeRepository, IntakeDocumentTypeRepository>();
            services.AddTransient<IDocumentTypeRepository, DocumentTypeRepository>();
            services.AddTransient<IVerticalRepository, VerticalRepository>();
            services.AddTransient<IRoleDesignationRepository, RoleDesignationRepository>();
            services.AddTransient<IAdmissionRepository, AdmissionRepository>();
            services.AddTransient<IAdmissionUserRepository, AdmissionUserRepository>();
            services.AddTransient<ITemplateRepository, TemplateRepository>();

            #endregion
        }
    }
}
