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

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeRolesAttribute"/> class.
        /// </summary>
        public AuthorizeRolesAttribute(params UserGroup[] roles)
        {
            _roles = roles;
        }

        /// <summary>
        /// Handles the authorization of the request.
        /// </summary>
        /// <param name="context">The authorization filter context.</param>
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
