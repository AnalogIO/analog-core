using System;
using CoffeeCard.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CoffeeCard.WebApi.Helpers
{
    /// <summary>
    /// Custom authorization attribute to allow usage of enums instead of strings for roles
    /// </summary>
    public class AuthorizeRolesAttribute : Attribute, IAuthorizationFilter
    {
        private readonly UserGroup[] _roles;
        public AuthorizeRolesAttribute(params UserGroup[] roles)
        {
            _roles = roles;
        }
        
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            var isAuthorized = false;
            foreach (var userGroup in _roles)
            {
                isAuthorized = user.IsInRole(userGroup.ToString());
                if (isAuthorized)
                    break;
            }

            if (!isAuthorized)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}