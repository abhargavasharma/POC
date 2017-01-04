using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TAL.QuoteAndApply.DataModel.Policy;
using TAL.QuoteAndApply.DataModel.Product;
using TAL.QuoteAndApply.DataModel.Service;
using TAL.QuoteAndApply.DataModel.Underwriting;
using TAL.QuoteAndApply.Policy.Configuration;
using TAL.QuoteAndApply.Policy.Data;
using TAL.QuoteAndApply.Policy.Models;
using TAL.QuoteAndApply.Policy.Service.RaisePolicy;
using TAL.QuoteAndApply.ServiceLayer.Payment.Models;
using TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy.Models;

namespace TAL.QuoteAndApply.ServiceLayer.Policy.RaisePolicy
{
    public interface IRaisePolicyConverter
    {
        Models.RaisePolicy From(PolicyNewBusinessOrderProcess_Type policyUberObject);
        PolicyNewBusinessOrderProcess_Type From(Models.RaisePolicy raisePolicy);
    }

    public class RaisePolicyConverter : IRaisePolicyConverter
    {
        private readonly IRaisePolicyDefinitionBuilder _raisePolicyDefinitionBuilder;
        private readonly IBeneficiaryRelationshipDtoRepository _beneficiaryRelationshipDtoRepository;
        private readonly IRaisePolicyPlanSpecificMappingProvider _raisePolicyPlanSpecificMappingProvider;
        private readonly IRaisePolicyConfigurationProvider _raisePolicyConfigurationProvider;

        public RaisePolicyConverter(IRaisePolicyDefinitionBuilder raisePolicyDefinitionBuilder, 
            IBeneficiaryRelationshipDtoRepository beneficiaryRelationshipDtoRepository,
            IRaisePolicyPlanSpecificMappingProvider raisePolicyPlanSpecificMappingProvider, 
            IRaisePolicyConfigurationProvider raisePolicyConfigurationProvider)
        {
            _raisePolicyDefinitionBuilder = raisePolicyDefinitionBuilder;
            _beneficiaryRelationshipDtoRepository = beneficiaryRelationshipDtoRepository;
            _raisePolicyPlanSpecificMappingProvider = raisePolicyPlanSpecificMappingProvider;
            _raisePolicyConfigurationProvider = raisePolicyConfigurationProvider;
        }

        public Models.RaisePolicy From(PolicyNewBusinessOrderProcess_Type policyUberObject)
        {
            var retVal = new Models.RaisePolicy();
            return retVal;
        }

        public PolicyNewBusinessOrderProcess_Type From(Models.RaisePolicy raisePolicy)
        {
            var raisePolicyPolicyOrderDetails = _raisePolicyDefinitionBuilder.BuildRaisePolicyPolicyOrderDefinition();

            var messageHeader = MapRaisePolicyMessageHeader();

            var retVal = new PolicyNewBusinessOrderProcess_Type()
            {
                MessageHeader = messageHeader,
                PolicyOrder = new PolicyOrder_Type()
                {
                    key = raisePolicy.QuoteReference,
                    DocumentId = raisePolicyPolicyOrderDetails.DocumentId,
                    TransactionFunctionCode = new PolicyOrderTransactionFunctionCode_Type()
                    {
                        Value = new XmlQualifiedName(raisePolicyPolicyOrderDetails.TransactionFunctionCode)
                    },
                    ApplicationTypeCode = new ApplicationTypeCode_Type()
                    {
                        Value = new XmlQualifiedName(raisePolicyPolicyOrderDetails.ApplicationTypeCode)
                    },
                    Description = raisePolicyPolicyOrderDetails.Description,
                    CaseId = raisePolicyPolicyOrderDetails.CaseId,
                    PreparedDate = messageHeader.MessageDateTime,
                    PreparedDateSpecified = true,
                    BroadLineOfBusinessCode = new BroadLineOfBusinessCode_Type
                    {
                        Value = new XmlQualifiedName(raisePolicyPolicyOrderDetails.BroadLineOfBusinessCode)
                    },
                    LodgementTypeCode = new LodgementTypeCode_Type()
                    {
                        Value = new XmlQualifiedName(raisePolicyPolicyOrderDetails.LodgementTypeCode)
                    },
                    //Removed as not working Sydney-side
                    FileAttachment = AddDocumentList(raisePolicy),
                    Address = AddAddresses(raisePolicy.Owner, raisePolicy.PrimaryRisk),
                    Person = AddPeople(raisePolicy.Owner, raisePolicy.PrimaryRisk),
                    Policy = new Policy_Type()
                    { 
                        ProductReferences = new ProductReferences_Type()
                        {
                            key = "34", //hard-coded -see integration mapping excel sheet line 26
                            SuperIndicator = false,
                            SuperIndicatorSpecified = true
                        },
                        PolicySection = AddPolicySection(raisePolicy),
                        //TODO confirm the index. Was 0 but as it is about risk - should be 1. Probably.
                        LifeItemSection = AddLifeItemSection(raisePolicy, 1, raisePolicy.PrimaryRisk),
                        InsurancePeriod = new InsurancePeriod_Type()
                        {
                            StartDate = raisePolicy.ReadyToSubmitDateTime
                        },
                        FundReferences = new FundReferences_Type
                        {
                            AssignedIdentifier = new []
                            {
                                new AssignedIdentifier_Type
                                {
                                    Id = "N",
                                    IdentifierDescription = "SuperContributionSource",
                                    RoleCode = new CoreRoleCode_Type
                                    {
                                        Value = new XmlQualifiedName("Insurer")
                                    }
                                },
                                new AssignedIdentifier_Type
                                {
                                    Id = raisePolicy.Owner.OwnerType == PolicyOwnerType.SelfManagedSuperFund ? "P" : "N",
                                    IdentifierDescription = "RiskUnderSuperFlag",
                                    RoleCode = new CoreRoleCode_Type
                                    {
                                        Value = new XmlQualifiedName("Insurer")
                                    }
                                }
                            }
                        }
                    },
                    PolicyReferences = new PolicyReferences_Type()
                    {
                        ProductReferences = new ProductReferences_Type()
                        {
                            AssignedIdentifier = new List<AssignedIdentifier_Type>
                            {
                                new AssignedIdentifier_Type
                                {
                                    Id = "1",
                                    IdentifierDescription = "ProductionVersionNumber",
                                    RoleCode = new CoreRoleCode_Type
                                    {
                                        Value = new XmlQualifiedName("Insurer")
                                    }
                                }
                            }.ToArray()
                        },
                        AssignedIdentifier = GetUnderwritingOutcome(raisePolicy)

                    },
                    Organization = new []
                    {
                        new Organization_Type
                        {
                            key = $"{PasInterestedPartyRoleCodeConstants.AgentKey}0",
                            AssignedIdentifier = new []
                            {
                                new AssignedIdentifier_Type
                                {
                                    Id = "51243",
                                    RoleCode = new CoreRoleCode_Type {Value = new XmlQualifiedName(PasInterestedPartyRoleCodeConstants.Agency)}
                                }
                            }
                        }
                    }
                }
            };
            return retVal;
        }

        private static AssignedIdentifier_Type[] GetUnderwritingOutcome(Models.RaisePolicy raisePolicy)
        {
            var outcome = "Standard";
            if (raisePolicy.HasExclusions || raisePolicy.HasLoadings)
            {
                outcome = "Revised";
            }

            return new List<AssignedIdentifier_Type>
            {
                new AssignedIdentifier_Type
                {
                    Id = outcome,
                    IdentifierDescription = "UnderwritingOutcome",
                    RoleCode = new CoreRoleCode_Type
                    {
                        Value = new XmlQualifiedName("Insurer")
                    }
                }
            }.ToArray();
        }

        public MessageHeader_Type MapRaisePolicyMessageHeader()
        {
            var raisePolicyMessageHeader = _raisePolicyDefinitionBuilder.BuildRaisePolicyMessageHeaderDefinition();
            var messageHeader = new MessageHeader_Type()
            {
                key = _raisePolicyConfigurationProvider.EnvironmentKey,
                ACORDStandardVersionCode = new ACORDStandardVersionCode_Type()
                {
                    Value = new XmlQualifiedName(raisePolicyMessageHeader.ACORDStandardVersionCode)
                },
                MessageId = raisePolicyMessageHeader.MessageId,
                CorrelationId = raisePolicyMessageHeader.CorrelationId,
                MessageDateTime = raisePolicyMessageHeader.MessageDateTime,
                Sender = new Sender_Type()
                {
                    key = raisePolicyMessageHeader.Sender,
                    Contact = AddCurrentUser()
                },
                Receiver = new Receiver_Type()
                {
                    key = raisePolicyMessageHeader.Receiver
                },
                MessageSubmissionTypeCode = new Code_Type() { Value = "Online" }
            };
            return messageHeader;
        }

        private EndPointActorContact_Type[] AddCurrentUser()
        {
            var returnObj = new List<EndPointActorContact_Type>()
            {
                new EndPointActorContact_Type()
                {
                    PersonReferences = new PersonReferences_Type()
                    {
                        PersonName = new List<PersonName_Type>()
                        {
                            new PersonName_Type()
                            {
                                FullName = "MMEAPP"
                            }
                        }.ToArray()
                    }
                }
            }.ToArray();
            return returnObj;
        }

        private FileAttachment_Type[] AddDocumentList(Models.RaisePolicy raisePolicy)
        {
            var returnObj = new []
            {
                new FileAttachment_Type
                {
                    AttachmentDocument = new AttachmentDocument_Type()
                    {
                        DocumentURI = raisePolicy.DocumentUrl
                    }
                }
            };

            return returnObj;
        }

        private LifeItemSection_Type[] AddLifeItemSection(Models.RaisePolicy raisePolicy, int index, RaisePolicyRisk risk)
        {
            return new[]
            {
                new LifeItemSection_Type()
                {
                    key = "PERSON" + index,
                    Coverage = AddCoverages(raisePolicy, risk.Plans),
                    EmployeeInformation = AddEmploymentInformation(risk),
                    FinancialReview = AddEmployeeIncome(risk.AnnualIncome),
                    AssignedIdentifier = new[]
                    {
                        new AssignedIdentifier_Type
                        {
                            Id = risk.InterviewId,
                            IdentifierDescription = "InterviewID",
                            RoleCode = new CoreRoleCode_Type
                            {
                                Value = new XmlQualifiedName("Insurer")
                            }
                        }
                    }
                }
            };
        }

        private EmployeeInformation_Type[] AddEmploymentInformation(RaisePolicyRisk policyOwner)
        {
            var returnObj = new List<EmployeeInformation_Type>()
            {
                new EmployeeInformation_Type()
                {
                    OccupationClassCode = new OccupationClassCode_Type()
                    {
                        Value = new XmlQualifiedName(policyOwner.OccupationClass)
                    },
                    OccupationDescription = policyOwner.OccupationTitle
                }
            }.ToArray();
            return returnObj;
        }

        private LifeItemSectionCoverage_Type[] AddCoverages(Models.RaisePolicy raisePolicy, List<RaisePolicyPlan> plans)
        {
            return plans.Select(p=> AddCoverageForPlan(raisePolicy, p)).ToArray();
        }

        private LifeItemSectionCoverage_Type AddCoverageForPlan(Models.RaisePolicy raisePolicy, RaisePolicyPlan plan)
        {
            var raisePolicyPlanMapping = _raisePolicyPlanSpecificMappingProvider.GetFor(plan.Code);

            var valuationTypeCode = "SumInsured";
            if (raisePolicyPlanMapping.IsIncomeProtection)
            {
                valuationTypeCode = "MonthlyBenefit";
            }

            var coverageCategoryCode = "Primary";
            if (raisePolicyPlanMapping.IsRider)
            {
                coverageCategoryCode = "Rider";
            }

            var coverObj = new LifeItemSectionCoverage_Type()
            {
                TypeCode = new LifeItemSectionCoverageTypeCode_Type()
                {
                    Value = new XmlQualifiedName(raisePolicyPlanMapping.Name),
                },
                CoverageCategory = new CoverageCategory_Type
                {
                    CoverageCategoryCode = new CoverageCategoryCode_Type
                    {
                        Value = new XmlQualifiedName(coverageCategoryCode)
                    }
                },
                Limit = new List<Limit_Type>()
                {
                    new Limit_Type()
                    {
                        ValuationTypeCode = new ValuationTypeCode_Type()
                        {
                            Value = new XmlQualifiedName(valuationTypeCode)
                        },
                        LimitAmount = new Amount_Type()
                        {
                            Value = plan.CoverAmount
                        }
                    }
                }.ToArray(),
                PolicyReferences = AddCoveragePolicyReferences(raisePolicy),
                UnderwritingDecision = "Accepted"
            };

            var premiumParts = new List<InsuranceAmountItem_Type>
            {
                new InsuranceAmountItem_Type
                    {
                    TypeCode = new InsuranceAmountItemTypeCode_Type {Value = new XmlQualifiedName("InsuredPayableAmount")},
                        ContractAmountItem = new List<ContractAmountItem_Type>
                        {
                            new ContractAmountItem_Type
                            {
                                ItemAmount = new Amount_Type
                                {
                                    Value = plan.Premium
                                },
                                TypeCode = new ContractAmountItemTypeCode_Type()
                                {
                                    Value = new XmlQualifiedName("TaxablePremium")
                                }
                            }
                        }.ToArray()
                    }
            };

            var options = new List<LifeCoverageOption_Type>
            {
                AddPremiumType(plan.PremiumType),
                AddOutOfTreatyAdjustmentPercent(),
                AddIndexationOption(plan.LinkedToCpi)
            };

            options.AddRange(AddIpCoverType(raisePolicyPlanMapping));
            options.AddRange(AddTpdOccupationType(raisePolicyPlanMapping, plan.OccupationDefinition));
            options.AddRange(AddPlanOptions(plan.Code, plan.Options));
            options.AddRange(AddComissionsOptions());

            AddCoverBlocks(plan, raisePolicyPlanMapping, coverObj, options, premiumParts);

            coverObj.Option = options.ToArray();
            coverObj.InsuranceAmountItem = premiumParts.ToArray();

            coverObj.Deductible = new []
            {
                new Deductible_Type
                {
                    AssignedIdentifier = new []
                    {
                        new AssignedIdentifier_Type
                        {
                            Id = plan.MultiPlanDiscountFactor.ToString(),
                            IdentifierDescription = "HealthSenseDiscountPercent",
                            RoleCode = new CoreRoleCode_Type
                            {
                                Value = new XmlQualifiedName("Insurer")
                            }
                        }
                    }
                }
            };

            if (plan.BenefitPeriod.HasValue)
            {
                coverObj.BenefitPeriodCode = new BenefitPeriodCode_Type
                {
                    Value = new XmlQualifiedName($"Year{plan.BenefitPeriod.Value}")
                };
            }

            if (plan.WaitingPeriod.HasValue)
            {
                var waitingPeriodDays = plan.WaitingPeriod.Value * 7;

                coverObj.WaitingPeriodCode = new WaitingPeriodCode_Type()
                {
                    Value = new XmlQualifiedName($"Day{waitingPeriodDays}")
                };
            }

            return coverObj;
        }

        private void AddCoverBlocks(RaisePolicyPlan plan, RaisePolicyPlanSpecificMapping raisePolicyPlanMapping, LifeItemSectionCoverage_Type coverObj, List<LifeCoverageOption_Type> options, List<InsuranceAmountItem_Type> premiumParts)
        {
            foreach (var cover in plan.Covers)
            {
                var coverMapping = raisePolicyPlanMapping.Covers[cover.Code];

                var val = "Y";
                if (!cover.Selected)
                {
                    val = "N";
                }

                options.Add(new LifeCoverageOption_Type
                {
                    AssignedIdentifier = new[]
                    {
                        new AssignedIdentifier_Type
                        {
                            RoleCode = new CoreRoleCode_Type { Value = new XmlQualifiedName("Insurer")},
                            Id = val,
                            IdentifierDescription = GetCoverBlockDescription(coverMapping.CoverType)
                        }
                    }
                });

                AddCoverBlockLoadings(premiumParts, cover, coverMapping);
            }
        }

        private void AddCoverBlockLoadings(List<InsuranceAmountItem_Type> premiumParts, RaisePolicyCover cover,
            RaisePolicyCoverSpecificMapping coverMapping)
        {
            var contractAmountItems = new List<ContractAmountItem_Type>();

            foreach (var coverLoading in cover.Loadings)
            {
                if (coverLoading.LoadingType == LoadingType.PerMille)
                {
                    contractAmountItems.Add(GetPerMilleLoading(coverMapping.CoverBlockCode, coverLoading.Loading));
                }
                if (coverLoading.LoadingType == LoadingType.Variable)
                {
                    contractAmountItems.Add(GetPercentageLoading(coverMapping.CoverBlockCode, coverLoading.Loading));
                }
            }

            if (contractAmountItems.Any())
            {
                premiumParts.Add(new InsuranceAmountItem_Type
                {
                    TypeCode = new InsuranceAmountItemTypeCode_Type() {Value = new XmlQualifiedName("Loading")},
                    ContractAmountItem = contractAmountItems.ToArray()
                });
            }
        }

        private ContractAmountItem_Type GetPercentageLoading(string coverBlockCode, decimal loading)
        {
            return new ContractAmountItem_Type
            {
                Description = coverBlockCode,
                ItemPercent = loading,
                ItemPercentSpecified = true,
                ItemFactor =  0,
                ItemFactorSpecified = true,
                TypeCode = new ContractAmountItemTypeCode_Type()
                {
                    Value = new XmlQualifiedName("RI")
                }
            };
        }

        private ContractAmountItem_Type GetPerMilleLoading(string coverBlockCode, decimal loadingAmt)
        {
            return new ContractAmountItem_Type
            {
                Description = coverBlockCode,
                ItemAmount = new Amount_Type
                {
                    Value = loadingAmt
                },
                ItemFactor = 0,
                ItemFactorSpecified = true,
                TypeCode = new ContractAmountItemTypeCode_Type()
                {
                    Value = new XmlQualifiedName("PM")
                }
            };
        }

        private string GetCoverBlockDescription(RaisePolicyCoverType coverType)
        {
            switch (coverType)
            {
                case RaisePolicyCoverType.AccidentOrSeriousInjury:
                    return "AccidentInjuryCoverFlag";
                case RaisePolicyCoverType.Illness:
                    return "IllnessCoverFlag";
                case RaisePolicyCoverType.SportsOrCancer:
                    return "SportsCancerFlag";
                default:
                    throw new ArgumentOutOfRangeException(nameof(coverType), coverType, null);
            }
        }

        private PolicyReferences_Type AddCoveragePolicyReferences(Models.RaisePolicy raisePolicy)
        {
            var decisionDate = raisePolicy.ReadyToSubmitDateTime;
            if (raisePolicy.LastCompletedReferralDateTime.HasValue)
            {
                decisionDate = raisePolicy.LastCompletedReferralDateTime.Value;
            }

            return new PolicyReferences_Type
            {
                AssignedIdentifier = new[]
                {
                    new AssignedIdentifier_Type
                    {
                        Id = decisionDate.ToString("yyyy-MM-dd"),
                        IdentifierDescription = "UnderwritingDecisionDate",
                        RoleCode = new CoreRoleCode_Type {Value = new XmlQualifiedName("Insurer")}
                    }
                }
            };
        }

        private LifeCoverageOption_Type AddIndexationOption(bool? linkedToCpi)
        {
            var indexationValue = "N";
            if (linkedToCpi.GetValueOrDefault(false))
            {
                indexationValue = "Y";
            }

            var option = new LifeCoverageOption_Type()
            {
                AssignedIdentifier = new List<AssignedIdentifier_Type>
                {
                    new AssignedIdentifier_Type
                    {
                        Id = indexationValue, IdentifierDescription = "CPIIndexationFlag", RoleCode = new CoreRoleCode_Type
                        {
                            Value = new XmlQualifiedName("Insurer")
                        }
                    }
                }.ToArray()
            };

            return option;
        }

        private IEnumerable<LifeCoverageOption_Type> AddComissionsOptions()
        {
            return new[]
            {
                new LifeCoverageOption_Type()
                {
                    AssignedIdentifier = new List<AssignedIdentifier_Type>
                    {
                        new AssignedIdentifier_Type
                        {
                            Id = "L", IdentifierDescription = "CommissionStyle", RoleCode = new CoreRoleCode_Type
                            {
                                Value = new XmlQualifiedName("Insurer")
                            }
                        }
                    }.ToArray()
                },
                new LifeCoverageOption_Type()
                {
                    AssignedIdentifier = new List<AssignedIdentifier_Type>
                    {
                        new AssignedIdentifier_Type
                        {
                            Id = "STD", IdentifierDescription = "CommissionRate", RoleCode = new CoreRoleCode_Type
                            {
                                Value = new XmlQualifiedName("Insurer")
                            }
                        }
                    }.ToArray()
                }
            };
        }

        private IEnumerable<LifeCoverageOption_Type> AddTpdOccupationType(RaisePolicyPlanSpecificMapping raisePolicyPlanMapping, OccupationDefinition occupationDefinition)
        {
            var options = new List<LifeCoverageOption_Type>();

            if (raisePolicyPlanMapping.IsTpd && occupationDefinition != OccupationDefinition.Unknown)
            {
                options.Add(new LifeCoverageOption_Type
                {
                    Description = "TPDDefinition", OptionCode = new OptionCode_Type()
                    {
                        Value = new XmlQualifiedName(occupationDefinition.ToString())
                    }
                });
            }

            return options;
        }

        private LifeCoverageOption_Type AddOutOfTreatyAdjustmentPercent()
        {
            return new LifeCoverageOption_Type()
            {
                AssignedIdentifier = new List<AssignedIdentifier_Type>
                {
                    new AssignedIdentifier_Type
                    {
                        Id = "0.00", IdentifierDescription = "OutOfTreatyAdjustmentPercent", RoleCode = new CoreRoleCode_Type
                        {
                            Value = new XmlQualifiedName("Insurer")
                        }
                    }
                }.ToArray()
            };
        }

        private IEnumerable<LifeCoverageOption_Type> AddIpCoverType(RaisePolicyPlanSpecificMapping raisePolicyPlanMapping)
        {
            var options = new List<LifeCoverageOption_Type>();
            string coverTypeVal = null;

            if (raisePolicyPlanMapping.IsIncomeProtection || raisePolicyPlanMapping.IsCriticalIllness)
            {
                coverTypeVal = "S";
            }

            options.Add(new LifeCoverageOption_Type
                {
                    AssignedIdentifier = new List<AssignedIdentifier_Type>
                    {
                        new AssignedIdentifier_Type
                        {
                            Id = coverTypeVal,
                            IdentifierDescription = "CoverType",
                            RoleCode = new CoreRoleCode_Type
                            {
                                Value = new XmlQualifiedName("Insurer")
                            }
                        }
                    }.ToArray()
                });

            return options;
        }

        private IEnumerable<LifeCoverageOption_Type> AddPlanOptions(string planCode, List<RaisePolicyOption> planOptions)
        {
            var options = new List<LifeCoverageOption_Type>();

            if (planOptions == null)
            {
                return options;
            }

            if (IsOptionSelected(planOptions, "IC"))
            {
                options.Add(new LifeCoverageOption_Type
                {
                    Description = "IncreasingClaimOption"
                });
            }

            if (IsOptionSelected(planOptions, "PR"))
            {
                options.Add(new LifeCoverageOption_Type
                {
                    Description = "PremiumRelief"
                });
            }

            if (IsOptionSelected(planOptions, "DOA"))
            {
                options.Add(new LifeCoverageOption_Type
                {
                    Description = "DayOneAccidentBenefit"
                });
            }

            if (IsOptionSelected(planOptions, planCode + "DBB"))
            {
                options.Add(new LifeCoverageOption_Type
                {
                    Description = "DeathBuyBack"
                });
            }

            options.Add(new LifeCoverageOption_Type
            {
                OptionCode = new OptionCode_Type() { Value = new XmlQualifiedName("Indemnity") },
                Description = "IndemnityOption"
            });

            return options;
        }

        private bool IsOptionSelected(List<RaisePolicyOption> planOptions, string optionCode)
        {
            var option = planOptions.FirstOrDefault(o => o.Code == optionCode);
            return (option != null && option.Selected.HasValue && option.Selected.Value);
        }

        private LifeCoverageOption_Type AddPremiumType(PremiumType premiumType)
        {
            return new LifeCoverageOption_Type()
            {
                AssignedIdentifier = new List<AssignedIdentifier_Type>
                {
                    new AssignedIdentifier_Type
                    {
                        Id = GetPremiumTypeId(premiumType), IdentifierDescription = "PremiumStyle", RoleCode = new CoreRoleCode_Type
                        {
                            Value = new XmlQualifiedName("Insurer")
                        }
                    }
                }.ToArray()
            };
        }

        private string GetPremiumTypeId(PremiumType premiumType)
        {
            if (premiumType == PremiumType.Stepped)
            {
                return "S";
            }

            return "L";
        }

        private FinancialReview_Type[] AddEmployeeIncome(long annualIncome)
        {
            var returnObj = new List<FinancialReview_Type>()
            {
                new FinancialReview_Type()
                {
                    FinancialAmountItem = new List<FinancialReviewFinancialAmountItem_Type>()
                    {
                        new FinancialReviewFinancialAmountItem_Type()
                        {
                            AmountItemValue = new List<AmountItemValue_Type>
                            {
                                new AmountItemValue_Type()
                                {
                                    ItemAmount = new Amount_Type() {Value = annualIncome}
                                }
                            }.ToArray(),
                            TypeCode = new FinancialReviewFinancialAmountItemTypeCode_Type()
                            {
                                Value = new XmlQualifiedName("AnnualEarnings")
                            }
                        }
                    }.ToArray()
                }
            }.ToArray();
            return returnObj;
        }

        private PolicyInterestedParty_Type AddPolicyInterestedParty(int index, string key, string name)
        {
            return new PolicyInterestedParty_Type()
            {
                key = key + index,
                RoleCode = new InterestedPartyRoleCode_Type { Value = new XmlQualifiedName(name) }
            };
        }

        private PolicyInterestedParty_Type AddPolicyBeneficiaryInterestedParty(int index, string key, string name, RaisePolicyBeneficiary beneficiary)
        {
            return new PolicyInterestedParty_Type()
            {
                key = key + (index + 1), RoleCode = new InterestedPartyRoleCode_Type()
                {
                    Value = new XmlQualifiedName(name)
                },
                BenefitValue = new BenefitValue_Type
                {
                    ValuePercent = beneficiary.Share,
                    ValuePercentSpecified = true
                },
                InterestedParty = new InterestedParty_Type
                {
                    RelationshipToInsuredCode = new RelationshipToInsuredCode_Type
                    {
                        Value = new XmlQualifiedName(_beneficiaryRelationshipDtoRepository.GetPasExportBeneficiaryRelationship(beneficiary.BeneficiaryRelationshipId))
                    }
                },
                InterestedPartyPeriod = new InterestedPartyPeriod_Type
                {
                    ContinuousDurationIndicator = false //Is varchar(1) or N in mapping spreadsheet
                },
                //need to set both of these for serialization to work properly. Magic!
                BeneficiaryBindingIndicator = false,
                BeneficiaryBindingIndicatorSpecified = true,
                BeneficiaryBindingPeriod = new BeneficiaryBindingPeriod_Type
                {
                    EndDate = DateTime.Now //Send nothing in mapping spreadsheet - line 103
                }
            };
        }

        private PolicySection_Type AddPolicySection(Models.RaisePolicy raisePolicy)
        {
            var ownerInterestedParty = AddPolicyInterestedParty(0, PasInterestedPartyRoleCodeConstants.PersonKey, PasInterestedPartyRoleCodeConstants.PolicyOwner);
            var insuredInterestedParty = AddPolicyInterestedParty(1, PasInterestedPartyRoleCodeConstants.PersonKey, PasInterestedPartyRoleCodeConstants.Insured);

            var beneficiaryInterestedParties = raisePolicy.PrimaryRisk.Beneficiaries.Select(
                    (beneficiary, i) => AddPolicyBeneficiaryInterestedParty(i, PasInterestedPartyRoleCodeConstants.PersonKey, PasInterestedPartyRoleCodeConstants.Beneficiary, beneficiary)
                    ).ToList();
            var agentInterestedParty = AddPolicyInterestedParty(0, PasInterestedPartyRoleCodeConstants.AgentKey, PasInterestedPartyRoleCodeConstants.Agency);

            var interestedParties = new List<PolicyInterestedParty_Type>
            {
                ownerInterestedParty, insuredInterestedParty, agentInterestedParty
            };
            interestedParties.AddRange(beneficiaryInterestedParties);

            var returnObj = new PolicySection_Type()
            {
                //policy fee, always zero
                InsuranceAmountItem = new[]
                {
                    new InsuranceAmountItem_Type
                    {
                        ContractAmountItem = new[]
                        {
                            new ContractAmountItem_Type
                            {
                                TypeCode = new ContractAmountItemTypeCode_Type()
                                {
                                    Value = new XmlQualifiedName("PolicyFee")
                                },
                                ItemAmount = new Amount_Type
                                {
                                    Value = 0
                                }
                            },
                            new ContractAmountItem_Type
                            {
                                TypeCode = new ContractAmountItemTypeCode_Type()
                                {
                                    Value = new XmlQualifiedName("NewBusinessCommission")
                                },
                                Commission = new Commission_Type()
                                {
                                    CommissionRatePercent = 100,
                                    CommissionRatePercentSpecified = true
                                }
                            },
                            new ContractAmountItem_Type
                            {
                                TypeCode = new ContractAmountItemTypeCode_Type()
                                {
                                    Value = new XmlQualifiedName("ServicingCommission")
                                },
                                Commission = new Commission_Type()
                                {
                                    CommissionRatePercent = 100,
                                    CommissionRatePercentSpecified = true
                                }
                            }
                        }
                    }
                },
                PolicyInterestedParty = interestedParties.ToArray()
            };

            AddBankAccountTypes(returnObj, raisePolicy);

            return returnObj;
        }

        private void AddBankAccountTypes(PolicySection_Type policySection, Models.RaisePolicy raisePolicy)
        {
            var bankAccounts = new List<BankAccount_Type>();


            switch (raisePolicy.Payment.PaymentType)
            {
                case PaymentType.DirectDebit:
                    bankAccounts.Add(AddDirectDebitDetails(raisePolicy));
                    break;
                case PaymentType.CreditCard:
                    bankAccounts.Add(AddCreditCardDetails(raisePolicy));
                    policySection.AssignedIdentifier = new[]
                    {
                        new AssignedIdentifier_Type
                        {
                            Id = raisePolicy.Payment.CreditCard.Token.Replace("-", ""),
                            RoleCode = new CoreRoleCode_Type {Value = new XmlQualifiedName("Insurer")},
                            IdentifierDescription = "CreditCardToken"
                        }
                    };
                    break;
                case PaymentType.SelfManagedSuperFund:
                    bankAccounts.Add(AddSMSFDetails(raisePolicy));
                    break;
            }

            policySection.PolicyBilling = new PolicyBilling_Type()
            {
                FrequencyCode = new BillingFrequencyCode_Type()
                {
                    Value = new XmlQualifiedName(raisePolicy.PremiumFrequency.ToShortString())
                },
                BankAccount = bankAccounts.ToArray()
            };
        }

        private BankAccount_Type AddCreditCardDetails(Models.RaisePolicy raisePolicy)
        {
            var bankAccountType = new BankAccount_Type
            {
                CardExpirationYearMonth = GetCreditExpiryYearMonth(raisePolicy.Payment.CreditCard.ExpiryMonth, raisePolicy.Payment.CreditCard.ExpiryYear),
                AccountNumberId = raisePolicy.Payment.CreditCard.CardNumber,
                TypeCode = new BankAccountTypeCode_Type
                {
                    Value = new XmlQualifiedName("Credit")
                },
                ContinuousAuthorityIndicator = true,
                ContinuousAuthorityIndicatorSpecified = true
            };

            return bankAccountType;
        }

        private static string GetCreditExpiryYearMonth(string expMonth, string expYear)
        {
            if (!string.IsNullOrEmpty(expMonth))
            {
                var month = int.Parse(expMonth);
                expMonth = month.ToString("D2");
            }

            if (!string.IsNullOrEmpty(expYear))
            {
                var year = int.Parse(expYear);
                expYear = year.ToString("D2");
            }

            return $"{expMonth}{expYear}";
        }

        private BankAccount_Type AddDirectDebitDetails(Models.RaisePolicy raisePolicy)
        {
            var bankAccountType = new BankAccount_Type();
            bankAccountType.BankBSBNumber = raisePolicy.Payment.DirectDebit.BSBNumber;
            bankAccountType.AccountNumberId = raisePolicy.Payment.DirectDebit.AccountNumber;
            bankAccountType.AccountHolder = new AccountHolder_Type()
            {
                PersonReferences = new PersonReferences_Type()
                {
                    PersonName = new List<PersonName_Type>
                    {
                        new PersonName_Type()
                        {
                            FullName = raisePolicy.Payment.DirectDebit.AccountName
                        }
                    }.ToArray()
                }
            };

            //need to set both of these for serialization to work properly. Magic!
            bankAccountType.ContinuousAuthorityIndicator = true;
            bankAccountType.ContinuousAuthorityIndicatorSpecified = true;

            return bankAccountType;
        }

        private BankAccount_Type AddSMSFDetails(Models.RaisePolicy raisePolicy)
        {
            var bankAccountType = new BankAccount_Type();
            bankAccountType.BankBSBNumber = raisePolicy.Payment.SelfManagedSuperFund.BSBNumber;
            bankAccountType.AccountNumberId = raisePolicy.Payment.SelfManagedSuperFund.AccountNumber;
            bankAccountType.AccountHolder = new AccountHolder_Type()
            {
                PersonReferences = new PersonReferences_Type()
                {
                    PersonName = new []
                    {
                        new PersonName_Type
                        {
                            FullName = raisePolicy.Payment.SelfManagedSuperFund.AccountName
                        }
                    }
                }
            };

            //need to set both of these for serialization to work properly. Magic!
            bankAccountType.ContinuousAuthorityIndicator = true;
            bankAccountType.ContinuousAuthorityIndicatorSpecified = true;

            return bankAccountType;
        }

        private Person_Type[] AddPeople(RaisePolicyOwner policyOwner, RaisePolicyRisk primaryRisk)
        {
            var people = new List<Person_Type>();
            people.Add(AddPolicyOwner(policyOwner));
            people.Add(AddLifeInsured(primaryRisk));
            people.AddRange(AddBeneficiaries(primaryRisk));

            return people.ToArray();
        }

        private Person_Type AddPolicyOwner(RaisePolicyOwner policyOwner)
        {
            var ownerFullName = "";
            var salutation = "";
            switch (policyOwner.OwnerType)
            {
                case PolicyOwnerType.Ordinary:
                    ownerFullName = $"{policyOwner.Title} {policyOwner.FirstName} {policyOwner.Surname}";
                    salutation = $"{policyOwner.Title} {policyOwner.Surname}";
                    break;
                case PolicyOwnerType.SelfManagedSuperFund:
                    ownerFullName = policyOwner.FundName;
                    salutation = "Trustees";
                    break;
            }

            var personOwner = new Person_Type
            {
                key = $"{PasInterestedPartyRoleCodeConstants.PersonKey}0",
                PersonName = new []
                {
                    new PersonName_Type
                    {
                        Surname = policyOwner.Surname,
                        GivenName = policyOwner.FirstName,
                        FullName = ownerFullName,
                        PersonTitlePrefix = salutation,
                        PersonTitlePrefixCode = new PersonTitlePrefixCode_Type { Value = new XmlQualifiedName(policyOwner.Title.ToString()) }
                    }
                },
                PersonCommunication = new PersonCommunication_Type
                {
                    Telephone = AddPersonTelephoneNumbers(policyOwner.MobileNumber, policyOwner.HomeNumber),
                    Email = new []
                    {
                        new Email_Type
                        {
                            EmailAddress = new [] { policyOwner.EmailAddress }
                        }
                    },
                    MailingAddress = AddPersonAddresses(0, AddPersonCommunicationPreferences())
                }
            };

            return personOwner;
        }

        private Person_Type AddLifeInsured(RaisePolicyRisk primaryRisk)
        {
            var lifeInsured = new Person_Type
            {
                key = $"{PasInterestedPartyRoleCodeConstants.PersonKey}1",
                PersonName = new []
                {
                    new PersonName_Type
                    {
                        Surname = primaryRisk.Surname,
                        GivenName = primaryRisk.FirstName,
                        FullName = $"{primaryRisk.Title} {primaryRisk.FirstName} {primaryRisk.Surname}",
                        PersonTitlePrefix = $"{primaryRisk.Title} {primaryRisk.Surname}",
                        PersonTitlePrefixCode = new PersonTitlePrefixCode_Type
                        {
                            Value = new XmlQualifiedName(primaryRisk.Title.ToString())
                        }
                    }
                },
                PersonCommunication = new PersonCommunication_Type
                {
                    Telephone = AddPersonTelephoneNumbers(primaryRisk.MobileNumber, primaryRisk.HomeNumber),
                    Email = new[]
                    {
                        new Email_Type
                        {
                            EmailAddress = new [] { primaryRisk.EmailAddress }
                        }
                    },
                    MailingAddress = AddPersonAddresses(1, AddPersonCommunicationPreferences())
                },
                GenderCode = new GenderCode_Type
                {
                    Value = new XmlQualifiedName(primaryRisk.Gender.ToString())
                },
                BirthDate = primaryRisk.DateOfBirth,
                BirthDateSpecified = true,
                Health = new Health_Type()
                {
                    //need to set both of these for serialization to work properly. Magic!
                    SmokerIndicator = primaryRisk.SmokerStatus == SmokerStatus.Yes,
                    SmokerIndicatorSpecified = true,
                    HealthStatus = new Code_Type { Value = "HealthSense" }
                },
                Occupation = new []
                {
                    new Occupation_Type
                    {
                        OccupationClassCode =new []
                        {
                            new OccupationClassCode_Type
                            {
                                Value = new XmlQualifiedName(primaryRisk.PasCode)
                            }
                        }
                    }
                }
            };

            return lifeInsured;
        }

        private Person_Type[] AddBeneficiaries(RaisePolicyRisk primaryRisk)
        {
            var beneficiaries = primaryRisk.Beneficiaries.Select((r, i) => new Person_Type
            {
                //key is incremented by 2 as 0 is owner and 1 is life insured
                key = $"{PasInterestedPartyRoleCodeConstants.PersonKey}{i + 2}",
                PersonName = new []
                {
                    new PersonName_Type
                    {
                        Surname = r.Surname,
                        GivenName = r.FirstName,
                        FullName = $"{r.Title} {r.FirstName} {r.Surname}",
                        PersonTitlePrefix = $"{r.Title} {r.Surname}",
                        PersonTitlePrefixCode = new PersonTitlePrefixCode_Type
                        {
                            Value = new XmlQualifiedName(r.Title.ToString())
                        },
                        TypeCode = new PersonNameTypeCode_Type
                        {
                            Value = new XmlQualifiedName("Beneficiary")
                        }
                    }
                },
                PersonCommunication = new PersonCommunication_Type
                {
                    Telephone = AddBeneficiaryTelephoneNumbers(r),
                    Email = AddBeneficiaryEmails(r),
                    MailingAddress = AddPersonAddresses(i + 2)
                },
                GenderCode = new GenderCode_Type
                {
                    Value = new XmlQualifiedName(r.Gender.ToString())
                },
                BirthDate = r.DateOfBirth.Value,
                BirthDateSpecified = true
            }).ToArray();
            
            return beneficiaries;
        }

        private CommunicationPreferences_Type[] AddPersonCommunicationPreferences()
        {
            return new []
            {
                new CommunicationPreferences_Type
                {
                    DoNotContactIndicator = false,
                    DoNotContactIndicatorSpecified = true
                }
            };
        }

        private MailingAddress_Type[] AddPersonAddresses(int idx, CommunicationPreferences_Type[] communicationPreferences = null)
        {
            var returnObj = new []
            {
                new MailingAddress_Type
                {
                    PrimaryIndicator = true,
                    PrimaryIndicatorSpecified = true,
                    key = "ADDRESS" + idx,
                    CommunicationPreferences = communicationPreferences
                }
            };

            return returnObj;
        }

        private Email_Type[] AddBeneficiaryEmails(RaisePolicyBeneficiary risk)
        {
            var returnObj = new List<Email_Type>()
            {
                new Email_Type()
                {
                    //Don't currently capture email type
                    /*TypeCode = new EmailTypeCode_Type()
                    {
                        Value = new XmlQualifiedName("Business")
                    },*/
                    EmailAddress = new List<string>()
                    {
                        risk.EmailAddress
                    }.ToArray()
                }
            }.ToArray();
            return returnObj;
        }
        
        private Telephone_Type[] AddPersonTelephoneNumbers(string mobileNumber, string homeNumber)
        {
            var returnObj = new List<Telephone_Type>
            {
                new Telephone_Type
                {
                    PrimaryIndicator = true,
                    PrimaryIndicatorSpecified = true,
                    TypeCode = new TelephoneTypeCode_Type
                    {
                        Value = new XmlQualifiedName("Mobile")
                    },
                    PhoneNumberUnformatted = mobileNumber
                }
            };
            if (!string.IsNullOrEmpty(homeNumber))
            {
                returnObj.Add(new Telephone_Type
                {
                    PrimaryIndicator = false,
                    PrimaryIndicatorSpecified = true,
                    TypeCode = new TelephoneTypeCode_Type
                    {
                        Value = new XmlQualifiedName("Home")
                    },
                    PhoneNumberUnformatted = homeNumber
                });
            }

            return returnObj.ToArray();
        }

        private Telephone_Type[] AddBeneficiaryTelephoneNumbers(RaisePolicyBeneficiary risk)
        {
            var returnObj = new List<Telephone_Type>()
            {
                new Telephone_Type()
                {
                    PrimaryIndicator = true,
                    PhoneNumber = risk.PhoneNumber,
                }
            }.ToArray();
            return returnObj;
        }

        private Address_Type[] AddAddresses(RaisePolicyOwner policyOwner, RaisePolicyRisk primaryRisk)
        {
            var ownerAddress = new Address_Type
            {
                key = "ADDRESS0",
                LineOne = policyOwner.Address, //Break up address into (x4) lines?
                SuburbName = policyOwner.Suburb,
                StateOrProvinceCode = new Code_Type
                {
                    Value = policyOwner.State.ToString()
                },
                PostalCode = new Code_Type
                {
                    Value = policyOwner.Postcode
                },
                CountryName = policyOwner.Country.ToString(),
                CountryCode = new Code_Type {Value = "AU"},
                FullAddressDescription = $"{policyOwner.Address} {policyOwner.Suburb} {policyOwner.State} {policyOwner.Postcode}"
            };

            var lifeInsuredAddress = new Address_Type
            {
                key = "ADDRESS1",
                //LineOne = policyOwner.Address, //Break up address into (x4) lines?
                //SuburbName = policyOwner.Suburb,
                //StateOrProvinceCode = new Code_Type
                //{
                //    Value = policyOwner.State.ToString()
                //},
                PostalCode = new Code_Type
                {
                    Value = primaryRisk.Postcode
                },
                //CountryName = policyOwner.Country.ToString(),
                //CountryCode = new Code_Type { Value = "AU" },
                //FullAddressDescription = $"{policyOwner.Address} {policyOwner.Suburb} {policyOwner.State} {policyOwner.Postcode}"
            };

            var beneficiaryAddresses = primaryRisk.Beneficiaries.Select((p, i) => new Address_Type
            {
                key = "ADDRESS" + (i + 2).ToString(),
                LineOne = p.Address, //Break up address into (x4) lines?
                SuburbName = p.Suburb,
                StateOrProvinceCode = new Code_Type()
                {
                    Value = p.State.ToString()
                },
                PostalCode = new Code_Type()
                {
                    Value = p.Postcode.ToString()
                },
                CountryName = p.Country.ToString(),
                FullAddressDescription = $"{p.Address} {p.Suburb} {p.State} {p.Postcode}"
            }).ToList();

            var addresses = new List<Address_Type>();
            addresses.Add(ownerAddress);
            addresses.Add(lifeInsuredAddress);
            addresses.AddRange(beneficiaryAddresses);
            
            return addresses.ToArray();
        }
    }
}