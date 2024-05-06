using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Edu.Abstraction.Models;

namespace Edu.Repository.Contexts
{
    public class EduDbContext : DbContext
    {
        public EduDbContext(DbContextOptions<EduDbContext> options)
             : base(options)
        { }

        public DbSet<AdmissionStatus> AdmissionStatuses { get; set; }
        public DbSet<Batch> Batches { get; set; }
        public DbSet<BatchUser> BatchUsers { get; set; }
        public DbSet<Intake> Intakes { get; set; }
        public DbSet<IntakeInstitute> IntakeInstitutes { get; set; }
        public DbSet<IntakeLocation> IntakeLocations { get; set; }
        public DbSet<SchemeInstitute> SchemeInstitutes { get; set; }
        public DbSet<IntakeTemplate> IntakeTemplates { get; set; }
        public DbSet<Scheme> Schemes { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Designation> Designations { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DurationType> DurationTypes { get; set; }
        public DbSet<Institute> Institutes { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<NominationInstitute> NominationInstitutes { get; set; }
        public DbSet<Nomination> Nominations { get; set; }
        public DbSet<EmailAccount> EmailAccounts { get; set; }
        public DbSet<NominationStatus> NominationStatuses { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserLogin> UserLogins { get; set; }
        public DbSet<IntakeDocumentType> IntakeDocumentTypes { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<Vertical> Verticals { get; set; }
        public DbSet<RoleDesignation> RoleDesignations { get; set; }
        public DbSet<Admission> Admissions { get; set; }
        public DbSet<AdmissionUser> AdmissionUsers { get; set; }
        public DbSet<Template> Templates { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
