﻿using System.Collections.Generic;
using IdentityServer3.Core.Models;

namespace MyIdentityServer.Tests.TestHelpers
{
    public static class Scopes
    {
        public static List<Scope> Get()
        {
            return new List<Scope>
            {
                new Scope
                {
                    Name = "api1"
                }
            };
        }
    }
}