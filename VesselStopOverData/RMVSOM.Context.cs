﻿//------------------------------------------------------------------------------
// <auto-generated>
//    Ce code a été généré à partir d'un modèle.
//
//    Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//    Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VesselStopOverData
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class RM_VSOMEntities : DbContext
    {
        public RM_VSOMEntities()
            : base("name=RM_VSOMEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<acces> acces { get; set; }
        public DbSet<compte_rule> compte_rule { get; set; }
        public DbSet<comptes> comptes { get; set; }
        public DbSet<entreprises> entreprises { get; set; }
        public DbSet<log> log { get; set; }
        public DbSet<quotation> quotation { get; set; }
        public DbSet<requetes> requetes { get; set; }
        public DbSet<rules> rules { get; set; }
    }
}
