﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Project
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class EuroleagueEntities3 : DbContext
    {
        public EuroleagueEntities3()
            : base("name=EuroleagueEntities3")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Evroliga> Evroligas { get; set; }
        public virtual DbSet<Hala> Halas { get; set; }
        public virtual DbSet<Igrac> Igracs { get; set; }
        public virtual DbSet<IgracIgra> IgracIgras { get; set; }
        public virtual DbSet<Klub> Klubs { get; set; }
        public virtual DbSet<Nalog> Nalogs { get; set; }
        public virtual DbSet<Rezervacija> Rezervacijas { get; set; }
        public virtual DbSet<Sudi> Sudis { get; set; }
        public virtual DbSet<Sudija> Sudijas { get; set; }
        public virtual DbSet<Trener> Treners { get; set; }
        public virtual DbSet<Utakmica> Utakmicas { get; set; }
    
        public virtual int DateValidationReservation(Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate, string hallId, ObjectParameter isOk)
        {
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("startDate", startDate) :
                new ObjectParameter("startDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("endDate", endDate) :
                new ObjectParameter("endDate", typeof(System.DateTime));
    
            var hallIdParameter = hallId != null ?
                new ObjectParameter("hallId", hallId) :
                new ObjectParameter("hallId", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("DateValidationReservation", startDateParameter, endDateParameter, hallIdParameter, isOk);
        }
    
        public virtual int DateValidationReservationModify(Nullable<System.DateTime> startDate, Nullable<System.DateTime> endDate, string oldRezId, string oldHalaOzn, ObjectParameter isOk)
        {
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("startDate", startDate) :
                new ObjectParameter("startDate", typeof(System.DateTime));
    
            var endDateParameter = endDate.HasValue ?
                new ObjectParameter("endDate", endDate) :
                new ObjectParameter("endDate", typeof(System.DateTime));
    
            var oldRezIdParameter = oldRezId != null ?
                new ObjectParameter("oldRezId", oldRezId) :
                new ObjectParameter("oldRezId", typeof(string));
    
            var oldHalaOznParameter = oldHalaOzn != null ?
                new ObjectParameter("oldHalaOzn", oldHalaOzn) :
                new ObjectParameter("oldHalaOzn", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("DateValidationReservationModify", startDateParameter, endDateParameter, oldRezIdParameter, oldHalaOznParameter, isOk);
        }
    
        public virtual int Stats(string playerId, ObjectParameter name, ObjectParameter games, ObjectParameter pts, ObjectParameter @as, ObjectParameter reb)
        {
            var playerIdParameter = playerId != null ?
                new ObjectParameter("playerId", playerId) :
                new ObjectParameter("playerId", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Stats", playerIdParameter, name, games, pts, @as, reb);
        }
    }
}
