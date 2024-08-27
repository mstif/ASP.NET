using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pcf.GivingToCustomer.Core.Domain;


namespace Pcf.GivingToCustomer.Core.Abstractions.Gateways
{
    public interface IReferenceGateway
    {
        Task<List<Preference>> GetPreferences();
        Task<Preference> GetPreferenceByIdAsync(Guid preferenceId);
        Task<List<Preference>> GetPreferenceRangeByIdAsync(List<Guid> preferenceIds);
    }
}