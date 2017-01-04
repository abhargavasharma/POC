using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TAL.QuoteAndApply.DataLayer.Service;
using TAL.QuoteAndApply.DataModel;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.ServiceLayer.Policy;
using TAL.QuoteAndApply.ServiceLayer.Search.AgentDashboard;
using TAL.QuoteAndApply.ServiceLayer.Search.AgentDashboard.Models;
using TAL.QuoteAndApply.Tests.Shared.Builders;

namespace TAL.QuoteAndApply.ServiceLayer.IntegrationTests.Policy.AgentDashboard
{
    [TestFixture]
    public class GetAgentDashboardQuotesServiceTests : BaseServiceLayerTest
    {
        [Test]
        public void PolicyAccessed_WhenPolicyAccessed_InteractionCreated_AgentDashboardQuoteAdded_VwAgentDashboardSearchPopulatedWithOneEntryForQuote()
        {
            //Arrange
            DbItemClassMapper<DbItem>.RegisterClassMaps();
            //create a policy
            var createQuoteResult = CreateQuote(new CreateQuoteBuilder()
                .Build());
            var policyWithRisks = GetPolicy(createQuoteResult.QuoteReference);
            var svc = GetServiceInstance<IPolicyInteractionService>();

            //Act
            svc.PolicyAccessed(policyWithRisks.Policy.Id);
            //query on policy id

            var agentDashboardRequest = new AgentDashboardRequest()
            {
                ClosedCantContact = false,
                ClosedNoSale = false,
                ClosedTriage = false,
                ClosedSale = false,
                InProgressPreUw = false,
                InProgressCantContact = false,
                InProgressUwReferral = false,
                InProgressRecommendation = false,
                EndDate = DateTime.Now,
                StartDate = DateTime.Now.AddMonths(-1),
                User = GetApplicationCurrentUser()
            };

            var quote = FindQuoteInAgentDashboard(agentDashboardRequest, createQuoteResult.QuoteReference);
            Assert.That(quote, Is.Not.Null);
        }

        [Test]
        public void CreateQuotes_WhenPoliciesProgressUpdated_SpAgentDashboardSearchPopulatedWithCorrectQuotes_WithAllCorrectPipelineStatuses()
        {
            //Arrange
            DbItemClassMapper<DbItem>.RegisterClassMaps();
            //create a policy

            var policyService = GetServiceInstance<IUpdatePolicyService>();
            var agentDaschboardSvc = GetServiceInstance<IGetAgentDashboardQuotesService>();
            var svc = GetServiceInstance<IPolicyInteractionService>();

            var quoteList = new List<AgentDashboardQuotesResult>();

            var quoteCount = 0;
            while (quoteCount < 9)
            {
                var createQuoteResult = CreateQuote(new CreateQuoteBuilder()
                    .Build());
                var policyWithRisks = GetPolicy(createQuoteResult.QuoteReference);

                //Act
                svc.PolicyAccessed(policyWithRisks.Policy.Id);
                //query on policy id

                policyService.UpdateProgress(createQuoteResult.QuoteReference, (PolicyProgress)quoteCount);

                var agentDashboardRequest = new AgentDashboardRequest()
                {
                    Unknown = quoteCount == 0,
                    InProgressPreUw = quoteCount == 1,
                    InProgressUwReferral = quoteCount == 2,
                    InProgressRecommendation = quoteCount == 3,
                    InProgressCantContact = quoteCount == 4,
                    ClosedSale = quoteCount == 5,
                    ClosedNoSale = quoteCount == 6,
                    ClosedTriage = quoteCount == 7,
                    ClosedCantContact = quoteCount == 8,
                    EndDate = DateTime.Now,
                    StartDate = DateTime.Now.AddMonths(-1),
                    User = GetApplicationCurrentUser(),
                    PageNumber = 1
                };

                //Act
                var quotes = agentDaschboardSvc.GetQuotes(agentDashboardRequest, 100);

                //Assert
                Assert.That(quotes.Count(), Is.GreaterThanOrEqualTo(1));
                var quote = quotes.Where(x => x.QuoteReference == createQuoteResult.QuoteReference).ToList();
                Assert.That(quote.Count(), Is.EqualTo(1));
                Assert.That(quote[0].Progress, Is.EqualTo(((PolicyProgress)quoteCount).ToString()));

                quoteList.Add(quote.First());

                quoteCount++;
            }

            Assert.That(quoteList.Count(), Is.EqualTo(quoteCount));
        }

        [Test]
        public void CreateQuote_WhenPolicyProgressUpdatedByAnotherAgent_SpAgentDashboardSearchPopulatedWithCorrectQuotes_WithCorrectPipelineStatuses()
        {
            var firstAgent = GetApplicationCurrentUser();
            var secondAgent = "AgentJimbo";

            //Arrange
            DbItemClassMapper<DbItem>.RegisterClassMaps();

            var policyService = GetServiceInstance<IUpdatePolicyService>();
            var agentDaschboardSvc = GetServiceInstance<IGetAgentDashboardQuotesService>();
            var svc = GetServiceInstance<IPolicyInteractionService>();

            var createQuoteResult = CreateQuote(new CreateQuoteBuilder().Build());
            var policyWithRisks = GetPolicy(createQuoteResult.QuoteReference);
            
            //Act
            svc.PolicyAccessed(policyWithRisks.Policy.Id);
            policyService.UpdateProgress(createQuoteResult.QuoteReference, PolicyProgress.Unknown);

            var agentDashboardRequest = new AgentDashboardRequest()
            {
                Unknown = true,
                EndDate = DateTime.Now,
                StartDate = DateTime.Now.AddMonths(-1),
                User = GetApplicationCurrentUser(),
                PageNumber = 1
            };

            var quotes = agentDaschboardSvc.GetQuotes(agentDashboardRequest, 100);

            //Assert
            Assert.That(quotes.Count(), Is.GreaterThanOrEqualTo(1));
            var quote = quotes.Where(x => x.QuoteReference == createQuoteResult.QuoteReference).ToList();
            Assert.That(quote.Count(), Is.EqualTo(1));
            Assert.That(quote[0].Progress, Is.EqualTo(PolicyProgress.Unknown.ToString()));

            //Arrange: Login as new agent
            SetApplicationCurrentUser(secondAgent);
            var firstQuote = quote.First();

            //Act: access quote and update pipeline status as new agent
            svc.PolicyAccessed(firstQuote.PolicyId);
            policyService.UpdateProgress(firstQuote.QuoteReference, PolicyProgress.ClosedSale);

            //Reset request
            agentDashboardRequest.ClosedSale = true;
            agentDashboardRequest.User = GetApplicationCurrentUser();
            
            //Assert
            Assert.That(agentDashboardRequest.User, Is.EqualTo(secondAgent));
            var newAgentQuote = FindQuoteInAgentDashboard(agentDashboardRequest, firstQuote.QuoteReference);
            Assert.That(newAgentQuote.Progress, Is.EqualTo(PolicyProgress.ClosedSale.ToString()));

            //Log back in as first agent and reset
            SetApplicationCurrentUser(firstAgent);
            agentDashboardRequest.User = firstAgent;
            agentDashboardRequest.ClosedSale = false;
            agentDashboardRequest.Unknown = true;

            var fistAgentQuote = FindQuoteInAgentDashboard(agentDashboardRequest, firstQuote.QuoteReference);

            //Filtering on previous pipeline status, assert that overriden pipeline status doesn't appear in list
            Assert.That(fistAgentQuote, Is.Null);
        }
    }
}

