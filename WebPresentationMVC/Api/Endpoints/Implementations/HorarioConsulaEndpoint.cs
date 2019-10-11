﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using WebPresentationMVC.Api.Endpoints.Interfaces;
using WebPresentationMVC.Api.Exceptions;
using WebPresentationMVC.Models;

namespace WebPresentationMVC.Api.Endpoints.Implementations
{
    public class HorarioConsulaEndpoint : IHorarioConsultaEndpoint
    {
        private IApiHelper _apiHelper;
        private readonly IUserSession _userSession;

        public HorarioConsulaEndpoint(IApiHelper apiHelper, IUserSession userSession)
        {
            _apiHelper = apiHelper;
            _userSession = userSession;
        }

        public async Task<IEnumerable<MvcHorarioConsultaModel>> GetAll()
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync("api/horariosConsulta", x => SetToken(x)))
            {
                if (!response.IsSuccessStatusCode)
                {
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.Unauthorized:
                            throw new UnauthorizedRequestException(response);
                        default:
                            throw new Exception($"{response.ReasonPhrase}: Contacte a soporte para mas detalles");
                    }
                }

                var result = await response.Content.ReadAsAsync<IEnumerable<MvcHorarioConsultaModel>>();

                return result;
            }
        }

        public async Task<MvcHorarioConsultaModel> Get(object id)
        {
            using (var response = await _apiHelper.ApiClient.GetAsync("api/horariosConsulta/" + id, x => SetToken(x)))
            {
                if (!response.IsSuccessStatusCode)
                {
                    switch (response.StatusCode) // Add NotFound Case
                    {
                        case HttpStatusCode.Unauthorized:
                            throw new UnauthorizedRequestException(response);
                        case HttpStatusCode.NotFound:
                            throw new NotFoundRequestException(response, id);
                        default:
                            throw new Exception($"{response.ReasonPhrase}: Contacte a soporte para mas detalles");
                    }
                }

                var result = await response.Content.ReadAsAsync<MvcHorarioConsultaModel>();

                return result;
            }
        }

        public async Task Delete(object id)
        {
            using (HttpResponseMessage response = await _apiHelper.ApiClient.DeleteAsync("api/horariosConsulta/" + id, x => SetToken(x)))
            {
                if (!response.IsSuccessStatusCode)
                {
                    // BadRequest might be received as well
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.Unauthorized:
                            throw new UnauthorizedRequestException(response);   
                        case HttpStatusCode.NotFound:
                            throw new NotFoundRequestException(response, id);
                        default:
                            throw new Exception($"{response.ReasonPhrase}: Contacte a soporte para mas detalles");
                    }
                }
            }
        }

        public async Task Post(MvcHorarioConsultaModel entity)
        {
            using (var response = await _apiHelper.ApiClient.PostAsJsonAsync("api/horariosConsulta", entity, x => SetToken(x)))
            {
                if (!response.IsSuccessStatusCode)
                {
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.Unauthorized:
                            throw new UnauthorizedRequestException(response);
                        case HttpStatusCode.BadRequest:
                            throw new BadRequestException(response);
                        default:
                            throw new Exception($"{response.ReasonPhrase}: Contacte a soporte para mas detalles");
                    }
                }
            }
        }

        public async Task Put(MvcHorarioConsultaModel entity)
        {
            using (var response = await _apiHelper.ApiClient.PutAsJsonAsync("api/horariosConsulta/" + entity.Id, entity, x => SetToken(x)))
            {
                if (!response.IsSuccessStatusCode)
                {
                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.Unauthorized:
                            throw new UnauthorizedRequestException(response);
                        case HttpStatusCode.BadRequest:
                            throw new BadRequestException(response);
                        case HttpStatusCode.NotFound:
                            throw new NotFoundRequestException(response, entity.Id);
                        default:
                            throw new Exception($"{response.ReasonPhrase}: Contacte a soporte para mas detalles");
                    }
                }
            }
        }

        private void SetToken(HttpRequestMessage r)
        {
            r.Headers.Authorization = AuthenticationHeaderValue.Parse(_userSession.BearerToken);
        }
    }
}