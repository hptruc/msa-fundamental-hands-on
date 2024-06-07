﻿// <auto-generated />
using System;
using MSA.SagaOrchestrationStateMachine.Infrastructure.Saga;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MSA.SagaOrchestrationStateMachine.Migrations
{
    [DbContext(typeof(OrderStateDbContext))]
    partial class OrderStateDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.6");

            modelBuilder.Entity("MSA.SagaOrchestrationStateMachine.StateMachine.OrderState", b =>
                {
                    b.Property<Guid>("CorrelationId")
                        .HasColumnType("TEXT");

                    b.Property<string>("CurrentState")
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("PaymentId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Reason")
                        .HasColumnType("TEXT");

                    b.HasKey("CorrelationId");

                    b.ToTable("OrderState");
                });
#pragma warning restore 612, 618
        }
    }
}
