using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GMS.Identity.WebHost.Cache;
using Microsoft.AspNetCore.Mvc;
using ReferenceInfo.WebHost.Models;
using ReferenceInfoCore.Domain;
using ReferenceInfoCore.Repositories;

namespace ReferenceInfo.WebHost.Controllers
{
    /// <summary>
    /// Предпочтения клиентов
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PreferencesController
        : ControllerBase
    {
        private readonly IRepository<Preference> _preferencesRepository;
        private readonly ICacheService _cacheService;

        public PreferencesController(IRepository<Preference> preferencesRepository, ICacheService cacheService)
        {
            _preferencesRepository = preferencesRepository;
            _cacheService = cacheService;

        }

        /// <summary>
        /// Получить список предпочтений
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<PreferenceResponse>>> GetPreferencesAsync()
        {

            var cacheData = _cacheService.GetData<IEnumerable<Preference>>("preferences");
            if (cacheData != null)
            {
                var resFromCache = cacheData.ToList();
                Response.Headers.Append("Access-Control-Expose-Headers", "X-Total-Count");
                Response.Headers.Append("X-Total-Count", resFromCache.Count.ToString());
                return Ok(resFromCache);
            }
            var preferences = await _preferencesRepository.GetAllAsync();
            var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);
            cacheData = preferences;
            _cacheService.SetData<IEnumerable<Preference>>("preferences", cacheData, expirationTime);

            var response = preferences.Select(x => new PreferenceResponse()
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();

            return Ok(response);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<List<PreferenceResponse>>> GetPreferenceByIdAsync(Guid id)
        {

            var cacheData = _cacheService.GetData<Preference>($"preference-{id}");
            if (cacheData != null)
            {
                return Ok(cacheData);
            }
            var preference = await _preferencesRepository.GetByIdAsync(id);
            var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);
            _cacheService.SetData<Preference>($"preference-{id}", preference, expirationTime);
            var response = new PreferenceResponse() { Id = id, Name = preference.Name };
            return Ok(response);
        }

        [HttpPost("range")]
        public async Task<ActionResult<List<PreferenceResponse>>> GetPreferencesByIdsAsync([FromBody] List<Guid> Ids)
        {
            List<Guid> missingIds = new List<Guid>();
            List<Preference> Result = new List<Preference>();
            var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);
            foreach (var cachedId in Ids)
            {
                var cached = _cacheService.GetData<Preference>($"preference-{cachedId}");

                if (cached == null)
                    missingIds.Add(cachedId);
                else
                    Result.Add(cached);
            }
            if (missingIds.Count > 0)
            {

                var missingPreferences = await _preferencesRepository.GetRangeByIdsAsync(missingIds);
                if (missingPreferences != null)
                {

                    Result.AddRange(missingPreferences);
                    foreach (var preference in missingPreferences)
                    {
                        _cacheService.SetData<Preference>($"preference-{preference.Id}", preference, expirationTime);
                    }

                }
            }

            var response = Result.Select(x => new PreferenceResponse()
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();

            return Ok(response);
        }
    }
}