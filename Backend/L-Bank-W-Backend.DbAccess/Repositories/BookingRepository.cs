﻿using System.Data.SqlClient;
using System.Data;


namespace L_Bank_W_Backend.Models
{
    public class BookingRepository : IBookingRepository
    {
        public bool Book(int sourceLedgerId, int destinationLKedgerId, decimal amount)
        {
            // Machen Sie eine Connection und eine Transaktion

            // In der Transaktion:

            // Schauen Sie ob genügend Geld beim Spender da ist
            // Führen Sie die Buchung durch und UPDATEn Sie die ledgers
            // Beenden Sie die Transaktion
            // Bei einem Transaktionsproblem: Restarten Sie die Transaktion in einer Schleife 
            // (Siehe LedgersModel.SelectOne)

            return false; // Lösch mich
        }


    }
}
