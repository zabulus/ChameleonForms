﻿using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AngleSharp.Dom.Html;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using RazorPagesProject.Tests.Helpers;

namespace RazorPagesProject.Tests.IntegrationTests
{
    #region snippet1
    public class BasicTests : IClassFixture<WebApplicationFactory<ChameleonForms.Example.Startup>>
    {
        private readonly WebApplicationFactory<ChameleonForms.Example.Startup> _factory;

        public BasicTests(WebApplicationFactory<ChameleonForms.Example.Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/Index")]
        [InlineData("/About")]
        [InlineData("/Privacy")]
        [InlineData("/Contact")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
        #endregion

        #region snippet2
        [Fact]
        public async Task Get_SecurePageRequiresAnAuthenticatedUser()
        {
            // Arrange
            var client = _factory.CreateClient(
                new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false
                });

            // Act
            var response = await client.GetAsync("/SecurePage");

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.StartsWith("http://localhost/Identity/Account/Login",
                response.Headers.Location.OriginalString);
        }
        #endregion

        #region snippet3
        [Fact]
        public async Task CanGetAGithubUser()
        {
            // Arrange
            var client = _factory
                .CreateClient();

            // Act
            var profile = await client.GetAsync("/GithubProfile");
            Assert.Equal(HttpStatusCode.OK, profile.StatusCode);
            var profileHtml = await HtmlHelpers.GetDocumentAsync(profile);

            var profileWithUserName = await client.SendAsync(
                (IHtmlFormElement)profileHtml.QuerySelector("#user-profile"),
                new Dictionary<string, string> { ["Input_UserName"] = "user" });

            // Assert
            Assert.Equal(HttpStatusCode.OK, profileWithUserName.StatusCode);
            var profileWithUserHtml =
                await HtmlHelpers.GetDocumentAsync(profileWithUserName);
            var userLogin = profileWithUserHtml.QuerySelector("#user-login");
            Assert.Equal("user", userLogin.TextContent);
        }

        #endregion
    }
}
