using apiBozzi.Models.Enums;
using apiBozzi.Models.FelicianoBozzi;
using apiBozzi.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace apiBozzi.Services.FelicianoBozzi;

public class PaymentService(IServiceProvider serviceProvider) : ServiceBase(serviceProvider)
{
    #region Main

    public async Task<ContractResponse?> CreatePaymentsByContract(int id)
    {
        var con = await Context.Contracts
            .FirstOrDefaultAsync(x => x.Id == id && x.Status == StatusContract.Active);

        if (con is null) return null;

        var payments = await Context.Payments.Where(x => x.Contract.Id == con.Id)
            .ToListAsync();

        var currentMonth = new DateTime(con.ValidSince.Year, con.ValidSince.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endMonth = new DateTime(con.ValidUntil.Year, con.ValidUntil.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        while (currentMonth <= endMonth)
        {
            int daysRented = GetDaysRentedInMonth(currentMonth, con.ValidSince, con.ValidUntil);

            if (daysRented <= 0)
            {
                currentMonth = currentMonth.AddMonths(1);
                continue;
            }

            var alreadyExists = payments.Any(p =>
                p.Competence.Year == currentMonth.Year &&
                p.Competence.Month == currentMonth.Month);

            if (!alreadyExists)
            {
                decimal amountToAsk = CalculateProportionalRent(daysRented, currentMonth, con.Rent);
                var competenceDate = GetCompetenceDate(currentMonth, con.PaymentDay);

                Context.Payments.Add(new Payment
                {
                    Contract = con,
                    Competence = competenceDate,
                    Status = StatusPayment.NotPaid,
                    Type = PaymentType.None,
                    Value = amountToAsk
                });
            }

            currentMonth = currentMonth.AddMonths(1);
        }

        await Context.SaveChangesAsync();

        return new ContractResponse(con);
    }

    public async Task CleanUpFutureUnpaidPayments(int contractId, DateTime cutoffDate)
    {
        var paymentsToDelete = await Context.Payments
            .Where(p => p.Contract.Id == contractId 
                     && p.Status == StatusPayment.NotPaid
                     && (p.Competence.Year > cutoffDate.Year || 
                        (p.Competence.Year == cutoffDate.Year && p.Competence.Month > cutoffDate.Month)))
            .ToListAsync();

        if (paymentsToDelete.Count > 0)
        {
            Context.Payments.RemoveRange(paymentsToDelete);
            await Context.SaveChangesAsync();
        }
    }

    #endregion

    #region Private

    private int GetDaysRentedInMonth(DateTime currentMonth, DateTime validSince, DateTime validUntil)
    {
        var monthStart = currentMonth;
        var monthEnd = currentMonth.AddMonths(1);

        var periodStart = validSince.Date > monthStart.Date ? validSince.Date : monthStart.Date;
        var periodEnd = validUntil.Date < monthEnd.Date ? validUntil.Date : monthEnd.Date;

        return (periodEnd - periodStart).Days;
    }

    private decimal CalculateProportionalRent(int daysRented, DateTime currentMonth, decimal fullRent)
    {
        int daysInMonth = DateTime.DaysInMonth(currentMonth.Year, currentMonth.Month);
        return daysRented == daysInMonth 
            ? fullRent 
            : Math.Round((fullRent / daysInMonth) * daysRented, 2);
    }

    private DateTime GetCompetenceDate(DateTime currentMonth, int contractPaymentDay)
    {
        int daysInMonth = DateTime.DaysInMonth(currentMonth.Year, currentMonth.Month);
        int pDay = contractPaymentDay > 0 ? contractPaymentDay : 1;
        int paymentDay = Math.Min(pDay, daysInMonth);
        
        return new DateTime(currentMonth.Year, currentMonth.Month, paymentDay, 0, 0, 0, DateTimeKind.Utc);
    }

    #endregion
}