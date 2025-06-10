using System;
using System.Linq;
using Patient.Data;
using Patient.Models;

public static class DataSeeder
{
    public static void SeedPatients(ApplicationDbContext context)
    {
        if (!context.Patients.Any())
        {
            var patients = new[]
            {
                new Patient.Models.Patient
                {
                    Nom = "TestNone",
                    Prenom = "Test",
                    DateNaissance = new DateTime(1966, 12, 31),
                    Genre = "F",
                    AdressePostale = "1 Brookside St",
                    NumeroTelephone = "100-222-3333"
                },
                new Patient.Models.Patient
                {
                    Nom = "TestBorderline",
                    Prenom = "Test",
                    DateNaissance = new DateTime(1945, 6, 24),
                    Genre = "M",
                    AdressePostale = "2 High St",
                    NumeroTelephone = "200-333-4444"
                },
                new Patient.Models.Patient
                {
                    Nom = "TestInDanger",
                    Prenom = "Test",
                    DateNaissance = new DateTime(2004, 6, 18),
                    Genre = "M",
                    AdressePostale = "3 Club Road",
                    NumeroTelephone = "300-444-5555"
                },
                new Patient.Models.Patient
                {
                    Nom = "TestEarlyOnset",
                    Prenom = "Test",
                    DateNaissance = new DateTime(2002, 6, 28),
                    Genre = "F",
                    AdressePostale = "4 Valley Dr",
                    NumeroTelephone = "400-555-6666"
                }
            };

            context.Patients.AddRange(patients);
            context.SaveChanges();
        }
    }
}
