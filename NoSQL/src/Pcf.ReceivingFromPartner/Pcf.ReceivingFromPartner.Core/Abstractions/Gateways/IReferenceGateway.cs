using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pcf.ReceivingFromPartner.Core.Domain;

namespace Pcf.ReceivingFromPartner.Core.Abstractions.Gateways
{
    public interface IReferenceGateway
    {
        Task<List<Preference>> GetPreferenses();
        Task<Preference> GetPreferenceByIdAsync(Guid preferenceId);
    }
}