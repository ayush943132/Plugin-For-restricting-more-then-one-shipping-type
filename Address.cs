using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Metadata;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MYPlugin
{
    public class Address : IPlugin


    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService =
            (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.  
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            // The InputParameters collection contains all the data passed in the message request.  
            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
            {
                // Obtain the target entity from the input parameters.  
                Entity customeraddress = (Entity)context.InputParameters["Target"];

                // Obtain the organization service reference which you will need for  
                // web service calls.  
                IOrganizationServiceFactory serviceFactory =
                    (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                try
                {
                    // Plug-in business logic goes here

                    //string AdType = string.Empty;
                    //if (contact.Attributes.Contains("address1_addresstypecode"))
                    //{
                    //    AdType = contact.FormattedValues["address1_addresstypecode"].ToString();

                    //}
                    //    string Address = string.Empty;
                    //    if (contact.Attributes.Contains("address1_name"))
                    //    {
                    //        Address = contact.Attributes["address1_name"].ToString();
                    //    }

                    //    Guid guid = contact.GetAttributeValue<EntityReference>("address1_name").Id;

                    //    QueryExpression query = new QueryExpression("contact");
                    //    query.ColumnSet = new ColumnSet(new string[] { "address1_name" });

                    //    query.Criteria.AddCondition("address1_name", ConditionOperator.Contains, AdType);

                    //    EntityCollection collection = service.RetrieveMultiple(query);


                    //    foreach (Entity address1_addresstypecode in collection.Entities)
                    //    {
                    //        if (contact.Attributes.Contains("address1_addresstypecode"))
                    //        {

                    //        }


                    //        //if (collection.Entities.Count > 0)
                    //        //{
                    //        //    throw new InvalidPluginExecutionException("addresstype alreaddy exists");
                    //        //}

                    //        // String text = contact.FormattedValues["address1_addresstypecode"].ToString();

                    //        //address.("address1_addresstypecode", new optionsetvalue(2));
                    //    }
                    //}

                    int count = 0;  

                    int getcurrentAdtype = ((OptionSetValue)customeraddress["addresstypecode"]).Value;



                    QueryExpression query = new QueryExpression("customeraddress");
                    Guid contactId = ((EntityReference)customeraddress.Attributes["parentid"]).Id;
                    query.ColumnSet = new ColumnSet(new string[] { "parentid", "addresstypecode", "customeraddressid" });
                    query.Criteria.AddCondition("parentid", ConditionOperator.Equal, contactId);



                    EntityCollection collection = service.RetrieveMultiple(query);

                    foreach (Entity address in collection.Entities)
                    {
                        if (address.Attributes.Contains("addresstypecode") && getcurrentAdtype != 4)
                        {
                            int NewAdType = address.GetAttributeValue<OptionSetValue>("addresstypecode").Value;

                            //NewAdType= Convert.ToInt32(NewAdType);  

                            //if(NewAdType == 4)
                            //{
                            //    service.Create(address);
                            //    count++;

                            //    if(count > 2)
                            //    {
                            //        throw new InvalidPluginExecutionException("There are already two others");
                            //    }
                            //}

                            //if(getcurrentAdtype == NewAdType && NewAdType ==  OptionSetValue(4))
                            //{
                            //    count++;    

                            //    if(count > 2)
                            //    {
                            //        throw new InvalidPluginExecutionException("Address type is already present");
                            //    }
                            //}


                            if (getcurrentAdtype == NewAdType)
                            {
                                throw new InvalidPluginExecutionException("Contact already contains this address type:" + getcurrentAdtype.ToString());
                            }
                        }

                        else if (getcurrentAdtype == 4)
                        {
                            count++;
                        }

                        if (count > 3 && getcurrentAdtype == 4)
                        {
                            throw new InvalidPluginExecutionException("others have exceeded the limit of 2");
                        } 
                        
                           

                        
                    }







                }

                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in FollowUpPlugin.", ex);
                }

                catch (Exception ex)
                {
                    tracingService.Trace("FollowUpPlugin: {0}", ex.ToString());
                    throw;
                }
            }

        }
    }
}