﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebPresentationMVC.Models;

namespace WebPresentationMVC.Api.Endpoints.Interfaces
{
    public interface IAuthenticationEndpoint
    {
        Task<Token> GetToken(LoginModel model);
        Task RegisterAccount(RegisterModel model);
    }
}