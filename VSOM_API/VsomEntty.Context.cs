﻿//------------------------------------------------------------------------------
// <auto-generated>
//    Ce code a été généré à partir d'un modèle.
//
//    Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//    Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VSOM_API
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class VSOMEntities : DbContext
    {
        public VSOMEntities()
            : base("name=VSOMEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<ARTICLE> ARTICLEs { get; set; }
        public DbSet<AVOIR> AVOIRs { get; set; }
        public DbSet<CLIENT> CLIENTs { get; set; }
        public DbSet<CONNAISSEMENT> CONNAISSEMENTs { get; set; }
        public DbSet<ELEMENT_FACTURATION> ELEMENT_FACTURATION { get; set; }
        public DbSet<ESCALE> ESCALEs { get; set; }
        public DbSet<FACTURE> FACTUREs { get; set; }
        public DbSet<LIGNE_AVOIR> LIGNE_AVOIR { get; set; }
        public DbSet<LIGNE_PRIX> LIGNE_PRIX { get; set; }
        public DbSet<LIGNE_PROFORMA> LIGNE_PROFORMA { get; set; }
        public DbSet<PAYEMENT> PAYEMENTs { get; set; }
        public DbSet<PROFORMA> PROFORMAs { get; set; }
        public DbSet<VEHICULE> VEHICULEs { get; set; }
    }
}