﻿using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Common;
using Ordering.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Persistence
{
    public class OrderContext : DbContext
    {
        public OrderContext(DbContextOptions<OrderContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<EntityBase>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.Now;
                        entry.Entity.CreatedBy = "roc";
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedDate = DateTime.Now;
                        entry.Entity.LastModifiedBy = "roc";
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}